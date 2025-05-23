using LeonoraInventory.Services;  // o LeonoraInventary.Services
using LeonoraInventory.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace LeonoraInventory  // o LeonoraInventary
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            using var db = new InventarioContext();
            db.Database.Migrate();

            // SEED de categorías solo si la tabla está vacía
            if (!db.Categorias.Any())
            {
                db.Categorias.AddRange(
                    new Categoria { Nombre = "Aparatología / Equipos", Descripcion = "Equipos y aparatología estética." },
                    new Categoria { Nombre = "Consumibles / Descartables", Descripcion = "Material descartable, guantes, mascarillas, etc." },
                    new Categoria { Nombre = "Cremas y Geles", Descripcion = "Cremas, geles hidratantes, leches limpiadoras." },
                    new Categoria { Nombre = "Sueros y Enzimas", Descripcion = "Sueros, enzimas y productos inyectables para tratamientos." },
                    new Categoria { Nombre = "Ácido hialurónico", Descripcion = "Ácido hialurónico en sus distintas presentaciones." },
                    new Categoria { Nombre = "Otros productos cosméticos", Descripcion = "Otros productos cosméticos y de apoyo." }
                );
                db.SaveChanges();
            }

            // Ahora sí levantamos la ventana principal
            var main = new Views.MainWindow();
            main.Show();
        }
    }
}