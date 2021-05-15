using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace AnalogIfranView.Views
{
    [MarkupExtensionReturnType(ReturnType = typeof(Type))]
    public class DisplayManager : MarkupExtension
    {
        private readonly Dictionary<string, Type> allPages;
        public DisplayManager()
        {
            var currentAssembly = this.GetType().GetTypeInfo().Assembly;
            var pageTypeInfo = typeof(Page).GetTypeInfo();
            allPages = new Dictionary<string, Type>();
            var keyValues = currentAssembly.DefinedTypes
                .Where(t => pageTypeInfo.IsAssignableFrom(t))
                .Select(t => new KeyValuePair<string, Type>(t.Name, t));
            allPages = new Dictionary<string, Type>(keyValues);
        }
        public string PathToPage
        {
            set; get;
        }

        private Type GetTypeByPathToPage(string path)
        {
            Type result;
            try
            {
                result = allPages[path];
            }
            catch(KeyNotFoundException)
            {
                result = typeof(MainPage);
            }
            return result;
        }

        protected override object ProvideValue()
        {
            return GetTypeByPathToPage(PathToPage);
        }
    }
}
