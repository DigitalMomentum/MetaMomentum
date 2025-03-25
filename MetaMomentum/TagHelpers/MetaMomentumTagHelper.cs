using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaMomentum.Extensions;
using MetaMomentum.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MetaMomentum.TagHelpers {

	[HtmlTargetElement(
		"meta-momentum"
	)]
	public class MetaMomentumTagHelper : TagHelper {

		public MetaValues? MetaValues { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output) {
			output.Reinitialize(null, TagMode.StartTagAndEndTag);

			if(MetaValues == null) {
				return;
			}

			
			output.Content = output.Content
				.RenderMetaRobotsTag(MetaValues?.NoIndex)
				.RenderMetaTitleTag(MetaValues?.Title)
				.RenderMetaDescriptionTag(MetaValues?.Description)
				.RenderTwitterCardTags(MetaValues?.ShareTitle, MetaValues?.ShareDescription, MetaValues?.ShareImageUrl, MetaValues?.TwitterName)
				.RenderOpenGraphTags(MetaValues?.ShareTitle, MetaValues?.ShareDescription, MetaValues?.ShareImageUrl, MetaValues?.OGSiteName, MetaValues?.FacebookAppId);
		}

	}
}
