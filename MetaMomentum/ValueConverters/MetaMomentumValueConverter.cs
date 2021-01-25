using MetaMomentum.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace MetaMomentum.ValueConverters
{
    public class MetaMomentumValueConverter : IPropertyValueConverter
    {
        private readonly ILogger _logger;

        //private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;

        //Injecting the PublishedSnapshotAccessor for fetching content
        public MetaMomentumValueConverter(ILogger logger)
        {
            _logger = logger;
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

            var val = mapToMetaValue(value);

            if(val == null) return false; //mapToMetaValue can return false, so need to catch this

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

            try
            {
                var md = JsonConvert.DeserializeObject<MetaValues>(sourceString);

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


                return md;
            }
            catch (Exception e)
            {
                _logger.Warn<MetaMomentumValueConverter>(String.Format("Can not convert MetaMomentum - {0} - {1}",
                    e.GetType().Name, e.Message));
                return null;
            }
        }
    }
}
