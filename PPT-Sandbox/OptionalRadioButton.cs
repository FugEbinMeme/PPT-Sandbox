using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Sandbox {
    public class OptionalRadioButton: RadioButton {
        static Dictionary<string, OptionalRadioButton> groupSelects = new Dictionary<string, OptionalRadioButton>();

        public static OptionalRadioButton GetSelected(string groupName) =>
            groupSelects.ContainsKey(groupName)? groupSelects[groupName] : null;

        public static DependencyProperty IsOptionalProperty =
            DependencyProperty.Register(
                "IsOptional",
                typeof(bool),
                typeof(OptionalRadioButton),
                new PropertyMetadata(true,
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
            if (IsOptional && wasChecked == true) {
                IsChecked = false;
                groupSelects[GroupName] = null;
            }

            if (IsChecked == true)
                groupSelects[GroupName] = this;
        }

        public OptionalRadioButton() =>
            Style = (Style)Application.Current.FindResource(typeof(RadioButton));
    }
}
