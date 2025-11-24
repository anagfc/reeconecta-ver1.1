using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using reeconecta.Models;

namespace reeconecta.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var produtos = await _context.Produtos
                .AsNoTracking()
                .Where(p => p.StatusProduto == StatusProduto.Disponivel && 
                           p.AnuncioAtivo &&
                           p.Usuario != null && 
                           p.Usuario.ContaAtiva)
                .Include(p => p.Usuario)
                .Include(p => p.ReservasProduto)
                .OrderByDescending(p => p.CriacaoProduto)
                .Take(6)
                .ToListAsync();

            return View(produtos);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
