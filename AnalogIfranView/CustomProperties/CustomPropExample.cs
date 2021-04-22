using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AnalogIfranView.CustomProperties
{
    public class CustomPropExample : InkCanvas
    {

        public static readonly DependencyProperty StrokesProperty = DependencyProperty.RegisterAttached(
            "Strokes",
             typeof(InkStrokeContainer),
             typeof(InkCanvas),
             new PropertyMetadata(null, new PropertyChangedCallback(OnItemChanged)));


        public InkStrokeContainer Strokes
        {
            get { return (InkStrokeContainer)GetValue(StrokesProperty); }
            set { SetValue(StrokesProperty, value); }
        }
        private static void OnItemChanged(DependencyObject target,
            DependencyPropertyChangedEventArgs e) {
            var element = target as InkCanvas;
            if (element != null) {
                // If we're putting in a new command and there wasn't one already
                // hook the event
                element.InkPresenter.StrokeContainer = (InkStrokeContainer)e.NewValue;
            }
        }


    }
}
