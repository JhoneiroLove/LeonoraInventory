using CommunityToolkit.Mvvm.Input;
using LeonoraInventory.Models;
using LeonoraInventory.Services;
using LeonoraInventory.Views;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace LeonoraInventory.ViewModels
{
    public class ProductItem
    {
        private readonly Producto _p;
        private readonly InventoryViewModel _parent;

        public ProductItem(Producto p, InventoryViewModel parent)
        {
            _p = p;
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            EditCommand = new RelayCommand(Edit);
            DeleteCommand = new RelayCommand(Delete);
        }

        public int ProductoId => _p.ProductoId;
        public string Nombre => _p.Nombre;
        public string SKU => $"INV{_p.ProductoId:D3}";
        public string Categoria => _p.Categoria.Nombre;
        public int Cantidad => _p.Cantidad;
        public string PrecioBoleta => $"S/ {_p.PrecioBoleta:0.00}";
        public string PrecioCosto => $"S/ {_p.PrecioCosto:0.00}";
        public string PrecioPublico => $"S/ {_p.PrecioPublico:0.00}";
        public string Porcentaje => $"{_p.Porcentaje:0.##}%";
        public string Proveedor => _p.Proveedor;
        public string Ubicacion => _p.Ubicacion;

        public string EstadoText
            => Cantidad == 0
               ? "Agotado"
               : (Cantidad <= _p.StockMinimo ? "Stock Bajo" : "En Stock");

        public Brush EstadoBrush
            => Cantidad == 0
               ? new SolidColorBrush(Colors.IndianRed)
               : (Cantidad <= _p.StockMinimo
                  ? new SolidColorBrush(Colors.Goldenrod)
                  : new SolidColorBrush(Colors.MediumSeaGreen));

        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        private void Edit()
        {
            // Abrir ventana edición
            var window = new EditProductWindow(_p);
            bool? result = window.ShowDialog();
            if (result == true)
            {
                // Refrescar vista en parent
                _parent.LoadProducts();
            }
        }

        private void Delete()
        {
            var confirm = MessageBox.Show(
                $"¿Seguro que quieres eliminar el producto \"{Nombre}\"?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm == MessageBoxResult.Yes)
            {
                try
                {
                    using var db = new InventarioContext();
                    var entity = db.Productos.Find(ProductoId);
                    if (entity != null)
                    {
                        db.Productos.Remove(entity);
                        db.SaveChanges();
                        MessageBox.Show("Producto eliminado correctamente.", "Eliminado", MessageBoxButton.OK, MessageBoxImage.Information);
                        _parent.LoadProducts();
                    }
                    else
                    {
                        MessageBox.Show("Producto no encontrado en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar producto: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}