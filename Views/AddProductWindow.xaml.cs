using LeonoraInventory.Models;
using LeonoraInventory.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
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
            if (!ValidarCampos())
                return;

            using var db = new InventarioContext();
            var prod = new Producto
            {
                Nombre = NameBox.Text.Trim(),
                CategoriaId = (int)CategoryBox.SelectedValue,
                Cantidad = int.Parse(QtyBox.Text.Trim()),
                StockMinimo = int.Parse(MinStockBox.Text.Trim()),
                PrecioBoleta = decimal.Parse(PriceBox.Text.Trim(), CultureInfo.InvariantCulture),
                Porcentaje = double.Parse(PercentBox.Text.Trim(), CultureInfo.InvariantCulture),
                Ubicacion = LocationBox.Text.Trim(),
                Proveedor = SupplierBox.Text.Trim()
            };
            db.Productos.Add(prod);
            db.SaveChanges();
            DialogResult = true;
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Por favor, ingresa el nombre del producto.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                NameBox.Focus();
                return false;
            }

            if (CategoryBox.SelectedValue == null)
            {
                MessageBox.Show("Selecciona una categoría.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                CategoryBox.Focus();
                return false;
            }

            if (!int.TryParse(QtyBox.Text.Trim(), out int cantidad) || cantidad < 0)
            {
                MessageBox.Show("Ingresa una cantidad válida (número entero positivo).", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                QtyBox.Focus();
                return false;
            }

            if (!int.TryParse(MinStockBox.Text.Trim(), out int stockMin) || stockMin < 0)
            {
                MessageBox.Show("Ingresa un stock mínimo válido (número entero positivo).", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                MinStockBox.Focus();
                return false;
            }

            if (!decimal.TryParse(PriceBox.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal precioBoleta) || precioBoleta < 0)
            {
                MessageBox.Show("Ingresa un precio boleta válido (número decimal positivo).", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                PriceBox.Focus();
                return false;
            }

            if (!double.TryParse(PercentBox.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out double porcentaje) || porcentaje < 0 || porcentaje > 100)
            {
                MessageBox.Show("Ingresa un porcentaje válido entre 0 y 100.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                PercentBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(SupplierBox.Text))
            {
                MessageBox.Show("Por favor, ingresa el proveedor.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                SupplierBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(LocationBox.Text))
            {
                MessageBox.Show("Por favor, ingresa la ubicación.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                LocationBox.Focus();
                return false;
            }

            return true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}