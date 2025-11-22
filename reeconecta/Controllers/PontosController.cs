using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using reeconecta.Models;
using System.Security.Claims;

namespace reeconecta.Controllers
{
    public class PontosController : Controller
    {
        private readonly AppDbContext _context;
        public PontosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? tipo)
        {
            var query = _context.Pontos.AsQueryable();

            // FILTRO POR TIPO (Compra ou Descarte)
            if (!string.IsNullOrEmpty(tipo) && Enum.TryParse<TipoPonto>(tipo, true, out var tipoEnum))
            {
                query = query.Where(p => p.Tipo == tipoEnum);
            }

            var dados = await query.ToListAsync();
            return View(dados);
        }

        //Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Ponto ponto, IFormFile ImagemFile)
        {
            // Validação: imagem é obrigatória
            if (ImagemFile == null || ImagemFile.Length == 0)
            {
                ModelState.AddModelError("ImagemFile", "Obrigatório enviar uma imagem do ponto de coleta.");
            }

            if (ModelState.IsValid)
            {
                // Obtém o ID do usuário atual
                var usuarioIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(usuarioIdStr, out var usuarioId))
                {
                    ponto.CriadoPorUsuarioId = usuarioId;
                }

                if (ImagemFile != null && ImagemFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImagemFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/pontos", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await ImagemFile.CopyToAsync(stream);

                    ponto.Imagem = "/images/pontos/" + fileName;
                }

                ponto.DataCriacao = DateTime.Now;
                _context.Pontos.Add(ponto);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(ponto);
        }

        //Edit
        [Authorize]
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

            // Verifica autorização
            if (!UsuarioPodeEditarPonto(dados))
            {
                return Forbid();
            }

            return View(dados);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, Ponto ponto, IFormFile? ImagemFile)
        {
            if (id != ponto.Id)
            {
                return NotFound();
            }

            // Verifica se o ponto existe
            var pontoAtual = await _context.Pontos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (pontoAtual == null)
            {
                return NotFound();
            }

            // Verifica autorização
            if (!UsuarioPodeEditarPonto(pontoAtual))
            {
                return Forbid();
            }

            // Remove a validação do ImagemFile para a edição (não é obrigatório editar a imagem)
            ModelState.Remove("ImagemFile");

            if (ModelState.IsValid)
            {
                try
                {
                    // Se uma nova imagem for enviada, processa e substitui
                    if (ImagemFile != null && ImagemFile.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImagemFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/pontos", fileName);
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                            await ImagemFile.CopyToAsync(stream);

                        ponto.Imagem = "/images/pontos/" + fileName;
                    }
                    else
                    {
                        // Se nenhuma imagem foi enviada, mantém a imagem existente
                        ponto.Imagem = pontoAtual.Imagem;
                    }

                    // Preserva os dados de criação
                    ponto.CriadoPorUsuarioId = pontoAtual.CriadoPorUsuarioId;
                    ponto.DataCriacao = pontoAtual.DataCriacao;

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
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var dados = await _context.Pontos.FindAsync(id);

            if (dados == null)
                return NotFound();

            // Verifica autorização
            if (!UsuarioPodeEditarPonto(dados))
            {
                return Forbid();
            }

            return View(dados);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
                return NotFound();

            var dados = await _context.Pontos.FindAsync(id);

            if (dados == null)
                return NotFound();

            // Verifica autorização
            if (!UsuarioPodeEditarPonto(dados))
            {
                return Forbid();
            }

            _context.Pontos.Remove(dados);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Método auxiliar para verificar autorização
        private bool UsuarioPodeEditarPonto(Ponto ponto)
        {
            // Administrador pode editar qualquer ponto
            if (User.IsInRole("Administrador"))
            {
                return true;
            }

            // Usuário comum só pode editar seus próprios pontos - usando ClaimTypes.NameIdentifier
            var usuarioIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(usuarioIdStr, out var usuarioId))
            {
                return ponto.CriadoPorUsuarioId == usuarioId;
            }

            return false;
        }

        private bool PontoExists(int id)
        {
            return _context.Pontos.Any(e => e.Id == id);
        }
    }
}