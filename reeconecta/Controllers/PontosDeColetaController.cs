using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using reeconecta.Models;

namespace reeconecta.Controllers
{
    public class PontosDeColetaController : Controller
    {
        private readonly AppDbContext _context;

        public PontosDeColetaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: PontosDeColeta
        public async Task<IActionResult> Index()
        {
            return View(await _context.PontosDeColeta.ToListAsync());
        }

        // GET: PontosDeColeta/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pontoDeColeta = await _context.PontosDeColeta
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pontoDeColeta == null)
            {
                return NotFound();
            }

            return View(pontoDeColeta);
        }

        // GET: PontosDeColeta/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PontosDeColeta/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NomePonto,Tipo,DescricaoPonto,CepPonto,FuncionamentoPonto,HorarioPonto,TelefoneP01,WppTelP1,TelefoneP02,WppTelP2,Imagem")] PontoDeColeta pontoDeColeta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pontoDeColeta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pontoDeColeta);
        }

        // GET: PontosDeColeta/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pontoDeColeta = await _context.PontosDeColeta.FindAsync(id);
            if (pontoDeColeta == null)
            {
                return NotFound();
            }
            return View(pontoDeColeta);
        }

        // POST: PontosDeColeta/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NomePonto,Tipo,DescricaoPonto,CepPonto,FuncionamentoPonto,HorarioPonto,TelefoneP01,WppTelP1,TelefoneP02,WppTelP2,Imagem")] PontoDeColeta pontoDeColeta)
        {
            if (id != pontoDeColeta.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pontoDeColeta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PontoDeColetaExists(pontoDeColeta.Id))
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
            return View(pontoDeColeta);
        }

        // GET: PontosDeColeta/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pontoDeColeta = await _context.PontosDeColeta
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pontoDeColeta == null)
            {
                return NotFound();
            }

            return View(pontoDeColeta);
        }

        // POST: PontosDeColeta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pontoDeColeta = await _context.PontosDeColeta.FindAsync(id);
            if (pontoDeColeta != null)
            {
                _context.PontosDeColeta.Remove(pontoDeColeta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PontoDeColetaExists(int id)
        {
            return _context.PontosDeColeta.Any(e => e.Id == id);
        }
    }
}
