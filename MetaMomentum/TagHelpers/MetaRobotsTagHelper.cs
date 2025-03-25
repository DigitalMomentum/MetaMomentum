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
		"meta",
		Attributes = "meta-values,[name=robots]"
	)]
	[HtmlTargetElement(
		"meta-robots"
	)]
	public class MetaRobotsTagHelper : TagHelper {

		public MetaValues? MetaValues{ get; set; }


		//Todo: Add support for other robots meta values like nofollow, noarchive, nosnippet, noindex, noimageindex, max-snippet:-1, max-image-preview:large, max-video-preview:-1

		public override void Process(TagHelperContext context, TagHelperOutput output) {
			output.Reinitialize(null, TagMode.SelfClosing);




			output.Content = output.Content.RenderMetaRobotsTag(MetaValues?.NoIndex);


		}

	}

}
