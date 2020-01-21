﻿using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using MyDotNetCoreWpfApp.Core.Contracts.Services;
using MyDotNetCoreWpfApp.Core.Services;
using MyDotNetCoreWpfAppPrism.Contracts.Services;
using MyDotNetCoreWpfAppPrism.Models;
using MyDotNetCoreWpfAppPrism.Services;
using MyDotNetCoreWpfAppPrism.Views;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;

namespace MyDotNetCoreWpfAppPrism
{
    public partial class App : PrismApplication
    {
        private string[] _startUpArgs;
        private IPrismDeeplinkDataService _args;

        public App()
        {
        }

        protected override Window CreateShell()
            => Container.Resolve<ShellPage>();

        public override void Initialize()
        {
            base.Initialize();
            var persistAndRestoreService = Container.Resolve<IPersistAndRestoreService>();
            persistAndRestoreService.RestoreData();

            var themeSelectorService = Container.Resolve<IThemeSelectorService>();
            themeSelectorService.SetTheme();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _startUpArgs = e.Args;
            _args = new PrismDeeplinkDataService(_startUpArgs);
            base.OnStartup(e);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Core Services
            containerRegistry.Register<IFilesService, FilesService>();
            containerRegistry.Register<IPrismDeeplinkDataService, PrismDeeplinkDataService>();
            containerRegistry.RegisterInstance<IPrismDeeplinkDataService>(_args);

            // App Services
            containerRegistry.Register<IThemeSelectorService, ThemeSelectorService>();
            containerRegistry.Register<IPersistAndRestoreService, PersistAndRestoreService>();

            // Views
            containerRegistry.RegisterForNavigation<ShellPage>();
            containerRegistry.RegisterForNavigation<MainPage>();
            containerRegistry.RegisterForNavigation<BlankPage>();
            containerRegistry.RegisterForNavigation<SettingsPage>();

            var configuration = BuildConfiguration();

            var appConfig = configuration
                .GetSection(nameof(AppConfig))
                .Get<AppConfig>();

            // Register configurations to IoC
            containerRegistry.RegisterInstance<IConfiguration>(configuration);
            containerRegistry.RegisterInstance<AppConfig>(appConfig);
        }

        private IConfiguration BuildConfiguration()
        {
            var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            return new ConfigurationBuilder()
                .SetBasePath(appLocation)
                .AddJsonFile("appsettings.json")
                .AddCommandLine(_startUpArgs)
                .Build();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewName = viewType.FullName;
                var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
                var viewModelName = $"{viewName[0..^4]}ViewModel, {viewAssemblyName}";
                return Type.GetType(viewModelName);
            });
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            var persistAndRestoreService = Container.Resolve<IPersistAndRestoreService>();
            persistAndRestoreService.PersistData();
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // TODO WTS: Please handle the exception as appropriate to your scenario
            // For more info see https://docs.microsoft.com/dotnet/api/system.windows.application.dispatcherunhandledexception?view=netcore-3.0

            // e.Handled = true;
        }
    }
}