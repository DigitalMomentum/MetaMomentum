using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace MetaMomentum.Models {
	public class MetaValues {
		public string Title { get; set; }
		public string Description { get; set; }
		public bool NoIndex { get; set; }

		public string ShareTitle { get; set; }
		public string ShareDescription { get; set; }

		/// <summary>
		/// This value can be set in configuration under AppSettings Name = "MetaMomentum.OGSiteName"
		/// </summary>
		public string OGSiteName { get; set; }

		/// <summary>
		/// This value can be set in the Web.Config under AppSettings.json under `MetaMomentum.TwitterName`. Make sure that you include the @ symbol
		/// </summary>
		public string TwitterName { get; set; }

		/// <summary>
		/// This value can be set in the Web.Config under AppSettings.json under `MetaMomentum.TwitterName`. Make sure that you include the @ symbol
		/// </summary>
		[Obsolete("Facebook no longer uses the App ID when sharing so this has been removed. The Value will always be null.")]
		public string FacebookAppId { get; set; } = null;

		public string ShareImageUrl { get; set; }


		public string GetAbsoluteShareImageUrl(HttpRequest httpRequest) {
			return $"{httpRequest.Scheme}://{httpRequest.Host}{ShareImageUrl}";
		}



	}
}
