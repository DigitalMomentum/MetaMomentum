﻿using MetaMomentum.Models;
using System;
using MetaMomentum.Config;


#if NET5_0_OR_GREATER
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Microsoft.Extensions.Logging;
#else
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.PublishedCache;

#endif

namespace MetaMomentum.ValueConverters {
	public class MetaMomentumValueConverter : IPropertyValueConverter {
		private readonly IContentService _contentService;
		private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;
		
		private readonly MetaMomentumConfig _metaMomentumConfig;

#if NET5_0_OR_GREATER
		JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
        };
#endif


#if NET5_0_OR_GREATER

		private readonly ILogger<MetaMomentumValueConverter> _logger;

		public MetaMomentumValueConverter(IContentService contentService, IPublishedSnapshotAccessor publishedSnapshotAccessor, IConfiguration configuration, ILogger<MetaMomentumValueConverter> logger, MetaMomentumConfig metaMomentumConfig = null) {
			_contentService = contentService;
			_publishedSnapshotAccessor = publishedSnapshotAccessor ?? throw new ArgumentNullException(nameof(publishedSnapshotAccessor));
			if(metaMomentumConfig == null) {
				metaMomentumConfig = new MetaMomentumConfig();
			}
			_metaMomentumConfig = metaMomentumConfig;
			_logger = logger;
		}
#else

		private readonly ILogger _logger;


		public MetaMomentumValueConverter(IContentService contentService, IPublishedSnapshotAccessor publishedSnapshotAccessor, ILogger logger) {
			_contentService = contentService;
			_publishedSnapshotAccessor = publishedSnapshotAccessor ?? throw new ArgumentNullException(nameof(publishedSnapshotAccessor));
			_logger = logger;
		}

#endif
		public bool IsConverter(IPublishedPropertyType propertyType) {
			return propertyType.EditorAlias.Equals("DM.MetaMomentum");
		}




		public Type GetPropertyValueType(IPublishedPropertyType propertyType) {
			return typeof(MetaValues);
		}

		public PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) {
			return PropertyCacheLevel.Elements;
		}





		public object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview) {
			if (inter == null)
				return null;

			if ((propertyType.Alias != null) == false) {



			}

			return inter;
		}

		public object ConvertIntermediateToXPath(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview) {
			if (inter == null) return null;
			return inter.ToString();
		}

		public bool? IsValue(object value, PropertyValueLevel level) {


			if (value == null) return false;



			var sourceString = value.ToString();
			if (String.IsNullOrWhiteSpace(sourceString)) return null;
#if NET5_0_OR_GREATER
		
			MetaValuesIntermediateModel val = JsonSerializer.Deserialize<MetaValuesIntermediateModel>(sourceString, jsonOptions);	
#else
			MetaValuesIntermediateModel val = JsonConvert.DeserializeObject<MetaValuesIntermediateModel>(sourceString);
#endif

			if (val == null) return false; //mapToMetaValue can return false, so need to catch this

			return (
				val.Title != null ||
				val.Description != null
			  );


		}

		public object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview) {

			return mapToMetaValue(source, preview, propertyType, owner);




		}


		/// <summary>
		/// Converts the datatype data string to a MetaValues object
		/// </summary>
		/// <param name="source">DataType Data</param>
		/// <param name="propertyType">Pass the property type, to sucessfully use the fallbacks</param>
		/// <param name="IPublishedElement">Pass the page element, to sucessfully use the fallbacks</param>
		/// <returns></returns>
		private MetaValues mapToMetaValue(object source, bool preview, IPublishedPropertyType propertyType = null, IPublishedElement owner = null) {

			if (source == null) return null;
			var sourceString = source.ToString();
			if (String.IsNullOrWhiteSpace(sourceString)) return null;

			try{

#if NET5_0_OR_GREATER
			var md = JsonSerializer.Deserialize<MetaValuesIntermediateModel>(sourceString, jsonOptions);
#else
			var md = JsonConvert.DeserializeObject<MetaValuesIntermediateModel>(sourceString);
#endif
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
#if NET5_0_OR_GREATER
			IPublishedSnapshot publishedSnapshot;
			_publishedSnapshotAccessor.TryGetPublishedSnapshot(out publishedSnapshot);
			IPublishedContent img = (guidUdi == null) ? null : publishedSnapshot.Media.GetById(preview, guidUdi.Guid);
#else
			IPublishedContent img = (guidUdi == null) ? null : _publishedSnapshotAccessor.PublishedSnapshot.Media.GetById(preview, guidUdi.Guid);
#endif
			if (img != null && md.ShareImageUrl == null) {
				//Handle Backwards compatibility where we used to store the image as an IPublished Content, rather than a URL
				md.ShareImageUrl = img.UrlSegment;// (mode: UrlMode.Absolute);
			}

			var retVal = new MetaValues() {
				Description = md.Description,
				ShareDescription = md.ShareDescription,
				ShareImage = img,
				ShareImageUrl = md.ShareImageUrl,
				ShareTitle = md.ShareTitle,
				Title = md.Title,
				NoIndex = md.NoIndex
			};

#if NET5_0_OR_GREATER
			retVal.OGSiteName = _metaMomentumConfig.OGSiteName;
			retVal.TwitterName = _metaMomentumConfig.TwitterName;
#endif

			return retVal;


			} catch (Exception e) {
#if NET5_0_OR_GREATER
				_logger.Log(LogLevel.Warning, "Can not convert MetaMomentum - {0} - {1}", e.GetType().Name, e.Message);
#else
				_logger.Warn<MetaMomentumValueConverter>(String.Format("Can not convert MetaMomentum - {0} - {1}",
					e.GetType().Name, e.Message));
#endif
				return null;
			}


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


		private class MetaValuesIntermediateModel {
			public string Title { get; set; }
			public string Description { get; set; }
			public bool NoIndex { get; set; }

			public string ShareTitle { get; set; }
			public string ShareDescription { get; set; }
			public string ShareImage { get; set; }
			public string ShareImageUrl { get; set; }

			public GuidUdi GetShareImageUdi(
				//IContentService _contentService
				) {
				Udi udi;
#if NET5_0_OR_GREATER

				if (UdiParser.TryParse(ShareImage, out udi))
                {
                    return udi as GuidUdi;
                    // return _contentService.GetById(guidUdi.Guid);

                }

#else

				if (Udi.TryParse(ShareImage, out udi)) {
					return udi as GuidUdi;
					// return _contentService.GetById(guidUdi.Guid);

				}
#endif



				return null;
			}
		}
	}
}
