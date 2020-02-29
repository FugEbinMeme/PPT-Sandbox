﻿using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace Sandbox {
    public partial class App {
        public static readonly int Version = Assembly.GetExecutingAssembly().GetName().Version.Minor;
        public static readonly string VersionString = $"PPT-Sandbox-{Version}";

#if DEBUG
        void OverrideLocale(string locale) =>
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo(locale);
#endif

        void Main(object sender, StartupEventArgs e) {
#if DEBUG
            OverrideLocale("en-US");
#endif

            AppDomain.CurrentDomain.UnhandledException += (s, ex) => {
                new Error(ex.ExceptionObject.ToString()).ShowDialog();
                Current.Shutdown();
            };
        }
    }
}
