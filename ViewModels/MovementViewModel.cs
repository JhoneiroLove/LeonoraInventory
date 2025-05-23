using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeonoraInventory.Models;
using LeonoraInventory.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace LeonoraInventory.ViewModels
{
    public partial class MovementViewModel : ObservableObject
    {
        public ObservableCollection<MovimientoItem> Movements { get; } = new();

        [ObservableProperty]
        private string searchQuery = "";

        [ObservableProperty]
        private int stockInCount;

        [ObservableProperty]
        private int stockOutCount;

        [ObservableProperty]
        private ObservableCollection<RecentMovementViewModel> recentMovements = new();

        [ObservableProperty]
        private string selectedPeriodText = "Últimos 30 días";

        public ICommand RefreshCommand { get; }
        public ICommand AddMovementCommand { get; }
        public ICommand FilterCommand { get; }

        public MovementViewModel()
        {
            RefreshCommand = new RelayCommand(Load);
            AddMovementCommand = new RelayCommand(OpenAdd);
            FilterCommand = new RelayCommand(ApplyFilter);

            Load();
        }

        private void Load()
        {
            using var db = new InventarioContext();
            var allMoves = db.Movimientos
                             .Include(m => m.Producto)
                             .OrderByDescending(m => m.Fecha)
                             .ToList();

            Movements.Clear();
            foreach (var m in allMoves)
                Movements.Add(new MovimientoItem(m));

            StockInCount = allMoves.Count(m => m.Tipo == "In");
            StockOutCount = allMoves.Count(m => m.Tipo == "Out");
        }

        private void OpenAdd()
        {
            var win = new Views.MovementWindow();
            win.ShowDialog();
            Load();
        }

        private void ApplyFilter()
        {
            Load();

            if (!string.IsNullOrWhiteSpace(SearchQuery))  // Usa la propiedad pública, no un campo privado
            {
                var filtered = Movements
                    .Where(m => m.ProductName.Contains(SearchQuery, System.StringComparison.OrdinalIgnoreCase)
                             || m.Reason.Contains(SearchQuery, System.StringComparison.OrdinalIgnoreCase))
                    .ToList();

                Movements.Clear();
                foreach (var item in filtered)
                    Movements.Add(item);
            }
        }
    }

    public class MovimientoItem
    {
        public int Id { get; }
        public string ProductName { get; }
        public int Quantity { get; }
        public string Type { get; }
        public DateTime Date { get; }
        public string User { get; }
        public string Reason { get; }

        public MovimientoItem(Movimiento m)
        {
            Id = m.MovimientoId;
            ProductName = m.Producto?.Nombre ?? "N/A";
            Quantity = m.Cantidad;
            Type = m.Tipo == "In" ? "Entrada" : "Salida";
            Date = m.Fecha;
            User = m.Usuario ?? "N/A";
            Reason = m.Motivo ?? "";
        }
    }
}