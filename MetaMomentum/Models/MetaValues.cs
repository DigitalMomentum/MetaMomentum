using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace MetaMomentum.Models
{
    public class MetaValues
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public string ShareTitle { get; set; }
        public string ShareDescription { get; set; }

        /// <summary>
        /// This value can be set in the Web.Config under AppSettings Name = "MetaMomentum.OGSiteName"
        /// </summary>
        public string OGSiteName
        {
            get
            {
                return ConfigurationManager.AppSettings["MetaMomentum.OGSiteName"];
            }
        }

        /// <summary>
        /// This value can be set in the Web.Config under AppSettings Name = "MetaMomentum.TwitterName". Make sure that you include the @ symbol
        /// </summary>
        public string TwitterName
        {
            get
            {
                return ConfigurationManager.AppSettings["MetaMomentum.TwitterName"];
            }
        }


        public IPublishedContent ShareImage { get; set; }


    }
}
