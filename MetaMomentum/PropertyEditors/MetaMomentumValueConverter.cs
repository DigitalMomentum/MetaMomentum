using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MetaMomentum.Models;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Extensions;

namespace MetaMomentum.PropertyEditors {
	public class MetaMomentumValueConverter(IPublishedMediaCache publishedMediaCache, ILogger<MetaMomentumValueConverter> logger) : IPropertyValueConverter {


		
		private readonly JsonSerializerOptions jsonOptions = new() {
			PropertyNameCaseInsensitive = true,
		};


		public object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview) {
			if (inter == null)
				return null;

			if (inter is MetaValuesIntermediateModel val) {
				//NOTE: This might be a good place to investigate server side fall backs here


				//GuidUdi guidUdi = val.GetShareImageUdi();
				var img = (val.ShareImage == null) ? null : publishedMediaCache.GetById(val.ShareImage);//.ConfigureAwait(false).GetAwaiter().GetResult();

				//if (img != null && val.ShareImageUrl == null) {
				//	//Handle Backwards compatibility where we used to store the image as an IPublished Content, rather than a URL
				//	val.ShareImageUrl = img.UrlSegment!;
				//}

				Dictionary<string, object> config = null;

				try {
					config = propertyType.DataType.ConfigurationAs<Dictionary<string, object>>();
				} catch {
					//This sometimes throws an error when upgrading from pre Umb 15 before the Data type is not re-saved
					logger.LogWarning("MetaMomentum Upgrade warning: Please open up the {alias} Data Type and click save to upgrade the data type and be rid of this warning", propertyType.Alias);
				}

				var retVal = new MetaValues() {
					Description = val.Description,
					ShareDescription = val.ShareDescription,
					//ShareImage = img,
					ShareImageUrl = val.ShareImageUrl ?? img?.MediaUrl(),
					ShareTitle = val.ShareTitle,
					Title = val.Title,
					NoIndex = val.NoIndex,
					//TODO: Add these to Property Config rather than App Settings
					OGSiteName = config?.GetValueAsString("siteName"),
					TwitterName = config?.GetValueAsString("twitterName")
				};

				return retVal;

			}





			return inter;
		}

		public object? ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview) {
			if (source == null) return false;

			var sourceString = source.ToString();
			if (String.IsNullOrWhiteSpace(sourceString)) return null;

			//Todo: Handle UUIDs here for backwards compatibility
			MetaValuesIntermediateModel val = JsonSerializer.Deserialize<MetaValuesIntermediateModel>(sourceString, jsonOptions);
			return val;
		}

		public PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) {
#if DEBUG
			return PropertyCacheLevel.None; //Don't cache whilst we are debugging and working out the Value Converter. 
#endif
			return PropertyCacheLevel.Elements; //Cache on node level so we can fall back to other properties on the server side
		}

		public Type GetPropertyValueType(IPublishedPropertyType propertyType) {
			return typeof(MetaValues);
		}

		public bool IsConverter(IPublishedPropertyType propertyType) {
			return propertyType.EditorUiAlias.Equals("DM.MetaMomentum");
		}

		public bool? IsValue(object? value, PropertyValueLevel level) {

			//TODO: This is the default, so need to inspect and change as needed.
			return level switch {
				PropertyValueLevel.Source => value != null && (!(value is string value2) || !string.IsNullOrWhiteSpace(value2)),
				PropertyValueLevel.Inter => null,
				PropertyValueLevel.Object => null,
				_ => throw new NotSupportedException($"Invalid level: {level}."),
			};
		}
	}
}
