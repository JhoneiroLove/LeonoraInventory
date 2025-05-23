using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeonoraInventory.Models
{
    public class Producto
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = null!;
        public int Cantidad { get; set; }
        public int StockMinimo { get; set; }
        public string Ubicacion { get; set; } = null!;
        public string Proveedor { get; set; } = string.Empty;

        // FK a Categoría
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; } = null!;

        // Nuevos campos persistentes
        public decimal PrecioBoleta { get; set; }            // Precio ingresado (equivale a precioBoleta)
        public double Porcentaje { get; set; }              // Porcentaje (%) para precio producto

        public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();

        // Propiedades calculadas (no mapeadas a BD)
        [NotMapped]
        public decimal PrecioCosto
            => PrecioBoleta * 1.02m;

        [NotMapped]
        public decimal PrecioPublico
            => PrecioCosto * (1 + (decimal)(Porcentaje / 100.0));
    }
}
