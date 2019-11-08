using System;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Kamban.ViewModels;
using Kamban.Views;
using Monik.Common;
using Ui.Wpf.Common;
using Ui.Wpf.Common.ShowOptions;

namespace Kamban
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 


    public partial class App : Application
    {
        MonikFile mon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SetupExceptionHandling();

            var shell = UiStarter.Start<IDockWindow>(
                new Bootstrapper(),
                new UiShowStartWindowOptions
                {
                    Title = "Kamban"
                });


            var container = shell.Container;
            mon = (MonikFile)container.Resolve<IMonik>();
            mon.ApplicationInfo("Bootstrapper initialized");


            shell.ShowView<StartupView>(
                viewRequest: new ViewRequest { ViewId = StartupViewModel.StartupViewId },
                options: new UiShowOptions { Title = "Start Page", CanClose = false });
        }


        private void SetupExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            Dispatcher.UnhandledException += (s, e) =>
                LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");

            TaskScheduler.UnobservedTaskException += (s, e) =>
                LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
        }

        private void LogUnhandledException(Exception exception, string source)
        {
            string message = $"Unhandled exception ({source})";
            try
            {
                System.Reflection.AssemblyName assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
                message = string.Format("Unhandled exception in {0} v{1}", assemblyName.Name, assemblyName.Version);
            }
            catch (Exception ex)
            {
                mon.ApplicationError("Exception in LogUnhandledException");
                mon.ApplicationError(ex.Message);
                mon.ApplicationError(ex.Source);
                mon.ApplicationError(ex.StackTrace);
                mon.ApplicationError(ex.ToString());
            }
            finally
            {
                mon.ApplicationError(message);
                mon.ApplicationError(exception.Message);
                mon.ApplicationError(exception.Source);
                mon.ApplicationError(exception.StackTrace);
                mon.ApplicationError(exception.ToString());
            }
        }
    }
}
