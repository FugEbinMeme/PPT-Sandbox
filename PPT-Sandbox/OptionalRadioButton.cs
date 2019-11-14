using System.Windows;
using System.Windows.Controls;

namespace Sandbox {
    public class OptionalRadioButton: RadioButton {
        public static DependencyProperty IsOptionalProperty =
            DependencyProperty.Register(
                "IsOptional",
                typeof(bool),
                typeof(OptionalRadioButton),
                new PropertyMetadata((bool)true,
                    (obj, args) => {
                        ((OptionalRadioButton)obj).OnIsOptionalChanged(args);
                    }));

        public bool IsOptional {
            get => (bool)GetValue(IsOptionalProperty);
            set => SetValue(IsOptionalProperty, value);
        }

        private void OnIsOptionalChanged(DependencyPropertyChangedEventArgs args) {
            // TODO: Add event handler if needed
        }

        protected override void OnClick() {
            bool? wasChecked = IsChecked;
            base.OnClick();
            if (IsOptional && wasChecked == true)
                IsChecked = false;
        }

        public OptionalRadioButton() =>
            Style = (Style)Application.Current.FindResource(typeof(RadioButton));
    }
}
