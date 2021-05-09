using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AnalogIfranView.CustomProperties
{
    public static class CustomPropExample
    {
        
        public static readonly DependencyProperty PresenterProperty = DependencyProperty.RegisterAttached(
            "Presenter",
             typeof(InkPresenter),
             typeof(CustomPropExample),
             new PropertyMetadata(null)
            );

        public static void SetPresenter(DependencyObject obj, InkPresenter presenter) {
            obj.SetValue(PresenterProperty, presenter);
        }
        public static InkPresenter GetPresenter(DependencyObject obj) {
            return (InkPresenter)obj.GetValue(PresenterProperty);
        }

        public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached(
            "Enabled",
             typeof(bool),
             typeof(CustomPropExample),
             new PropertyMetadata(null, OnChangedEnabled));
        private static void OnChangedEnabled(object sender, DependencyPropertyChangedEventArgs dp) {
            if (sender is InkCanvas ink) {
                SetPresenter(ink, ink.InkPresenter);
               
            }
        }
        public static void SetEnabled(DependencyObject obj,bool val) {
            obj.SetValue(EnabledProperty, val);
        }
        public static bool GetEnabled(DependencyObject obj) {
            return (bool)obj.GetValue(PresenterProperty);
        }



    }
}
