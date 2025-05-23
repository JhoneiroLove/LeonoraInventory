using CommunityToolkit.Mvvm.Input; // para RelayCommand
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace LeonoraInventory.ViewModels
{
    public class ProductoViewModel : INotifyPropertyChanged
    {
        private decimal precioBoleta;
        public decimal PrecioBoleta
        {
            get => precioBoleta;
            set
            {
                if (precioBoleta != value)
                {
                    precioBoleta = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PrecioCosto));
                    OnPropertyChanged(nameof(PrecioPublico));
                }
            }
        }

        private double porcentaje;
        public double Porcentaje
        {
            get => porcentaje;
            set
            {
                if (porcentaje != value)
                {
                    porcentaje = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PrecioCosto));
                    OnPropertyChanged(nameof(PrecioPublico));
                }
            }
        }

        // Propiedades calculadas
        public decimal PrecioCosto
            => PrecioBoleta * 1.02m;
        public decimal PrecioPublico
            => PrecioCosto * (1 + (decimal)(Porcentaje / 100.0));

        // Comando Guardar
        public ICommand GuardarCommand { get; }

        public ProductoViewModel()
        {
            // Inicializa valores por defecto si lo deseas
            PrecioBoleta = 0m;
            Porcentaje = 0.0;

            // RelayCommand viene de CommunityToolkit.Mvvm.Input
            GuardarCommand = new RelayCommand(Guardar);
        }

        private void Guardar()
        {
            // Aquí tu lógica de guardado:
            // Por ejemplo, crear un Producto, poblar sus campos y guardarlo vía tu DbContext.
            // var producto = new Producto { PrecioBoleta = PrecioBoleta, Porcentaje = Porcentaje, ... };
            // using var db = new InventarioContext(); db.Productos.Add(producto); db.SaveChanges();
            MessageBox.Show($"Guardado: Costo={PrecioCosto:C}, Público={PrecioPublico:C}");
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}