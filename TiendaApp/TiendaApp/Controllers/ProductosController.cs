using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaApp.Client.Models;

namespace TiendaApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            // Filtramos para devolver solo los que tienen Activo = true
            return await _context.Productos
                .Where(p => p.Activo)
                .ToListAsync();
        }

        // POST: api/Productos
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            // Al crear, nos aseguramos que nazca con la fecha actual y activo
            producto.CreatedAt = DateTime.Now;
            producto.Activo = true;

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProductos), new { id = producto.Id }, producto);
        }

        // DELETE: api/Productos/5
        //  Implementar el "Soft Delete" (Regla Técnica)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            // REGLA TÉCNICA: Cambio de estado en lugar de eliminación física
            producto.Activo = false; // Soft Delete
            producto.UpdatedAt = DateTime.Now; // Registramos el momento del cambio

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}