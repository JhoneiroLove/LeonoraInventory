using LeonoraInventory.Models;
using LeonoraInventory.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;

namespace LeonoraInventory.Views
{
    public partial class AddProductWindow : Window
    {
        public AddProductWindow()
        {
            InitializeComponent();
            LoadCategories();
        }

        private void LoadCategories()
        {
            using var db = new InventarioContext();
            var cats = db.Categorias.AsNoTracking().ToList();
            CategoryBox.ItemsSource = cats;
            if (cats.Any()) CategoryBox.SelectedIndex = 0;
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
             || string.IsNullOrWhiteSpace(LocationBox.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos correctamente.",
                                "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var db = new InventarioContext();
            var prod = new Producto
            {
                Nombre = NameBox.Text.Trim(),
                CategoriaId = (int)CategoryBox.SelectedValue,
                Cantidad = qty,
                StockMinimo = minStk,
                PrecioBoleta = priceBoleta,
                Porcentaje = pct,
                Ubicacion = LocationBox.Text.Trim(),
                Proveedor = SupplierBox.Text.Trim()
            };
            db.Productos.Add(prod);
            db.SaveChanges();
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}