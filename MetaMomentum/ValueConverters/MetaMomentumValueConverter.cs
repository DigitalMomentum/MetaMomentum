using MetaMomentum.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.PublishedCache;

namespace MetaMomentum.ValueConverters
{
    public class MetaMomentumValueConverter : IPropertyValueConverter
    {
        private readonly ILogger _logger;
        private readonly IContentService _contentService;
        private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;


        //private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;

        //Injecting the PublishedSnapshotAccessor for fetching content
        public MetaMomentumValueConverter(ILogger logger, IContentService contentService, IPublishedSnapshotAccessor publishedSnapshotAccessor)
        {
            _logger = logger;
            _contentService = contentService;
            _publishedSnapshotAccessor = publishedSnapshotAccessor ?? throw new ArgumentNullException(nameof(publishedSnapshotAccessor));
        }

        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("DM.MetaMomentum");
        }




        public Type GetPropertyValueType(IPublishedPropertyType propertyType)
        {
            return typeof(MetaValues);
        }

        public PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType)
        {
            return PropertyCacheLevel.Elements;
        }





        public object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            if (inter == null)
                return null;

            if ((propertyType.Alias != null) == false)
            {



            }

            return inter;
        }

        public object ConvertIntermediateToXPath(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            if (inter == null) return null;
            return inter.ToString();
        }

        public bool? IsValue(object value, PropertyValueLevel level)
        {
            if (value == null) return false;
            //TODO: This gets run each time, so need to check in a more efficient manner
            var val = mapToMetaValue(value);

            if (val == null) return false; //mapToMetaValue can return false, so need to catch this

            return (
                val.Title != null ||
                val.Description != null
              );


        }

        public object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview)
        {

            return mapToMetaValue(source, propertyType, owner);




        }


        /// <summary>
        /// Converts the datatype data string to a MetaValues object
        /// </summary>
        /// <param name="source">DataType Data</param>
        /// <param name="propertyType">Pass the property type, to sucessfully use the fallbacks</param>
        /// <param name="IPublishedElement">Pass the page element, to sucessfully use the fallbacks</param>
        /// <returns></returns>
        private MetaValues mapToMetaValue(object source, IPublishedPropertyType propertyType = null, IPublishedElement owner = null)
        {
            if (source == null) return null;
            var sourceString = source.ToString();
            if (String.IsNullOrWhiteSpace(sourceString)) return null;

            //try
            //{
                var md = JsonConvert.DeserializeObject<MetaValuesIntermediateModel>(sourceString);

            //Not sure if I need the following code. It works, but might be redundant. 
            //Lets see if we are ever missing the fallbacks from the angular component, and if so, we might need to use the following
            //if (propertyType != null && owner != null && (string.IsNullOrWhiteSpace(md.Title) || string.IsNullOrWhiteSpace(md.Description)))
            //{
            //    var config = (Dictionary<string, object>)propertyType.DataType.Configuration;
            //    //Need to setup the fall backs
            //    if (string.IsNullOrWhiteSpace(md.Title))
            //    {
            //        var fallbackAliases = config["fallbackTitleFields"].ToString().Split(',');
            //        foreach (string fallbackAlias in fallbackAliases)
            //        {
            //            var prop = owner.GetProperty(fallbackAlias.Trim());
            //            if (prop != null && prop.HasValue())
            //            {
            //                md.Title = prop.GetValue().ToString();
            //            }
            //        }
            //    }


            //    if (string.IsNullOrWhiteSpace(md.Description))
            //    {
            //        var fallbackAliases = config["fallbackDescriptionFields"].ToString().Split(',');
            //        foreach (string fallbackAlias in fallbackAliases)
            //        {
            //            var prop = owner.GetProperty(fallbackAlias.Trim());
            //            if (prop != null && prop.HasValue())
            //            {
            //                md.Description = prop.GetValue().ToString();
            //            }
            //        }
            //    }
            //}

            var objectType = UmbracoObjectTypes.Media;
            GuidUdi guidUdi = md.GetShareImageUdi();
          
          // var image = GetPublishedContent(guidUdi, ref objectType, UmbracoObjectTypes.Media, id => _publishedSnapshotAccessor.PublishedSnapshot.Media.GetById(guidUdi.Guid));

            return new MetaValues()
                {
                    Description = md.Description,
                    ShareDescription = md.ShareDescription,
                    ShareImage = _publishedSnapshotAccessor.PublishedSnapshot.Media.GetById(guidUdi.Guid),
                    ShareTitle = md.ShareTitle,
                    Title = md.Title
                };
            //}
            //catch (Exception e)
            //{
            //    _logger.Warn<MetaMomentumValueConverter>(String.Format("Can not convert MetaMomentum - {0} - {1}",
            //        e.GetType().Name, e.Message));
            //    return null;
            //}
        }


        //Copied from https://github.com/umbraco/Umbraco-CMS/blob/0bd4dced0b3e9205660114406b7e814f817179c7/src/Umbraco.Web/PropertyEditors/ValueConverters/MultiNodeTreePickerValueConverter.cs#L137
        /// <summary>
        /// Attempt to get an IPublishedContent instance based on ID and content type
        /// </summary>
        /// <param name="nodeId">The content node ID</param>
        /// <param name="actualType">The type of content being requested</param>
        /// <param name="expectedType">The type of content expected/supported by <paramref name="contentFetcher"/></param>
        /// <param name="contentFetcher">A function to fetch content of type <paramref name="expectedType"/></param>
        /// <returns>The requested content, or null if either it does not exist or <paramref name="actualType"/> does not match <paramref name="expectedType"/></returns>
        //private IPublishedContent GetPublishedContent<T>(T nodeId, ref UmbracoObjectTypes actualType, UmbracoObjectTypes expectedType, Func<T, IPublishedContent> contentFetcher)
        //{
        //    // is the actual type supported by the content fetcher?
        //    if (actualType != UmbracoObjectTypes.Unknown && actualType != expectedType)
        //    {
        //        // no, return null
        //        return null;
        //    }

        //    // attempt to get the content
        //    var content = contentFetcher(nodeId);
        //    if (content != null)
        //    {
        //        // if we found the content, assign the expected type to the actual type so we don't have to keep looking for other types of content
        //        actualType = expectedType;
        //    }
        //    return content;
        //}


        private class MetaValuesIntermediateModel
        {
            public string Title { get; set; }
            public string Description { get; set; }

            public string ShareTitle { get; set; }
            public string ShareDescription { get; set; }
            public string ShareImage { get; set; }

            public GuidUdi GetShareImageUdi(
                //IContentService _contentService
                )
            {
                Udi udi;
                if (Udi.TryParse(ShareImage, out udi))
                {
                    return  udi as GuidUdi;
                   // return _contentService.GetById(guidUdi.Guid);
                 
                }
                return null;
            }
        }
    }
}