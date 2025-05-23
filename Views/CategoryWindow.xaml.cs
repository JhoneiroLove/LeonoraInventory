using LeonoraInventory.Models;
using LeonoraInventory.Services;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace LeonoraInventory.Views
{
    public partial class CategoryWindow : Window
    {
        private readonly Categoria? _categoriaOriginal;

        // Constructor para agregar nueva categoría
        public CategoryWindow()
        {
            InitializeComponent();
            _categoriaOriginal = null;
            this.Title = "Agregar Categoría";
        }

        // Constructor para editar categoría existente
        public CategoryWindow(Categoria categoria) : this()
        {
            _categoriaOriginal = categoria;
            this.Title = "Editar Categoría";

            NameBox.Text = categoria.Nombre;
            DescBox.Text = categoria.Descripcion;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Validar campos
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Por favor, ingrese un nombre para la categoría.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var db = new InventarioContext();

            if (_categoriaOriginal == null)
            {
                // Crear nueva categoría
                var nuevaCategoria = new Categoria
                {
                    Nombre = NameBox.Text.Trim(),
                    Descripcion = DescBox.Text.Trim()
                };
                db.Categorias.Add(nuevaCategoria);
            }
            else
            {
                // Actualizar categoría existente
                var categoriaDb = db.Categorias.Find(_categoriaOriginal.CategoriaId);
                if (categoriaDb != null)
                {
                    categoriaDb.Nombre = NameBox.Text.Trim();
                    categoriaDb.Descripcion = DescBox.Text.Trim();
                    db.Categorias.Update(categoriaDb);
                }
            }

            db.SaveChanges();
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}