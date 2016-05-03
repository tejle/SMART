
using System.Windows;
using System.Windows.Data;

namespace SMART.Gui
{
    using System.Globalization;
    using System.Windows.Markup;

    using IOC;
    using View;
    using System;
    using System.Diagnostics;
    using System.Windows.Controls;

    public partial class App
    {
        public App()
        {
            //PresentationTraceSources.Refresh();
            //PresentationTraceSources.TraceLevelProperty.OverrideMetadata(
            //    typeof (ContentPresenter),
            //    new PropertyMetadata(
            //        PresentationTraceLevel.High));

            //PresentationTraceSources.DataBindingSource.Listeners.Add(new DefaultTraceListener());
            //PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.All;

            FrameworkElement.LanguageProperty.OverrideMetadata(
                  typeof(FrameworkElement),
                  new FrameworkPropertyMetadata(
                  XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            //Resolver.Configure();
            BootStrapper.Configure(Resolver.Container);
            //Resolver.RegisterSingleton(typeof(ISelectionService), typeof(SelectionService));
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var msg = e.ExceptionObject == null ? "(no error message is available)" : e.ExceptionObject.ToString();

            Clipboard.SetText(msg);
            MessageBox.Show(
                "SMART threw an unhandled exception. The following exception details have been copied to the Windows clipboard.\n\n" + msg,
                "Unexpected Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow = Resolver.Resolve<MainView>(); //Resolver.Resolve<ApplicationView>();
            MainWindow.Show();
        }

    }
}
