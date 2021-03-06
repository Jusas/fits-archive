﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FitsArchiveLib.Entities;
using FitsArchiveLib.Interfaces;
using FitsArchiveLib.Services;
using FitsArchiveUI.ViewModels;
using FitsArchiveUI.Views;

namespace FitsArchiveUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MainWindowBase
    {
        private IFitsDatabase _database;

        public MainWindow(MainViewModel viewModel, ILogService logService) : base(viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }

        private async void RunButton_Click(object sender, RoutedEventArgs e)
        {
            //var path = PathBox.Text;
            //// Problem with drive root: get *.fits from root, then get directories under root, ignore system files
            //// if (dir.EndsWith("System Volume Information")) return true;
            //// if (dir.Contains("$RECYCLE.BIN")) return true;
            //// then enumerate files in all the dirs, combine them, and finally pass that to AddFiles().
            //var allFitsFiles = Directory.EnumerateFiles(path, "*.fits", SearchOption.AllDirectories);
            //await _database.AddFiles(allFitsFiles);

        }
    }
}
