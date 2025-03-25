#if NET5_0_OR_GREATER
using System;
using System.Linq;
using MetaMomentum.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;

namespace MetaMomentum.Deprecated {
	public static class MetaMomentumBuilderExtensions {

		[Obsolete("This method is deprecated. Use The MetaMomentum DataType to configure values instead.")]
		public static IUmbracoBuilder AddMetaMomentum(this IUmbracoBuilder builder, Action<MetaMomentumConfig> defaultOptions = default) {
			
			return builder;
		}
	}



}
#endif
