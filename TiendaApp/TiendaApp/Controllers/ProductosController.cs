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

        // 1. GET: api/productos (Soporta filtros como pide el punto 3.1)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos(string? codigo, string? nombre, bool? activo = true)
        {
            var query = _context.Productos.AsQueryable();

            if (!string.IsNullOrEmpty(codigo)) query = query.Where(p => p.Codigo.Contains(codigo));
            if (!string.IsNullOrEmpty(nombre)) query = query.Where(p => p.Nombre.Contains(nombre));
            if (activo.HasValue) query = query.Where(p => p.Activo == activo.Value);

            return await query.ToListAsync();
        }

        // 2. GET: api/productos/5 (Punto 3.1.21)
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();
            return producto;
        }

        // 3. POST: api/productos (Con reglas del punto 2: precio > 0 y stock >= 0)
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            if (await _context.Productos.AnyAsync(p => p.Codigo == producto.Codigo))
                return BadRequest("El código ya existe.");

            if (producto.Precio <= 0) return BadRequest("El precio debe ser mayor a 0.");
            if (producto.Stock < 0) return BadRequest("El stock no puede ser negativo.");

            producto.CreatedAt = DateTime.Now;
            producto.Activo = true; // Por defecto true según tabla punto 2

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, producto);
        }

        // 4. PUT: api/productos/5 (Actualizar - Punto 3.1.23)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, Producto producto)
        {
            if (id != producto.Id) return BadRequest();

            var existente = await _context.Productos.FindAsync(id);
            if (existente == null) return NotFound();

            // Actualizar campos permitidos
            existente.Nombre = producto.Nombre;
            existente.Precio = producto.Precio;
            existente.Stock = producto.Stock;
            existente.UpdatedAt = DateTime.Now; // Regla de auditoría punto 2

            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException) { throw; }

            return NoContent();
        }

        // 5. DELETE: api/productos/5 (SOFT DELETE OBLIGATORIO punto 2.12)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            producto.Activo = false; // Soft delete
            producto.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}