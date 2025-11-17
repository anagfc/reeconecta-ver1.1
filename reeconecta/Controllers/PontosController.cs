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
        public async Task<IActionResult> Create(Ponto ponto)
        {   
            if(ModelState.IsValid)
            {
                _context.Pontos.Add(ponto);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }       

            return View(ponto);
        }

        //Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var dados = await _context.Pontos.FindAsync(id);

            if(dados == null)
            {
                return NotFound();
            }
            return View(dados);
        }
        [HttpPost]  
        public async Task<IActionResult> Edit(int id, Ponto ponto)
        {
            if(id != ponto.Id)
            {
                return NotFound();
            }
            if(ModelState.IsValid)
            {
                _context.Pontos.Update(ponto);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(); 
        }

        //Details
        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var dados = await _context.Pontos.FindAsync(id);
            if(dados == null)
            {
                return NotFound();
            }
            return View(dados);
        }

        //Delete    
        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
                return NotFound();

            var dados = await _context.Pontos.FindAsync(id);

            if(dados == null)
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
    }
}
