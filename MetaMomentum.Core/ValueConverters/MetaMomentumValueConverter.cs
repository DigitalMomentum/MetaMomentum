using MetaMomentum.Models;
using MetaMomentum.Config;
using System;
#if NET5_0_OR_GREATER
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;
#else
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web.PublishedCache;
#endif

namespace MetaMomentum.Core.ValueConverters {
	public class MetaMomentumValueConverter : IPropertyValueConverter {
		private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;

		private readonly Utility.Logging<MetaMomentumValueConverter> _logger;
		private readonly MetaMomentumConfig _metaMomentumConfig;

#if NET5_0_OR_GREATER
		private readonly JsonSerializerOptions jsonOptions = new() {
			PropertyNameCaseInsensitive = true,
		};

		public MetaMomentumValueConverter(
			IPublishedSnapshotAccessor publishedSnapshotAccessor,
			ILogger<MetaMomentumValueConverter> logger, 
			MetaMomentumConfig metaMomentumConfig) {
			_publishedSnapshotAccessor = publishedSnapshotAccessor ?? throw new ArgumentNullException(nameof(publishedSnapshotAccessor));
			_metaMomentumConfig = metaMomentumConfig;
			_logger = new Utility.Logging<MetaMomentumValueConverter>(logger);
		}
#else
		public MetaMomentumValueConverter(
			IPublishedSnapshotAccessor publishedSnapshotAccessor,
			ILogger logger,
			MetaMomentumConfig metaMomentumConfig) {
			_publishedSnapshotAccessor = publishedSnapshotAccessor ?? throw new ArgumentNullException(nameof(publishedSnapshotAccessor));

			_metaMomentumConfig = metaMomentumConfig;
			_logger = new Utility.Logging<MetaMomentumValueConverter>(logger);
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

			return val?.Title != null || val?.Description != null;
		}

		public object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview) {
			return MapToMetaValue(source, preview, propertyType, owner);
		}

		/// <summary>
		/// Converts the datatype data string to a MetaValues object
		/// </summary>
		/// <param name="source">DataType Data</param>
		/// <param name="propertyType">Pass the property type, to sucessfully use the fallbacks</param>
		/// <param name="IPublishedElement">Pass the page element, to sucessfully use the fallbacks</param>
		/// <returns></returns>
		private MetaValues MapToMetaValue(object source, bool preview, IPublishedPropertyType propertyType = null, IPublishedElement owner = null) {

			if (source == null) return null;
			var sourceString = source.ToString();
			if (String.IsNullOrWhiteSpace(sourceString)) return null;

			try {

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

				GuidUdi guidUdi = md.GetShareImageUdi();
				IPublishedSnapshot publishedSnapshot;
#if NET5_0_OR_GREATER
				_publishedSnapshotAccessor.TryGetPublishedSnapshot(out publishedSnapshot);
#else
				publishedSnapshot = _publishedSnapshotAccessor.PublishedSnapshot;
#endif
				IPublishedContent img = (guidUdi == null) ? null : publishedSnapshot.Media.GetById(preview, guidUdi.Guid);
				if (img != null && md.ShareImageUrl == null) {
					//Handle Backwards compatibility where we used to store the image as an IPublished Content, rather than a URL
					md.ShareImageUrl = img.UrlSegment;
				}

				var retVal = new MetaValues() {
					Description = md.Description,
					ShareDescription = md.ShareDescription,
					ShareImage = img,
					ShareImageUrl = md.ShareImageUrl,
					ShareTitle = md.ShareTitle,
					Title = md.Title,
					NoIndex = md.NoIndex,
					OGSiteName = _metaMomentumConfig.OGSiteName,
					TwitterName = _metaMomentumConfig.TwitterName,
					FacebookAppId = _metaMomentumConfig.FacebookAppId,
					FallbackImageUrl = _metaMomentumConfig?.FallbackImageUrl
				};

				return retVal;
			} catch (Exception ex) {
				_logger.LogWarning(ex, "Cannot convert MetaMomentum");
				return null;
			}
		}

		private class MetaValuesIntermediateModel {
			public string Title { get; set; }
			public string Description { get; set; }
			public bool NoIndex { get; set; }

			public string ShareTitle { get; set; }
			public string ShareDescription { get; set; }
			public string ShareImage { get; set; }
			public string ShareImageUrl { get; set; }

			public GuidUdi GetShareImageUdi() {
#if NET5_0_OR_GREATER
				if (UdiParser.TryParse(ShareImage, out Udi udi)) {
					return udi as GuidUdi;
				}
#else
				if (Udi.TryParse(ShareImage, out Udi udi)) {
					return udi as GuidUdi;
				}
#endif
				return null;
			}
		}
	}
}
