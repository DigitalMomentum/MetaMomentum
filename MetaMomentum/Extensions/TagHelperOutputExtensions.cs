using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaMomentum.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MetaMomentum.Extensions {
	public static class TagHelperOutputExtensions {



		public static TagHelperContent RenderMetaTitleTag(this TagHelperContent content, string? title) {
			
			if (title == null) return content;
			
			return content.AppendHtml($"<title>{title}</title>");
			
		}


		public static TagHelperContent RenderMetaDescriptionTag(this TagHelperContent content, string? description) {

			if (description == null) return content;

			return content.AppendHtml($"<meta name='description' content='{description}' />");

		}

		public static TagHelperContent RenderMetaRobotsTag(this TagHelperContent content, bool? noIndex) {

			if (noIndex == null || !noIndex.Value) return content;

			return content.AppendHtml($"<meta name='robots' content='nofollow' />");

		}


		public static TagHelperContent RenderOpenGraphTitleTag(this TagHelperContent content, string? shareTitle) {
			if (shareTitle == null) return content;

			return content.AppendHtml($"<meta property='og:title' content='{shareTitle}' />");

		}
		public static TagHelperContent RenderOpenGraphDescriptionTag(this TagHelperContent content, string? description) {
			if (description == null) return content;

			return content.AppendHtml($"<meta property='og:description' content='{description}' />");

		}

		public static TagHelperContent RenderOpenGraphImageTag(this TagHelperContent content, string? url) {
			if (url == null) return content;
			//Meta(FB) require images at least 1200x630 https://developers.facebook.com/docs/sharing/webmasters/images/
			return content.AppendHtml($"<meta property='og:image:width' content='1200' /><meta property='og:image:height' content='630' /><meta property='og:image' content='{url}?width=1200&height=630&bgcolor=white' />"); //TODO: Better handling of query string needed here

		}

		public static TagHelperContent RenderOpenGraphSiteNameTag(this TagHelperContent content, string? siteName) {
			if (siteName == null) return content;

			return content.AppendHtml($"<meta property='og:site_name' content='{siteName}' />");

		}


		public static TagHelperContent RenderTwitterSiteNameTag(this TagHelperContent content, string? siteName) {
			if (siteName == null) return content;

			return content.AppendHtml($"<meta name='twitter:site' content='{siteName}' />");

		}


		public static TagHelperContent RenderTwitterTitleTag(this TagHelperContent content, string? shareTitle) {
			if (shareTitle == null) return content;

			return content.AppendHtml($"<meta name='twitter:title' content='{shareTitle}' />");

		}
		public static TagHelperContent RenderTwitterDescriptionTag(this TagHelperContent content, string? shareDescription) {
			if (shareDescription == null) return content;

			return content.AppendHtml($"<meta name='twitter:description' content='{shareDescription}' />");

		}

		public static TagHelperContent RenderTwitterImageTag(this TagHelperContent content, string? url) {
			if (url == null) return content;

			//Twitter Card ratio is 2:1 https://developer.twitter.com/en/docs/twitter-for-websites/cards/overview/summary-card-with-large-image
			return content.AppendHtml($"<meta name='twitter:image' content='{url}?width=1200&height=600&bgcolor=white' />"); //TODO: Better handling of query string needed here

		}


		public static TagHelperContent RenderTwitterCardTags(this TagHelperContent content, string? title, string? description, string? imageUrl, string? twitterSiteName = null) {

			return content
				.AppendHtml("<meta name='twitter:card' content='summary_large_image' />")
				.RenderTwitterTitleTag(title)
				.RenderTwitterDescriptionTag(description)
				.RenderTwitterImageTag(imageUrl)
				.RenderTwitterSiteNameTag(twitterSiteName);
		}

		public static TagHelperContent RenderOpenGraphTags(this TagHelperContent content, string? title, string? description, string? imageUrl, string? siteName = null, string? facebookAppId = null) {

			return content
				.AppendHtml("<meta property='og:type' content='website' />")
				.RenderOpenGraphTitleTag(title)
				.RenderOpenGraphDescriptionTag(description)
				.RenderOpenGraphImageTag(imageUrl)
				.RenderOpenGraphSiteNameTag(siteName);
		}
	}
}

