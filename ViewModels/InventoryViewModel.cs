using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeonoraInventory.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace LeonoraInventory.ViewModels
{
    public partial class InventoryViewModel : ObservableObject
    {
        public ObservableCollection<ProductItem> AllProducts { get; } = new();
        public ObservableCollection<ProductItem> PagedProducts { get; } = new();

        [ObservableProperty] private string searchQuery = "";
        [ObservableProperty] private int currentPage = 1;
        [ObservableProperty] private int pageSize = 10;
        [ObservableProperty] private int totalPages;

        public ICommand NextPageCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand AddProductCommand { get; }
        public ICommand ExportCommand { get; }

        public string PaginationText => $"Página {CurrentPage} de {TotalPages}";

        public InventoryViewModel()
        {
            NextPageCommand = new RelayCommand(NextPage, () => CurrentPage < TotalPages);
            PrevPageCommand = new RelayCommand(PrevPage, () => CurrentPage > 1);
            AddProductCommand = new RelayCommand(OpenAddProduct);
            ExportCommand = new RelayCommand(ExportExcel);

            LoadProducts();
        }

        partial void OnSearchQueryChanged(string oldValue, string newValue)
        {
            CurrentPage = 1;
            UpdatePaging();
            ((RelayCommand)NextPageCommand).NotifyCanExecuteChanged();
            ((RelayCommand)PrevPageCommand).NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(PaginationText));
        }

        partial void OnCurrentPageChanged(int oldValue, int newValue)
        {
            UpdatePaging();
            OnPropertyChanged(nameof(PaginationText));
        }

        partial void OnTotalPagesChanged(int oldValue, int newValue)
        {
            OnPropertyChanged(nameof(PaginationText));
        }

        public void LoadProducts()
        {
            using var db = new InventarioContext();
            var list = db.Productos
                         .Include(p => p.Categoria)
                         .AsNoTracking()
                         .ToList()
                         .Select(p => new ProductItem(p, this)); 
            AllProducts.Clear();
            foreach (var it in list) AllProducts.Add(it);

            UpdatePaging();
        }

        private void UpdatePaging()
        {
            var filtered = AllProducts
                .Where(p => string.IsNullOrWhiteSpace(SearchQuery)
                         || p.Nombre.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                .ToList();

            TotalPages = (int)Math.Ceiling(filtered.Count / (double)PageSize);
            if (TotalPages == 0) TotalPages = 1;
            if (CurrentPage > TotalPages) CurrentPage = TotalPages;

            PagedProducts.Clear();
            foreach (var it in filtered
                                .Skip((CurrentPage - 1) * PageSize)
                                .Take(PageSize))
                PagedProducts.Add(it);

            ((RelayCommand)NextPageCommand).NotifyCanExecuteChanged();
            ((RelayCommand)PrevPageCommand).NotifyCanExecuteChanged();
        }

        private void NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                // UpdatePaging se llama automáticamente en OnCurrentPageChanged
            }
        }

        private void PrevPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                // UpdatePaging se llama automáticamente en OnCurrentPageChanged
            }
        }

        private void OpenAddProduct()
        {
            var win = new Views.AddProductWindow();
            if (win.ShowDialog() == true)
                LoadProducts();
        }

        private void ExportExcel()
        {
            var dlg = new SaveFileDialog
            {
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = "inventory_export.xlsx"
            };
            if (dlg.ShowDialog() != true) return;

            using var wb = new ClosedXML.Excel.XLWorkbook();
            var ws = wb.Worksheets.Add("Inventario");

            // Cabecera
            ws.Cell(1, 1).Value = "Nombre";
            ws.Cell(1, 2).Value = "SKU";
            ws.Cell(1, 3).Value = "Categoria";
            ws.Cell(1, 4).Value = "Cantidad";
            ws.Cell(1, 5).Value = "Precio Público";
            ws.Cell(1, 6).Value = "Precio Costo";
            ws.Cell(1, 7).Value = "Precio Boleta";
            ws.Cell(1, 10).Value = "Proveedor";
            ws.Cell(1, 11).Value = "Porcentaje";
            ws.Cell(1, 8).Value = "Ubicacion";
            ws.Cell(1, 9).Value = "Estado";

            int row = 2;
            foreach (var p in AllProducts
                              .Where(p => string.IsNullOrWhiteSpace(SearchQuery)
                                       || p.Nombre.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)))
            {
                ws.Cell(row, 1).Value = p.Nombre;
                ws.Cell(row, 2).Value = p.SKU;
                ws.Cell(row, 3).Value = p.Categoria;
                ws.Cell(row, 4).Value = p.Cantidad;
                ws.Cell(row, 5).Value = p.PrecioPublico;
                ws.Cell(row, 6).Value = p.PrecioCosto;
                ws.Cell(row, 7).Value = p.PrecioBoleta;
                ws.Cell(row, 10).Value = p.Proveedor;
                ws.Cell(row, 11).Value = p.Porcentaje;
                ws.Cell(row, 8).Value = p.Ubicacion;
                ws.Cell(row, 9).Value = p.EstadoText;
                row++;
            }

            ws.Columns().AdjustToContents();
            wb.SaveAs(dlg.FileName);
        }

        private static string Escape(string s)
        {
            return $"\"{s.Replace("\"", "\"\"")}\"";
        }
    }
}