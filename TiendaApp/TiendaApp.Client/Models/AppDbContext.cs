using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TiendaApp.Client.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Producto> Productos { get; set; }
    }
}