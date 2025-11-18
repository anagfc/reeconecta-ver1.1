using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using reeconecta.Models;

namespace reeconecta.Controllers
{
    public class PontosController : Controller
    {
        private readonly AppDbContext _context;
        public PontosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var dados = await _context.Pontos.ToListAsync();

            return View(dados);
        }

        //Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ponto ponto, IFormFile ImagemFile)
        {
            if (ModelState.IsValid)
            {
                if (ImagemFile != null && ImagemFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImagemFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/pontos", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await ImagemFile.CopyToAsync(stream);

                    ponto.Imagem = "/images/pontos/" + fileName;
                }

                _context.Pontos.Add(ponto);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(ponto);
        }

        //Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var dados = await _context.Pontos.FindAsync(id);

            if (dados == null)
            {
                return NotFound();
            }
            return View(dados);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ponto ponto, IFormFile? ImagemFile)
        {
            if (id != ponto.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (ImagemFile != null && ImagemFile.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImagemFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/pontos", fileName);
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                            await ImagemFile.CopyToAsync(stream);

                        ponto.Imagem = "/images/pontos/" + fileName;
                    }

                    _context.Pontos.Update(ponto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PontoExists(ponto.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Index");
            }
            return View(ponto);
        }

        //Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var dados = await _context.Pontos.FindAsync(id);
            if (dados == null)
            {
                return NotFound();
            }
            return View(dados);
        }

        //Delete    
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var dados = await _context.Pontos.FindAsync(id);

            if (dados == null)
                return NotFound();

            return View(dados);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
                return NotFound();

            var dados = await _context.Pontos.FindAsync(id);

            if (dados == null)
                return NotFound();

            _context.Pontos.Remove(dados);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool PontoExists(int id)
        {
            return _context.Pontos.Any(e => e.Id == id);
        }
    }
}