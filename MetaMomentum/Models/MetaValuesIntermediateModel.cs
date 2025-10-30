using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core;

namespace MetaMomentum.Models {
	internal class MetaValuesIntermediateModel {
		public string Title { get; set; }
		public string Description { get; set; }
		public bool NoIndex { get; set; }

		public string ShareTitle { get; set; }
		public string ShareDescription { get; set; }
		public string? ShareImage { get; set; }
		public string ShareImageUrl { get; set; }

		//public GuidUdi GetShareImageUdi() {


		//	string imgAsString = ShareImage as string;
		//	if (imgAsString == null) return null;

		//	if (UdiParser.TryParse(imgAsString, out Udi udi)) {
		//		return udi as GuidUdi;
		//	}


		//	return null;
		//}
	}
}
