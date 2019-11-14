using System.Globalization;
using System.Threading;
using System.Windows;

namespace Sandbox {
    public partial class App {
#if DEBUG
        void OverrideLocale(string locale) =>
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo(locale);
#endif

        void Main(object sender, StartupEventArgs e) {
#if DEBUG
            OverrideLocale("en-US");
#endif
        }
    }
}
