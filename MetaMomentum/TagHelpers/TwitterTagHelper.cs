using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaMomentum.Extensions;
using MetaMomentum.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MetaMomentum.TagHelpers {


	[HtmlTargetElement(
	"meta-twitter-card"
)]

	public class TwitterTagHelper : TagHelper {
		public MetaValues? MetaValues { get; set; }

		public string? ShareTitle { get; set; } = null;
		public string? ShareDescription { get; set; } = null;
		public string? ShareImageUrl { get; set; } = null;
		public string? TwitterName { get; set; } = null;

		public override void Process(TagHelperContext context, TagHelperOutput output) {
			output.Reinitialize(null, TagMode.StartTagAndEndTag);

			if (MetaValues != null) {

				if (MetaValues.ShareTitle != null) {
					ShareTitle = MetaValues.ShareTitle;
				}
				if (MetaValues.ShareDescription != null) {
					ShareDescription = MetaValues.ShareDescription;
				}
				if (MetaValues.ShareImageUrl != null) {
					ShareImageUrl = MetaValues.ShareImageUrl;
				}
				if (MetaValues.TwitterName != null) {
					TwitterName = MetaValues.TwitterName;
				}


			}


			output.Content = output.Content
				.RenderTwitterCardTags(MetaValues?.ShareTitle, MetaValues?.ShareDescription, MetaValues?.ShareImageUrl, MetaValues?.TwitterName);
		}

	}
}
