using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeonoraInventory.Models;
using LeonoraInventory.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace LeonoraInventory.ViewModels
{
    public partial class CategoryViewModel : ObservableObject
    {
        public ObservableCollection<Categoria> Categories { get; } = new();

        public IRelayCommand RefreshCommand { get; }
        public IRelayCommand AddCategoryCommand { get; }
        public IRelayCommand<Categoria> EditCategoryCommand { get; }
        public IRelayCommand<Categoria> DeleteCategoryCommand { get; }

        public CategoryViewModel()
        {
            RefreshCommand = new RelayCommand(LoadCategories);
            AddCategoryCommand = new RelayCommand(OpenAddCategory);
            EditCategoryCommand = new RelayCommand<Categoria>(OpenEditCategory);
            DeleteCategoryCommand = new RelayCommand<Categoria>(DeleteCategory);

            LoadCategories();
        }

        private void LoadCategories()
        {
            using var db = new InventarioContext();
            var list = db.Categorias
                         .Include(c => c.Productos)
                         .AsNoTracking()
                         .OrderBy(c => c.Nombre)
                         .ToList();

            Categories.Clear();
            foreach (var c in list)
                Categories.Add(c);
        }

        private void OpenAddCategory()
        {
            var win = new Views.CategoryWindow();
            if (win.ShowDialog() == true)
                LoadCategories();
        }

        private void OpenEditCategory(Categoria categoria)
        {
            if (categoria == null) return;

            var win = new Views.CategoryWindow(categoria);
            if (win.ShowDialog() == true)
                LoadCategories();
        }

        private void DeleteCategory(Categoria categoria)
        {
            if (categoria == null) return;

            var result = MessageBox.Show(
                $"¿Está seguro que desea eliminar la categoría '{categoria.Nombre}'?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            using var db = new InventarioContext();
            var toDelete = db.Categorias.Find(categoria.CategoriaId);
            if (toDelete != null)
            {
                db.Categorias.Remove(toDelete);
                db.SaveChanges();
                LoadCategories();
            }
        }
    }
}