#if NET5_0_OR_GREATER
using System;
using System.Linq;
using MetaMomentum.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;

namespace MetaMomentum{
	public static class MetaMomentumBuilderExtensions {

		//A BIG Thanks to Kevin Jump, Your uSync Settings saved me time working the config out! https://github.com/KevinJump/uSync/blob/v9/main/uSync.BackOffice/uSyncBackOfficeBuilderExtensions.cs

		public static IUmbracoBuilder AddMetaMomentum(this IUmbracoBuilder builder, Action<MetaMomentumConfig> defaultOptions = default) {
			// if the MetaMomentumConfig Service is registered then we assume this has been added before so we don't do it again. 
			if (builder.Services.FirstOrDefault(x => x.ServiceType == typeof(MetaMomentumConfig)) != null) {
				return builder;
			}

			var options = builder.Services.AddSingleton(r => {
				var ret = new MetaMomentumConfig(builder.Config);

				if (defaultOptions != default) {
					//Override with custom details
					defaultOptions.Invoke(ret);
				}
				return ret;
			});


			if (defaultOptions != default) {
				//options..Configure(defaultOptions);
			}

			return builder;
		}
	}
}
#endif
