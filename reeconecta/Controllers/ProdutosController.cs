using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using reeconecta.Models;

namespace reeconecta.Controllers
{
    [Authorize]
    public class ProdutosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public ProdutosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Produtos
        public async Task<IActionResult> Index()
        {
            var produtos = await _context.Produtos
                .Include(p => p.Usuario)
                .Include(p => p.ReservasProduto)
                .ToListAsync();

            return View(produtos);
        }

        // GET: Produtos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var produto = await _context.Produtos
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (produto == null) return NotFound();

            return View(produto);
        }

        // GET: Produtos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Produtos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Titulo,Preco,Descricao,Condicao,Bairro,Cidade,Imagem,StatusProduto")] Produto produto, IFormFile ImagemFile)
        {
            if (ModelState.IsValid)
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    ModelState.AddModelError("", "Usuário logado não encontrado.");
                    return View(produto);
                }

                if (!int.TryParse(userIdClaim, out int userId))
                {
                    ModelState.AddModelError("", "ID do usuário logado inválido.");
                    return View(produto);
                }

                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == userId);
                if (usuario == null)
                {
                    ModelState.AddModelError("", "Usuário não encontrado no banco de dados.");
                    return View(produto);
                }

                produto.AnuncianteId = usuario.Id;
                produto.CriacaoProduto = DateTime.Now;
                produto.AnuncioAtivo = true;
                produto.StatusProduto = StatusProduto.Disponivel;

                if (ImagemFile != null && ImagemFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImagemFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/produtos", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await ImagemFile.CopyToAsync(stream);

                    produto.Imagem = "/images/produtos/" + fileName;
                }

                _context.Add(produto);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(produto);
        }


        // GET: Produtos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null) return NotFound();

            ViewData["AnuncianteId"] = new SelectList(_context.Usuarios, "Id", "Nome", produto.AnuncianteId);
            return View(produto);
        }

        // POST: Produtos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Produto produto, IFormFile? ImagemFile)
        {
            if (id != produto.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Atualiza imagem a cada novo envio
                    if (ImagemFile != null && ImagemFile.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await ImagemFile.CopyToAsync(ms);
                            var bytes = ms.ToArray();
                            produto.Imagem = $"data:{ImagemFile.ContentType};base64,{Convert.ToBase64String(bytes)}";
                        }
                    }

                    _context.Update(produto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutoExists(produto.Id)) return NotFound();
                    else throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["AnuncianteId"] = new SelectList(_context.Usuarios, "Id", "Nome", produto.AnuncianteId);
            return View(produto);
        }

        // GET: Produtos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var produto = await _context.Produtos
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (produto == null) return NotFound();

            return View(produto);
        }

        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProdutoExists(int id)
        {
            return _context.Produtos.Any(e => e.Id == id);
        }

        // GET: Produtos/MeusProdutos
        public async Task<IActionResult> MeusProdutos()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Usuarios");

            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest("ID do usuário logado inválido.");

            var meusProdutos = await _context.Produtos
                .Where(p => p.AnuncianteId == userId)
                .Include(p => p.Usuario)
                .ToListAsync();

            return View(meusProdutos);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Reservar(int id)
        {
            var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.Id == id);
            if (produto == null)
                return NotFound();

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int usuarioId = int.Parse(userIdString);

            bool jaReservado = await _context.ReservasProduto
                .AnyAsync(r => r.ProdutoId == id && r.Status == StatusReserva.Pendente);

            if (jaReservado)
                return Json(new { success = false, message = "Produto já reservado." });

            var reserva = new ReservaProduto
            {
                ProdutoId = produto.Id,
                UsuarioId = usuarioId,
                Status = StatusReserva.Pendente
            };

            _context.ReservasProduto.Add(reserva);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Produto reservado com sucesso!" });
        }



    }

}
