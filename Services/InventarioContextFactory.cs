using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LeonoraInventory.Services
{
    public class InventarioContextFactory : IDesignTimeDbContextFactory<InventarioContext>
    {
        public InventarioContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<InventarioContext>()
                .UseSqlite("Data Source=Inventario.db")
                .Options;

            return new InventarioContext(options);
        }
    }
}