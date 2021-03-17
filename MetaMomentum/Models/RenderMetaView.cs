using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.PublishedContent;

namespace MetaMomentum.Models
{
   public class RenderMetaView : MetaValues
    {
        public string PageUrl { get; set; }
        public RenderMetaView(IPublishedContent page, MetaValues metaValues)
        {
            this.Title = metaValues.Title;
            this.Description = metaValues.Description;
            this.ShareTitle = metaValues.ShareTitle;
            this.ShareDescription = metaValues.ShareDescription;
            this.ShareImage = metaValues.ShareImage;
            //PageUrl = page.Url.abs();
        }
    }
}
