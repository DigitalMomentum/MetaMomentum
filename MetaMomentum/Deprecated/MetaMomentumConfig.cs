#if NET5_0_OR_GREATER
using Microsoft.Extensions.Configuration;
#else
using System.Configuration;
#endif

namespace MetaMomentum.Config {

	public class MetaMomentumConfig {

		[Obsolete("Moved to The MetaMomentum Data Type Configuration")]
		public string OGSiteName { get; set; } = null;
		[Obsolete("Moved to The MetaMomentum Data Type Configuration")]
		public string TwitterName { get; set; } = null;

		[Obsolete("Removed by Facebook. No longer used")]
		public string FacebookAppId { get; set; } = null;
		//public string CanonicalDomain { get; set; } = null;

#if NET5_0_OR_GREATER
		public MetaMomentumConfig() {
		}

		internal MetaMomentumConfig(IConfiguration configuration) {
			
		}
#else
		public MetaMomentumConfig() {
			
		}
#endif

	

	}
}
