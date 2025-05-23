using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeonoraInventory.ViewModels
{
    public class LowStockItemViewModel
    {
        public string Nombre { get; }
        public int StockActual { get; }
        public int StockMinimo { get; }
        public double Porcentaje => StockMinimo == 0 ? 0 : (double)StockActual / StockMinimo * 100;

        public LowStockItemViewModel(string nombre, int actual, int minimo)
        {
            Nombre = nombre;
            StockActual = actual;
            StockMinimo = minimo;
        }
    }
}