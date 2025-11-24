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
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? busca, string? cidade, string? condicao, string? ordenar)
        {
            var query = _context.Produtos
                .AsNoTracking()
                .Where(p => p.StatusProduto == StatusProduto.Disponivel &&
                    p.Usuario.ContaAtiva)
                .Include(p => p.Usuario)
                .Include(p => p.ReservasProduto)
                .AsQueryable();

            // BUSCA (título e descrição)
            if (!string.IsNullOrWhiteSpace(busca))
            {
                query = query.Where(p =>
                    p.Titulo.Contains(busca) ||
                    p.Descricao.Contains(busca));
            }

            // FILTRO POR CIDADE
            if (!string.IsNullOrWhiteSpace(cidade))
            {
                query = query.Where(p => p.Cidade == cidade);
            }

            // FILTRO POR CONDIÇÃO (Novo, SemiNovo, Usado)
            if (!string.IsNullOrEmpty(condicao) &&
             Enum.TryParse<CondicaoProduto>(condicao, true, out var condicaoEnum))
            {
                query = query.Where(p => p.Condicao == condicaoEnum);
            }

            // ORDENAR
            query = ordenar switch
            {
                "preco_asc" => query.OrderBy(p => p.Preco),
                "preco_desc" => query.OrderByDescending(p => p.Preco),
                "titulo_asc" => query.OrderBy(p => p.Titulo),
                "titulo_desc" => query.OrderByDescending(p => p.Titulo),
                _ => query.OrderByDescending(p => p.CriacaoProduto)
            };

            // Carrega lista final
            var produtos = await query.ToListAsync();

            // Enviar cidades únicas para dropdown
            ViewBag.Cidades = await _context.Produtos
                .Select(p => p.Cidade)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            return View(produtos);
        }


        // GET: Produtos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var produto = await _context.Produtos
                .Include(p => p.Usuario) 
                .Include(p => p.ReservasProduto) 
                    .ThenInclude(r => r.Usuario) 
                .FirstOrDefaultAsync(p => p.Id == id);


            if (produto == null)
                return NotFound();

            return View(produto);
        }

        // Confirmar e recusar
        [HttpGet]
        public async Task<IActionResult> Confirmar(int id)
        {
            var reserva = await _context.ReservasProduto
                .Include(r => r.Produto)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null)
            {
                TempData["Error"] = "Reserva não encontrada.";
                return RedirectToAction(nameof(MeusProdutos));
            }

            reserva.Status = StatusReserva.Confirmada;

            if (reserva.Produto != null)
            {
                reserva.Produto.StatusProduto = StatusProduto.Vendido;

                var outrasReservas = await _context.ReservasProduto
                    .Where(r => r.ProdutoId == reserva.ProdutoId && r.Id != reserva.Id && r.Status == StatusReserva.Pendente)
                    .ToListAsync();

                foreach (var r in outrasReservas)
                {
                    r.Status = StatusReserva.Cancelada;
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Reserva confirmada com sucesso!";
            return RedirectToAction(nameof(MeusProdutos));
        }



        [HttpGet]
        public async Task<IActionResult> Recusar(int id)
        {
            var reserva = await _context.ReservasProduto.FindAsync(id);
            if (reserva == null)
            {
                TempData["Error"] = "Reserva não encontrada.";
                return RedirectToAction(nameof(MeusProdutos));
            }

            reserva.Status = StatusReserva.Cancelada;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Reserva recusada com sucesso!";
            return RedirectToAction(nameof(MeusProdutos));
        }



        // GET: Produtos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Produtos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Titulo,Preco,Descricao,Condicao,Bairro,Cidade,Imagem")] Produto produto, IFormFile? ImagemFile)
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
                else
                {
                    produto.Imagem = "/images/produtos/semimagem.png";
                }


                _context.Add(produto);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Anúncio cadastrado com sucesso!";
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

            return View(produto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Produto produto, IFormFile? ImagemFile)
        {
            if (id != produto.Id)
            {
                TempData["Error"] = "Erro ao atualizar o produto!";
                return RedirectToAction(nameof(MeusProdutos));
            }

            if (ModelState.IsValid)
            {
                var produtoDb = await _context.Produtos.FindAsync(id);
                if (produtoDb == null)
                {
                    TempData["Error"] = "Erro ao atualizar o produto!";
                    return RedirectToAction(nameof(MeusProdutos));
                }

                produtoDb.Titulo = produto.Titulo;
                produtoDb.Preco = produto.Preco;
                produtoDb.Descricao = produto.Descricao;
                produtoDb.Condicao = produto.Condicao;
                produtoDb.Bairro = produto.Bairro;
                produtoDb.Cidade = produto.Cidade;
                produtoDb.StatusProduto = produto.StatusProduto;

                if (ImagemFile != null && ImagemFile.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await ImagemFile.CopyToAsync(ms);
                    produtoDb.Imagem = $"data:{ImagemFile.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}";
                }


                await _context.SaveChangesAsync();

                TempData["Success"] = "Produto atualizado com sucesso!";
                return RedirectToAction(nameof(MeusProdutos));
            }

            return View(produto);
        }



        // GET: Produtos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Erro ao deletar o produto!";
                return RedirectToAction(nameof(MeusProdutos));
            }

            var produto = await _context.Produtos
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (produto == null)
            {
                TempData["Error"] = "Erro ao deletar o produto!";
                return RedirectToAction(nameof(MeusProdutos));
            }

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
                var reservas = _context.ReservasProduto
                                       .Where(r => r.ProdutoId == id)
                                       .ToList(); 
                _context.ReservasProduto.RemoveRange(reservas);

                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();


                TempData["Success"] = "Produto excluído com sucesso!";
            }

            return RedirectToAction(nameof(MeusProdutos));
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

        // GET: Produtos/MinhasReservas
        public async Task<IActionResult> MinhasReservas()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Usuarios");

            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest("ID do usuário logado inválido.");

            var minhasReservas = await _context.ReservasProduto
                .Include(r => r.Produto)
                .ThenInclude(p => p.Usuario)
                .Where(r => r.UsuarioId == userId)
                .ToListAsync();

            return View(minhasReservas);
        }

        [HttpGet]
        public async Task<IActionResult> CancelarReserva(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Usuarios");

            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest("ID do usuário logado inválido.");

            var reserva = await _context.ReservasProduto
                .FirstOrDefaultAsync(r => r.Id == id && r.UsuarioId == userId);

            if (reserva == null)
                return NotFound();

            reserva.Status = StatusReserva.Cancelada;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MinhasReservas));
        }



        // GET: Produtos/Reservar
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
