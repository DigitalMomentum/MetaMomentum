using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace MetaMomentum.Compositions
{
    public class Startup : IUserComposer
    {
        public void Compose(Composition composition)
        {
            //If the type is accessible (not internal) you can deregister it by the type:
            composition.PropertyValueConverters().Remove<MyCustom.StandardValueConnector>();

            //If the type is not accessible you will need to locate the instance and then remove it:
            //var contentPickerValueConverter = composition.PropertyValueConverters().GetTypes().FirstOrDefault(x => x.Name == "MetaMomentumValueConverter");
            //if (contentPickerValueConverter != null)
            //{
            //    composition.PropertyValueConverters().Remove(contentPickerValueConverter);
            //}
        }
    }