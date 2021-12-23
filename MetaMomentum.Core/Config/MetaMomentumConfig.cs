#if NET5_0_OR_GREATER
using Microsoft.Extensions.Configuration;
#else
using System.Configuration;
#endif

namespace MetaMomentum.Config {

	public class MetaMomentumConfig {
		public string OGSiteName { get; set; } = null;
		public string TwitterName { get; set; } = null;
		public string FacebookAppId { get; set; } = null;
		//public string CanonicalDomain { get; set; } = null;

#if NET5_0_OR_GREATER
		public MetaMomentumConfig() {
		}

		internal MetaMomentumConfig(IConfiguration configuration) {
			if (configuration != null) {
				var configSection = configuration.GetSection(Constants.Configuration.SectionName).Get<MetaMomentumConfig>();

				OGSiteName = configSection?.OGSiteName;
				TwitterName = configSection?.TwitterName;
				FacebookAppId = configSection?.FacebookAppId;
				//CanonicalDomain = ValidateCanonicalDomain(configSection?.CanonicalDomain);
			}
		}
#else
		public MetaMomentumConfig() {
			OGSiteName = ConfigurationManager.AppSettings[$"{Constants.Configuration.SectionName}.{Constants.Configuration.OGSiteName}"];
			TwitterName = ConfigurationManager.AppSettings[$"{Constants.Configuration.SectionName}.{Constants.Configuration.TwitterName}"];
			FacebookAppId = ConfigurationManager.AppSettings[$"{Constants.Configuration.SectionName}.{Constants.Configuration.FacebookId}"];
			//CanonicalDomain = MetaMomentumConfig.ValidateCanonicalDomain(ConfigurationManager.AppSettings[$"{Constants.Configuration.SectionName}.{Constants.Configuration.CanonicalDomain}"]);
		}
#endif

		//private static string ValidateCanonicalDomain(string canonicalDomain) {
		//	// Validate the CanonicalDomain
		//	var domain = canonicalDomain.Trim('/');
		//	if (!string.IsNullOrEmpty(domain)) {
		//		var domainRegex = @"(?!:\/\/)([a-zA-Z0-9-_]+\.)*[a-zA-Z0-9][a-zA-Z0-9-_]+\.[a-zA-Z]{2,11}?$";
		//		domain = Regex.Match(domain, domainRegex).Value;
		//	}

		//	return domain;
		//}

	}
}
