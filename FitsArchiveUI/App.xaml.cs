using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using FitsArchiveLib.Interfaces;
using FitsArchiveUI.ViewModels;
using Ninject;
using PresentationTheme.Aero;

namespace FitsArchiveUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            AeroTheme.SetAsCurrentTheme();
        }

        private void AppStartup(object sender, StartupEventArgs e)
        {
            Bootstrap.Setup();
            var mainWin = Bootstrap.Kernel.Get<MainWindow>();
            MainWindow = mainWin;
            MainWindow.Show();
        }
    }
}
