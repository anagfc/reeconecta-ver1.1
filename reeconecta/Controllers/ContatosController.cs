using Microsoft.AspNetCore.Mvc;
using reeconecta.Models;
using System;
using System.Threading.Tasks;

namespace reeconecta.Controllers
{
    public class ContatosController : Controller
    {
        private readonly AppDbContext _context;

        public ContatosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnviarContato(Contato contato)
        {
            if (string.IsNullOrWhiteSpace(contato.Mensagem))
                ModelState.AddModelError("Mensagem", "É obrigatório informar a mensagem.");

            if (string.IsNullOrWhiteSpace(contato.Nome) && User.Identity.IsAuthenticated)
            {
                contato.Nome = User.Identity.Name;
            }

            if (string.IsNullOrWhiteSpace(contato.Email) && HttpContext.Session.GetString("Email") != null)
            {
                contato.Email = HttpContext.Session.GetString("Email");
            }

            contato.DataEnvio = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(contato);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Mensagem enviada com sucesso!";
                return RedirectToAction("Index", "Home"); 
            }

            TempData["Error"] = "Preencha todos os campos corretamente.";
            return RedirectToAction("Index", "Home");
        }
    }
}
