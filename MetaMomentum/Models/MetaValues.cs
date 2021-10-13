using System;
using System.Configuration;

#if NET472
using Umbraco.Core.Models.PublishedContent;
using System.Web;
#elif NET5_0_OR_GREATER
using Umbraco.Cms.Core.Models.PublishedContent;
using Microsoft.AspNetCore.Http;
#endif
namespace MetaMomentum.Models {
	public class MetaValues {
		public string Title { get; set; }
		public string Description { get; set; }
		public bool NoIndex { get; set; }

		public string ShareTitle { get; set; }
		public string ShareDescription { get; set; }


#if NET5_0_OR_GREATER
		/// <summary>
		/// This value can be set in the Web.Config under AppSettings Name = "MetaMomentum.OGSiteName"
		/// Note: This only works via ValueConverter OR ModelsBuilder
		/// </summary>
		public string OGSiteName { get; set; } = null;


		/// <summary>
		/// This value can be set in the Web.Config under AppSettings.json under `MetaMomentum.TwitterName`. Make sure that you include the @ symbol
		/// Note: This only works via ValueConverter OR ModelsBuilder
		/// </summary>
		public string TwitterName { get; set; } = null;

#else

		/// <summary>
		/// This value can be set in the Web.Config under AppSettings Name = "MetaMomentum.OGSiteName"
		/// </summary>
		public string OGSiteName {
			get {
				return ConfigurationManager.AppSettings["MetaMomentum.OGSiteName"];
			}
		}
		/// <summary>
		/// This value can be set in the Web.Config under AppSettings Name = "MetaMomentum.TwitterName". Make sure that you include the @ symbol
		/// </summary>
		public string TwitterName {
			get {
				return ConfigurationManager.AppSettings["MetaMomentum.TwitterName"];

			}
		}
				

#endif

		[Obsolete("Please use ShareImageUrl instead")]
		public IPublishedContent ShareImage { get; set; }


		public string ShareImageUrl { get; set; }


#if NET5_0_OR_GREATER

		public string GetAbsoluteShareImageUrl(HttpRequest httpRequest) {
			return httpRequest.Scheme + "://" + httpRequest.Host + ShareImageUrl;
		}
#else
		public string GetAbsoluteShareImageUrl(HttpRequestBase httpRequest) {
			return httpRequest.Url.Scheme + "://" + httpRequest.Url.Authority + ShareImageUrl;
		}
#endif


	}
}
