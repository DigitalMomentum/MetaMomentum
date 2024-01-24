#if NET5_0_OR_GREATER
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
#else
using MetaMomentum.Config;
using Umbraco.Core;
using Umbraco.Core.Composing;
#endif

namespace MetaMomentum.Core.Composing {
#if NET5_0_OR_GREATER
	internal class MetaMomentumComposer : IComposer
#else
	[RuntimeLevel(MinLevel = RuntimeLevel.Install)]
	public class MetaMomentumComposer : IUserComposer
#endif
	{
#if NET5_0_OR_GREATER
		public void Compose(IUmbracoBuilder builder) {
			builder.AddMetaMomentum();
		}
#else
		public void Compose(Composition composition) {
			composition.Register<MetaMomentumConfig>();
		}
#endif
	}
}
