#if NET5_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaMomentum.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;

namespace MetaMomentum {
	public static class MetaMomentumBuilderExtensions {

		//A BIG Thanks to Kevin Jump, Your uSync Settings saved me time working the config out! https://github.com/KevinJump/uSync/blob/v9/main/uSync.BackOffice/uSyncBackOfficeBuilderExtensions.cs

		public static IUmbracoBuilder AddMetaMomentum(this IUmbracoBuilder builder, Action<MetaMomentumConfig> defaultOptions = default) {
			// if the uSyncConfig Service is registred then we assume this has been added before so we don't do it again. 
			if (builder.Services.FirstOrDefault(x => x.ServiceType == typeof(MetaMomentumConfig)) != null) {
				return builder;
			}

			var options = builder.Services.AddSingleton<MetaMomentumConfig>(r => {
				
				var ret =  builder.Config.GetSection("MetaMomentum").Get<MetaMomentumConfig>();
				if(ret == null) {
					ret = new MetaMomentumConfig();
				}
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
