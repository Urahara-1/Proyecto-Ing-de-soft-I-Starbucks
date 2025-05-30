using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ing_Soft.Data;
using Ing_Soft.Models;
using Ing_Soft.Filters;

namespace Ing_Soft.Controllers
{
    [RolAuthorize("Administrador")]
    public class OrdenCompraController : Controller
    {
        private readonly AppDbContext _context;

        public OrdenCompraController(AppDbContext context)
        {
            _context = context;
        }

        // GET: OrdenCompra
        public async Task<IActionResult> Index()
        {
            var ordenesCompra = await _context.OrdenCompra
                .Include(o => o.ID_ProveedorNavigation)
                .OrderByDescending(o => o.Fecha)
                .ToListAsync();
            return View(ordenesCompra);
        }

        // GET: OrdenCompra/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordenCompra = await _context.OrdenCompra
                .FirstOrDefaultAsync(m => m.ID_OrdenCompra == id);
            if (ordenCompra == null)
            {
                return NotFound();
            }

            return View(ordenCompra);
        }

        // GET: OrdenCompra/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OrdenCompra/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID_OrdenCompra,Fecha,ID_Proveedor,Total")] OrdenCompra ordenCompra)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ordenCompra);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ordenCompra);
        }

        // GET: OrdenCompra/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordenCompra = await _context.OrdenCompra.FindAsync(id);
            if (ordenCompra == null)
            {
                return NotFound();
            }
            return View(ordenCompra);
        }

        // POST: OrdenCompra/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID_OrdenCompra,Fecha,ID_Proveedor,Total")] OrdenCompra ordenCompra)
        {
            if (id != ordenCompra.ID_OrdenCompra)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ordenCompra);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdenCompraExists(ordenCompra.ID_OrdenCompra))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ordenCompra);
        }

        // GET: OrdenCompra/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordenCompra = await _context.OrdenCompra
                .FirstOrDefaultAsync(m => m.ID_OrdenCompra == id);
            if (ordenCompra == null)
            {
                return NotFound();
            }

            return View(ordenCompra);
        }

        // POST: OrdenCompra/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Obtener todos los detalles relacionados con la orden
            var detalles = _context.DetalleOrdenCompra.Where(d => d.ID_OrdenCompra == id);
            _context.DetalleOrdenCompra.RemoveRange(detalles); // eliminar todos los detalles

            // Obtener la orden a eliminar
            var ordenCompra = await _context.OrdenCompra.FindAsync(id);
            if (ordenCompra != null)
            {
                _context.OrdenCompra.Remove(ordenCompra);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrdenCompraExists(int id)
        {
            return _context.OrdenCompra.Any(e => e.ID_OrdenCompra == id);
        }



        // Metodo GET: para mostrar el detalle de la orden de compra completo
        public async Task<IActionResult> VerDetalleOrdenCompra(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalles = await _context.DetalleOrdenCompra
                .Include(d => d.ID_ProductoNavigation)
                .Where(d => d.ID_OrdenCompra == id)
                .ToListAsync();

            if (detalles == null || detalles.Count == 0)
            {
                return NotFound("No se encontraron productos en esta orden.");
            }

            return View(detalles); // ← devolvés directamente la lista de DetalleOrdenCompra
        }






    }
}
