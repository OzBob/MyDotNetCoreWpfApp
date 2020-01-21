using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MyDotNetCoreWpfAppPrism.Contracts.Services;
using MyDotNetCoreWpfAppPrism.Helpers;
using MyDotNetCoreWpfAppPrism.Models;
using MyDotNetCoreWpfAppPrism.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace MyDotNetCoreWpfAppPrism.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        private readonly AppConfig _config;
        private readonly IRegionManager _regionManager;
        private IRegionNavigationService _navigationService;
        private HamburgerMenuItem _selectedMenuItem;
        private HamburgerMenuItem _selectedOptionsMenuItem;
        private DelegateCommand _goBackCommand;
        private ICommand _loadedCommand;
        private ICommand _menuItemInvokedCommand;
        private ICommand _optionsMenuItemInvokedCommand;
        private string _pagekey = Strings.Resources.ShellMainPage;
        private IPrismDeeplinkDataService _deeplinkDataService;
        private bool _firstload = true;

        public HamburgerMenuItem SelectedMenuItem
        {
            get { return _selectedMenuItem; }
            set { SetProperty(ref _selectedMenuItem, value); }
        }

        public HamburgerMenuItem SelectedOptionsMenuItem
        {
            get { return _selectedOptionsMenuItem; }
            set { SetProperty(ref _selectedOptionsMenuItem, value); }
        }

        // TODO WTS: Change the icons and titles for all HamburgerMenuItems here.
        public ObservableCollection<HamburgerMenuItem> MenuItems { get; } = new ObservableCollection<HamburgerMenuItem>()
        {
            new HamburgerMenuGlyphItem() { Label = Strings.Resources.SettingsPageTitle, Glyph = "\uE8A5", Tag = typeof(MainPage).Name },
            new HamburgerMenuGlyphItem() { Label = "Blank", Glyph = "\uE8A5", Tag = typeof(BlankPage).Name }
        };

        public ObservableCollection<HamburgerMenuItem> OptionMenuItems { get; } = new ObservableCollection<HamburgerMenuItem>()
        {
            new HamburgerMenuGlyphItem() { Label = "Settings", Glyph = "\uE713", Tag = typeof(SettingsPage).Name }
        };

        public DelegateCommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new DelegateCommand(OnGoBack, CanGoBack));

        public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new DelegateCommand(OnLoaded));

        public ICommand MenuItemInvokedCommand => _menuItemInvokedCommand ?? (_menuItemInvokedCommand = new DelegateCommand(OnMenuItemInvoked));

        public ICommand OptionsMenuItemInvokedCommand => _optionsMenuItemInvokedCommand ?? (_optionsMenuItemInvokedCommand = new DelegateCommand(OnOptionsMenuItemInvoked));

        public ShellViewModel(AppConfig config, IRegionManager regionManager, IPrismDeeplinkDataService deeplinkDataService)
        {
            _config = config;
            _regionManager = regionManager;
            _deeplinkDataService = deeplinkDataService;
            if (_deeplinkDataService != null && !string.IsNullOrEmpty(_deeplinkDataService.SubPage))
            {
                switch (_deeplinkDataService.SubPage)
                {
                    case Constants.PageKeys.Main:
                        _pagekey = typeof(MainPage).Name;
                        break;
                    case Constants.PageKeys.Blank:
                        _pagekey = typeof(BlankPage).Name;
                        break;
                    default:
                        break;
                }
            }
        }

        private void OnLoaded()
        {
            _navigationService = _regionManager.Regions[_config.MainRegion].NavigationService;
            _navigationService.Navigated += OnNavigated;
            var item = MenuItems
                       .OfType<HamburgerMenuItem>()
                       .FirstOrDefault(i => _pagekey == i.Tag.ToString());
            SelectedMenuItem = item ?? MenuItems.First();
        }

        private bool CanGoBack()
            => _navigationService != null && _navigationService.Journal.CanGoBack;

        private void OnGoBack()
            => _navigationService.Journal.GoBack();

        private void OnMenuItemInvoked()
            => RequestNavigate(SelectedMenuItem.Tag.ToString());

        private void OnOptionsMenuItemInvoked()
            => RequestNavigate(SelectedOptionsMenuItem.Tag.ToString());

        private void RequestNavigate(string target)
        {
            if (_navigationService.CanNavigate(target))
            {
                if (_firstload)
                {
                    _navigationService.RequestNavigate(_deeplinkDataService.Uri);
                    _firstload = false;
                }
                else
                {
                    _navigationService.RequestNavigate(target);
                }
            }
        }

        private void OnNavigated(object sender, RegionNavigationEventArgs e)
        {
            var item = MenuItems
                        .OfType<HamburgerMenuItem>()
                        .FirstOrDefault(i => e.Uri.ToString() == i.Tag.ToString());
            if (item != null)
            {
                SelectedMenuItem = item;
            }
            else
            {
                SelectedOptionsMenuItem = OptionMenuItems
                        .OfType<HamburgerMenuItem>()
                        .FirstOrDefault(i => e.Uri.ToString() == i.Tag.ToString());
            }

            GoBackCommand.RaiseCanExecuteChanged();
        }
    }
}
