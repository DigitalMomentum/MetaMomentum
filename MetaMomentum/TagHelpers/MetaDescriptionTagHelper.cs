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
		Attributes = "meta-values,[name=description]"
	)]
	[HtmlTargetElement(
		"meta-description"
	)]
	public class MetaDescriptionTagHelper : TagHelper {

		public MetaValues? MetaValues{ get; set; }

		public string? Content { get; set; } = null;

		public override void Process(TagHelperContext context, TagHelperOutput output) {
			output.Reinitialize(null, TagMode.SelfClosing);
			if(MetaValues!= null) {
				Content = MetaValues?.Description;
			}
			output.Content = output.Content.RenderMetaDescriptionTag(Content);


		}

	}

}
