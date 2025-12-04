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
            var query = _context.Pontos.Include(p => p.Avaliacoes).AsQueryable();

            // FILTRO POR TIPO (Compra ou Descarte)
            if (!string.IsNullOrEmpty(tipo) && Enum.TryParse<TipoPonto>(tipo, true, out var tipoEnum))
            {
                query = query.Where(p => p.Tipo == tipoEnum);
            }

            var dados = await query.ToListAsync();
            return View(dados);
        }

        // GET: Pontos/MeusPontos
        [Authorize]
        public async Task<IActionResult> MeusPontos(string? tipo)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Usuarios");

            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest("ID do usuário logado inválido.");

            var query = _context.Pontos
                .Where(p => p.CriadoPorUsuarioId == userId)
                .AsQueryable();

            // FILTRO POR TIPO (Compra ou Descarte)
            if (!string.IsNullOrEmpty(tipo) && Enum.TryParse<TipoPonto>(tipo, true, out var tipoEnum))
            {
                query = query.Where(p => p.Tipo == tipoEnum);
            }

            var meusPontos = await query.ToListAsync();
            return View(meusPontos);
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
            // Incluir as avaliações relacionadas ao carregar o ponto
            var dados = await _context.Pontos
                .Include(p => p.Avaliacoes)
                .FirstOrDefaultAsync(p => p.Id == id);
                
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
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int? id, string? returnUrl)
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

            // Retorna para a página anterior (MeusPontos ou Index)
            if (returnUrl == "MeusPontos")
            {
                return RedirectToAction("MeusPontos");
            }
            
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Avaliar(int PontoId, string NotaString, string? Comentario)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            if (usuarioId == 0)
                return RedirectToAction("Login", "Usuarios");

            // Converter e validar nota com suporte a valores decimais
            // Normalizar o separador decimal para ponto
            NotaString = NotaString?.Replace(',', '.');
            
            if (!decimal.TryParse(NotaString, System.Globalization.CultureInfo.InvariantCulture, out decimal nota))
            {
                TempData["Erro"] = "Nota inválida.";
                return RedirectToAction("Details", new { id = PontoId });
            }

            // Validar nota
            if (nota < 0 || nota > 5)
            {
                TempData["Erro"] = "A nota deve estar entre 0 e 5.";
                return RedirectToAction("Details", new { id = PontoId });
            }

            // Verificar se o ponto existe
            var ponto = await _context.Pontos.FindAsync(PontoId);
            if (ponto == null)
                return NotFound();

            // Verificar se o usuário já avaliou
            var avaliacaoExistente = await _context.Avaliacoes
                .FirstOrDefaultAsync(a => a.PontoId == PontoId && a.UsuarioId == usuarioId);

            if (avaliacaoExistente != null)
            {
                // Atualizar avaliação existente
                avaliacaoExistente.Nota = nota;
                avaliacaoExistente.Comentario = Comentario;
                avaliacaoExistente.DataAvaliacao = DateTime.Now;
                _context.Avaliacoes.Update(avaliacaoExistente);
            }
            else
            {
                // Criar nova avaliação
                var avaliacao = new Avaliacao
                {
                    PontoId = PontoId,
                    UsuarioId = usuarioId,
                    Nota = nota,
                    Comentario = Comentario,
                    DataAvaliacao = DateTime.Now
                };
                _context.Avaliacoes.Add(avaliacao);
            }

            await _context.SaveChangesAsync();
            TempData["Sucesso"] = "Avaliação registrada com sucesso!";
            return RedirectToAction("Details", new { id = PontoId });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> RemoverAvaliacao(int id)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            if (usuarioId == 0)
                return RedirectToAction("Login", "Usuarios");

            // Verificar se a avaliação existe e pertence ao usuário
            var avaliacao = await _context.Avaliacoes
                .FirstOrDefaultAsync(a => a.PontoId == id && a.UsuarioId == usuarioId);

            if (avaliacao == null)
            {
                TempData["Erro"] = "Avaliação não encontrada.";
                return RedirectToAction("Details", new { id = id });
            }

            _context.Avaliacoes.Remove(avaliacao);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Avaliação removida com sucesso!";
            return RedirectToAction("Details", new { id = id });
        }
    }
}