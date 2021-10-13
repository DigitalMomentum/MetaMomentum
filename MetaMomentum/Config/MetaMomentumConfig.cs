using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NET5_0_OR_GREATER
using Microsoft.Extensions.Configuration;
#endif

namespace MetaMomentum.Config {

	public class MetaMomentumConfig {
		public string OGSiteName { get; set; } = null;
		public string TwitterName { get; set; } = null;

#if NET5_0_OR_GREATER
		public MetaMomentumConfig() {
		}
		public MetaMomentumConfig(IConfiguration configuration = null) {

			if (configuration != null) {
				var configSection = configuration.GetSection("MetaMomentum").Get<MetaMomentumConfig>();

				OGSiteName = configSection?.OGSiteName;
				TwitterName = configSection?.TwitterName;

			}
		}
#endif
	}
}
