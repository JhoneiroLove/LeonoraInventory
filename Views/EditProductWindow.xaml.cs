using LeonoraInventory.Models;
using LeonoraInventory.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;

namespace LeonoraInventory.Views
{
    public partial class EditProductWindow : Window
    {
        private readonly Producto _producto;

        public EditProductWindow(Producto producto)
        {
            InitializeComponent();
            _producto = producto ?? throw new ArgumentNullException(nameof(producto));
            LoadCategories();
            LoadProductData();
        }

        private void LoadCategories()
        {
            using var db = new InventarioContext();
            var cats = db.Categorias.AsNoTracking().ToList();
            CategoryBox.ItemsSource = cats;
            if (cats.Any())
                CategoryBox.SelectedValue = _producto.CategoriaId;
            else
                CategoryBox.SelectedIndex = 0;
        }

        private void LoadProductData()
        {
            NameBox.Text = _producto.Nombre;
            QtyBox.Text = _producto.Cantidad.ToString();
            MinStockBox.Text = _producto.StockMinimo.ToString();
            PriceBox.Text = _producto.PrecioBoleta.ToString("0.00");
            PercentBox.Text = _producto.Porcentaje.ToString("0.##");
            SupplierBox.Text = _producto.Proveedor ?? "";
            LocationBox.Text = _producto.Ubicacion;

            CalcularPrecios();
        }

        private void PriceBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CalcularPrecios();
        }

        private void PercentBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CalcularPrecios();
        }

        private void CalcularPrecios()
        {
            if (decimal.TryParse(PriceBox.Text, out var priceBoleta) && double.TryParse(PercentBox.Text, out var pct))
            {
                var precioCosto = priceBoleta * 1.02m;
                var precioPublico = precioCosto * (1 + (decimal)(pct / 100.0));
                CostoBox.Text = precioCosto.ToString("0.00");
                PublicoBox.Text = precioPublico.ToString("0.00");
            }
            else
            {
                CostoBox.Text = "";
                PublicoBox.Text = "";
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text)
             || CategoryBox.SelectedValue == null
             || !int.TryParse(QtyBox.Text, out var qty)
             || !int.TryParse(MinStockBox.Text, out var minStk)
             || !decimal.TryParse(PriceBox.Text, out var priceBoleta)
             || !double.TryParse(PercentBox.Text, out var pct)
             || string.IsNullOrWhiteSpace(SupplierBox.Text)
             || string.IsNullOrWhiteSpace(LocationBox.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos correctamente.",
                                "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var db = new InventarioContext();
            var prod = db.Productos.Find(_producto.ProductoId);
            if (prod == null)
            {
                MessageBox.Show("Producto no encontrado en base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            prod.Nombre = NameBox.Text.Trim();
            prod.CategoriaId = (int)CategoryBox.SelectedValue;
            prod.Cantidad = qty;
            prod.StockMinimo = minStk;
            prod.PrecioBoleta = priceBoleta;
            prod.Porcentaje = pct;
            prod.Proveedor = SupplierBox.Text.Trim();
            prod.Ubicacion = LocationBox.Text.Trim();

            try
            {
                db.SaveChanges();
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar cambios: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}