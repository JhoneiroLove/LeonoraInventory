public class RecentMovementViewModel
{
    public string Producto { get; set; }
    public int Cantidad { get; set; }
    public DateTime Fecha { get; set; }

    public RecentMovementViewModel() { }

    public RecentMovementViewModel(string producto, int cantidad, DateTime fecha)
    {
        Producto = producto;
        Cantidad = cantidad;
        Fecha = fecha;
    }
}