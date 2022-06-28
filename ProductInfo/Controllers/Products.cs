using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProductInfo.Models;

namespace ProductInfo
{
    public class Products : Controller
    {
        private readonly ApplicationDbContext _context;
        IWebHostEnvironment _webHostEnvironment;

        public Products(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;

            _webHostEnvironment = webHostEnvironment;

        }


        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products.Include(p => p.Customer);
            return View(await applicationDbContext.ToListAsync());
        }

        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Customer)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

       
        public IActionResult Create()
        {
            ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "ID");
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Product product)
        {
            if (ModelState.IsValid)
            {


                string uploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (product.Image.Length > 0)
                {
                    string filePath = Path.Combine(uploads, product.Image.FileName);
                    using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.Image.CopyToAsync(fileStream);
                        product.ImagePath = product.Image.FileName;
                    }

                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "ID", product.CustomerID);
            return View(product);
        }

       
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "ID", product.CustomerID);
            return View(product);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  Product product)
        {
            if (id != product.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string uploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (product.Image.Length > 0)
                    {
                        string filePath = Path.Combine(uploads, product.Image.FileName);
                        using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await product.Image.CopyToAsync(fileStream);
                            product.ImagePath = product.Image.FileName;
                        }

                    }


                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ID))
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
            ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "ID", product.CustomerID);
            return View(product);
        }

       
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Customer)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ID == id);
        }
    }
}
