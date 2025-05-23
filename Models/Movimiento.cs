using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeonoraInventory.Models
{
    public class Movimiento
    {
        public int MovimientoId { get; set; }
        public DateTime Fecha { get; set; }
        public int Cantidad { get; set; }
        public string Tipo { get; set; } = null!;   // "In" o "Out"
        public string Motivo { get; set; } = null!;
        public string Usuario { get; set; } = null!;

        // FK a Producto
        public int ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;
    }
}
