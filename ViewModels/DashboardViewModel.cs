using CommunityToolkit.Mvvm.ComponentModel;
using LeonoraInventory.Models;
using LeonoraInventory.Services;
using Microsoft.EntityFrameworkCore;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace LeonoraInventory.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        // Estadísticas generales
        [ObservableProperty] private int totalProducts;
        [ObservableProperty] private int lowStockItems;
        [ObservableProperty] private decimal inventoryValue;
        [ObservableProperty] private int outOfStockItems;

        // Productos con stock bajo
        [ObservableProperty]
        private ObservableCollection<LowStockItemViewModel> lowStockProducts = new();

        // Movimientos recientes
        [ObservableProperty]
        private ObservableCollection<RecentMovementViewModel> recentMovements = new();

        // Movimiento neto diario para gráfica
        private PlotModel _movementPlotModel;
        public PlotModel MovementPlotModel
        {
            get => _movementPlotModel;
            private set => SetProperty(ref _movementPlotModel, value);
        }

        // Indicadores dinámicos
        [ObservableProperty] private int movimientosHoy;
        [ObservableProperty] private int movimientosSemana;
        [ObservableProperty] private int movimientosMes;
        [ObservableProperty] private int movimientosAno;

        public DashboardViewModel()
        {
            BuildMovementPlot();
            LoadStats();
        }

        private void BuildMovementPlot()
        {
            var model = new PlotModel { Title = "Movimiento neto (últimos 30 días)" };

            model.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "dd MMM",
                IntervalType = DateTimeIntervalType.Days,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot
            });

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Movimiento neto",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot
            });

            var series = new LineSeries { Title = "Neto", MarkerType = MarkerType.Circle };
            model.Series.Add(series);

            MovementPlotModel = model;
        }

        public void LoadStats()
        {
            using var db = new InventarioContext();

            // Estadísticas generales
            TotalProducts = db.Productos.Count();
            LowStockItems = db.Productos.Count(p => p.Cantidad <= p.StockMinimo && p.Cantidad > 0);
            OutOfStockItems = db.Productos.Count(p => p.Cantidad == 0);

            var lista = db.Productos
                          .Where(p => p.Cantidad <= p.StockMinimo && p.Cantidad > 0)
                          .Select(p => new LowStockItemViewModel(p.Nombre, p.Cantidad, p.StockMinimo))
                          .ToList();
            LowStockProducts = new ObservableCollection<LowStockItemViewModel>(lista);

            var movimientosRecientes = db.Movimientos
                                .Include(m => m.Producto)
                                .OrderByDescending(m => m.Fecha)
                                .Take(5)
                                .Select(m => new RecentMovementViewModel(m.Producto.Nombre, m.Cantidad, m.Fecha))
                                .ToList();
            RecentMovements = new ObservableCollection<RecentMovementViewModel>(movimientosRecientes);

            var todos = db.Productos.AsNoTracking().ToList();
            InventoryValue = todos.Sum(p => p.PrecioPublico * p.Cantidad);

            // Traer todos los movimientos para cálculos
            var movimientos = db.Movimientos.AsNoTracking().ToList();

            // Fechas para indicadores
            var hoy = DateTime.Today;
            var inicioSemana = hoy.AddDays(-(int)hoy.DayOfWeek);
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
            var inicioAno = new DateTime(hoy.Year, 1, 1);

            // Función para calcular neto
            int CalcularNeto(DateTime inicio, DateTime fin)
            {
                var movs = movimientos.Where(m => m.Fecha.Date >= inicio && m.Fecha.Date <= fin);
                int ingreso = movs.Where(m => m.Tipo == "In").Sum(m => m.Cantidad);
                int salida = movs.Where(m => m.Tipo == "Out").Sum(m => m.Cantidad);
                return ingreso - salida;
            }

            MovimientosHoy = CalcularNeto(hoy, hoy);
            MovimientosSemana = CalcularNeto(inicioSemana, hoy);
            MovimientosMes = CalcularNeto(inicioMes, hoy);
            MovimientosAno = CalcularNeto(inicioAno, hoy);

            // Movimiento neto diario para los últimos 30 días
            var startDate = hoy.AddDays(-29);
            var line = (LineSeries)MovementPlotModel.Series[0];
            line.Points.Clear();

            for (int i = 0; i < 30; i++)
            {
                var day = startDate.AddDays(i);
                int ingreso = movimientos.Where(m => m.Fecha.Date == day && m.Tipo == "In").Sum(m => m.Cantidad);
                int salida = movimientos.Where(m => m.Fecha.Date == day && m.Tipo == "Out").Sum(m => m.Cantidad);
                int netoDia = ingreso - salida;
                line.Points.Add(DateTimeAxis.CreateDataPoint(day, netoDia));
            }

            MovementPlotModel.InvalidatePlot(true);
        }
    }
}