﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyDotNetCoreWpfApp.MVVMLight.Contracts.Services;
using MyDotNetCoreWpfApp.MVVMLight.Contracts.Views;
using MyDotNetCoreWpfApp.MVVMLight.ViewModels;

namespace MyDotNetCoreWpfApp.MVVMLight.Services
{
    public class ApplicationHostService : IApplicationHostService
    {
        private IShellWindow _shell;
        private INavigationService _navigationService;
        private IPersistAndRestoreService _persistAndRestoreService;
        private IThemeSelectorService _themeSelectorService;

        public ApplicationHostService(IShellWindow shell, INavigationService navigationService, IPersistAndRestoreService persistAndRestoreService, IThemeSelectorService themeSelectorService)
        {
            _shell = shell;
            _navigationService = navigationService;
            _persistAndRestoreService = persistAndRestoreService;
            _themeSelectorService = themeSelectorService;
        }

        public async Task StartAsync()
        {
            // Tasks before activation
            await InitializeAsync();

            _shell.ShowWindow();
            _navigationService.NavigateTo(typeof(MainViewModel).FullName);

            // Tasks after activation
            await StartupAsync();
        }

        public async Task StopAsync()
        {
            await Task.CompletedTask;
            _persistAndRestoreService.PersistData();
        }

        private async Task InitializeAsync()
        {
            var frame = _shell.GetNavigationFrame();
            _navigationService.Initialize(frame);
            _persistAndRestoreService.RestoreData();
            _themeSelectorService.SetTheme();
            await Task.CompletedTask;
        }

        private async Task StartupAsync()
        {
            await Task.CompletedTask;
        }
    }
}
