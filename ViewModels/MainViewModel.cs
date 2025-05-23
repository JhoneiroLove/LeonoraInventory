using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeonoraInventory.Models;
using LeonoraInventory.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace LeonoraInventory.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public ObservableCollection<MenuItem> MenuItems { get; }
            = new ObservableCollection<MenuItem>
        {
            new MenuItem{ Title="Dashboard", Icon="📊" },
            new MenuItem{ Title="Inventario", Icon="📦" },
            new MenuItem{ Title="Categorias", Icon="🏷️" },
            new MenuItem{ Title="Movimientos", Icon="🔄" },
            //new MenuItem{ Title="Configuracion", Icon="⚙️" },
        };

        [ObservableProperty] private MenuItem selectedMenuItem;
        [ObservableProperty] private object currentView;

        public ICommand ToggleSidebarCommand { get; }
        public ICommand LogoutCommand { get; }

        public MainViewModel()
        {
            SelectedMenuItem = MenuItems[0];
            CurrentView = new DashboardViewModel();

            ToggleSidebarCommand = new RelayCommand(() => { /* alternar sidebar si quieres */ });
            LogoutCommand = new RelayCommand(() => { /* logout */ });

            PropertyChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged(object s, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedMenuItem))
            {
                switch (SelectedMenuItem.Title)
                {
                    case "Dashboard":
                        CurrentView = new DashboardViewModel();
                        break;
                    case "Inventario":
                        CurrentView = new InventoryViewModel();
                        break;
                    case "Categorias":
                        CurrentView = new CategoryViewModel();
                        break;
                    case "Movimientos":
                        CurrentView = new MovementViewModel();
                        break;
                        // case "Categories":   CurrentView = new CategoriesViewModel();   break;
                        // case "Movements":    CurrentView = new MovementsViewModel();    break;
                        // case "Settings":     CurrentView = new SettingsViewModel();     break;
                }
            }
        }
    }
}