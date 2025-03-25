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
		"title",
		Attributes = "meta-values"
	)]
	[HtmlTargetElement(
		"meta-title"
	)]
	public class MetaTitleTagHelper : TagHelper {

		public MetaValues? MetaValues{ get; set; }

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output) {
			output.Reinitialize(null, TagMode.SelfClosing);

			string? title = null;
			if (MetaValues != null) {
				title = MetaValues.Title;
			} else {
				title = (await output.GetChildContentAsync()).GetContent();
			}

			output.Content = output.Content.RenderMetaTitleTag(title);

		}

	}

}
