using LeonoraInventory.Models;
using LeonoraInventory.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LeonoraInventory.Views
{
    public partial class MovementWindow : Window
    {
        public MovementWindow()
        {
            InitializeComponent();
            CargarProductos();
            DatePicker.SelectedDate = DateTime.Now;
        }

        private void CargarProductos()
        {
            using var db = new InventarioContext();
            ProductBox.ItemsSource = db.Productos.ToList();
            ProductBox.DisplayMemberPath = "Nombre";
            ProductBox.SelectedValuePath = "ProductoId";
            if (ProductBox.Items.Count > 0)
                ProductBox.SelectedIndex = 0;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (ProductBox.SelectedItem == null ||
                !int.TryParse(QtyBox.Text, out var qty) ||
                TypeBox.SelectedItem == null ||
                string.IsNullOrEmpty(ReasonBox.Text) ||
                string.IsNullOrEmpty(UserBox.Text))
            {
                MessageBox.Show("Todos los campos son obligatorios y la cantidad debe ser un número válido.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // <-- ESTE CAMBIO: obtiene el valor real del tipo
            string tipoMovimiento = ((ComboBoxItem)TypeBox.SelectedItem).Tag.ToString();

            using var db = new InventarioContext();
            var mov = new Movimiento
            {
                ProductoId = (int)ProductBox.SelectedValue,
                Fecha = DatePicker.SelectedDate ?? DateTime.Now,
                Cantidad = qty,
                Tipo = tipoMovimiento, // Guarda "In" o "Out"
                Motivo = ReasonBox.Text.Trim(),
                Usuario = UserBox.Text.Trim()
            };
            db.Movimientos.Add(mov);

            // Actualizar stock en producto
            var prod = db.Productos.Find(mov.ProductoId);
            if (mov.Tipo == "In") prod.Cantidad += mov.Cantidad;
            else if (mov.Tipo == "Out") prod.Cantidad -= mov.Cantidad;

            db.SaveChanges();

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}