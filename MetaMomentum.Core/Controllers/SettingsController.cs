
using MetaMomentum.Config;


#if NET5_0_OR_GREATER
using Umbraco.Cms.Web.Common.Controllers;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Attributes;
#else
using Umbraco.Web.WebApi;
using Umbraco.Web.Mvc;
using System.Web.Http;
#endif


namespace MetaMomentum.Controllers {

	[PluginController("MetaMomentum")]
	public class SettingsController: UmbracoApiController {

		private readonly MetaMomentumConfig _metaMomentumConfig;

		public SettingsController(MetaMomentumConfig metaMomentumConfig) {
			_metaMomentumConfig = metaMomentumConfig;
		}

		[HttpGet]
		public string FallbackImage() {
			return _metaMomentumConfig.FallbackImageUrl;
		}
	}
}
