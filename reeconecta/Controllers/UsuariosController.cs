using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using reeconecta.Models;

namespace reeconecta.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuarios.ToListAsync());
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Usuario usuario)
        {
            var dados = await _context.Usuarios
                .SingleOrDefaultAsync(u => u.Email == usuario.Email);

            if (dados == null)
            {
                ViewBag.Message = "Usuário e/ou senha inválidos.";
                return View();
            }

            bool SenhaCorreta = BCrypt.Net.BCrypt.Verify(usuario.Senha, dados.Senha);

            if (SenhaCorreta)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, dados.Nome ?? dados.NomeFantasia ?? "Usuário"),
                    new Claim(ClaimTypes.NameIdentifier, dados.Id.ToString()),
                    new Claim(ClaimTypes.Email, dados.Email),
                    new Claim(ClaimTypes.Role, dados.TipoUsuario.ToString())
                };

                var usuarioIdentity = new ClaimsIdentity(claims, "login");
                var principal = new ClaimsPrincipal(usuarioIdentity);

                var props = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTime.UtcNow.ToLocalTime().AddHours(8),
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(principal, props);
                return Redirect("/");
            }
            else
            {
                ViewBag.Message = "Usuário e/ou senha inválidos.";
                return View();
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        // GET: Usuarios/Details/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            // Verifica se o cadastro é de pessoa física ou jurídica
            if (usuario.TipodePerfil == TipoPerfil.PessoaFisica)
            {
                if (string.IsNullOrWhiteSpace(usuario.Nome))
                    ModelState.AddModelError("Nome", "É obrigatório informar o nome.");
            }

            if (usuario.TipodePerfil == TipoPerfil.PessoaJuridica)
            {
                if (string.IsNullOrWhiteSpace(usuario.RazaoSocial))
                    ModelState.AddModelError("RazaoSocial", "É obrigatório informar a razão social.");

                if (string.IsNullOrWhiteSpace(usuario.NomeFantasia))
                    ModelState.AddModelError("NomeFantasia", "É obrigatório informar o nome fantasia.");

                if (string.IsNullOrWhiteSpace(usuario.RepresentanteLegal))
                    ModelState.AddModelError("RepresentanteLegal", "É obrigatório informar o representante legal.");
            }

            // Verifica se o email inserido é único no BD
            bool emailExistente = await _context.Usuarios
                .AnyAsync(u => u.Email.ToLower() == usuario.Email.ToLower());

            if (emailExistente)
            {
                ModelState.AddModelError("Email", "Esse email já possui cadastro no ReeConecta.");
            }

            // Verifica se o documento inserido é único no BD
            bool documentoExistente = await _context.Usuarios
                .AnyAsync(u => u.Documento == usuario.Documento);

            if (documentoExistente)
            {
                ModelState.AddModelError("Documento", "Esse documento já possui cadastro no ReeConecta.");
            }

            if (ModelState.IsValid)
            {
                usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);
                usuario.ContaAtiva = true;
                usuario.CriacaoConta = DateTime.Now;
                // Garantir que WppTel2 seja false se for null
                usuario.WppTel2 = usuario.WppTel2 ?? false;
                // Para Pessoa Jurídica, usar NomeFantasia como Nome
                if (usuario.TipodePerfil == TipoPerfil.PessoaJuridica && string.IsNullOrEmpty(usuario.Nome))
                {
                    usuario.Nome = usuario.NomeFantasia ?? usuario.RazaoSocial;
                }

                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TipodePerfil,Documento,Nome,RazaoSocial,NomeFantasia,RepresentanteLegal, EmailRepresentante,TipoUsuario,Cep,Endereco,Telefone01,WppTel1,Telefone02,WppTel2,Email,Senha,ContaAtiva,CriacaoConta")] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);

                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
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
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Relatorio(int? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound();

            var anuncios = await _context.Produtos
                .Where(c => c.AnuncianteId == id)
                .OrderByDescending(c => c.CriacaoProduto)
                .ToListAsync();

            decimal totalVendido = anuncios?
                .Where(c => c.StatusProduto == StatusProduto.Vendido)
                .Sum(c => c.Preco) ?? 0;

            int itensVendidos = anuncios?
                .Count(c => c.StatusProduto == StatusProduto.Vendido) ?? 0;

            ViewBag.Usuario = usuario;
            ViewBag.TotalVendido = totalVendido.ToString("C");
            ViewBag.ItensVendidos = itensVendidos;

            return View(anuncios);
        }

        // GET: Usuarios/Perfil (Visualização)
        [Authorize]
        public async Task<IActionResult> Perfil()
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (usuarioIdClaim == null || !int.TryParse(usuarioIdClaim.Value, out int usuarioId))
            {
                return RedirectToAction("Login");
            }

            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/EditarPerfil (Modo edição)
        [Authorize]
        public async Task<IActionResult> EditarPerfil()
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (usuarioIdClaim == null || !int.TryParse(usuarioIdClaim.Value, out int usuarioId))
            {
                return RedirectToAction("Login");
            }

            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/EditarPerfil (Salvar alterações)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditarPerfil(int id, [Bind("Id,Nome,NomeFantasia,RazaoSocial,RepresentanteLegal,EmailRepresentante,Cep,Endereco,Telefone01,WppTel1,Telefone02,WppTel2,Email")] Usuario usuario, string? NovaSenha, string? ConfirmacaoSenha)
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (usuarioIdClaim == null || !int.TryParse(usuarioIdClaim.Value, out int usuarioIdLogado))
            {
                return RedirectToAction("Login");
            }

            // Validação de segurança: usuário só pode editar seu próprio perfil
            if (id != usuarioIdLogado)
            {
                return Forbid();
            }

            if (id != usuario.Id)
            {
                return NotFound();
            }

            // Remover validações dos campos que não estão sendo editados
            ModelState.Remove("Documento");
            ModelState.Remove("TipodePerfil");
            ModelState.Remove("Produtos");
            ModelState.Remove("ReservasProduto");
            ModelState.Remove("TipoUsuario");
            ModelState.Remove("Senha");
            ModelState.Remove("ContaAtiva");
            ModelState.Remove("CriacaoConta");

            // Validação de senha
            if (!string.IsNullOrEmpty(NovaSenha) || !string.IsNullOrEmpty(ConfirmacaoSenha))
            {
                if (NovaSenha != ConfirmacaoSenha)
                {
                    ModelState.AddModelError("", "A nova senha e a confirmação devem ser iguais.");
                }
                else if (NovaSenha?.Length < 6)
                {
                    ModelState.AddModelError("", "A nova senha deve ter no mínimo 6 caracteres.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var usuarioExistente = await _context.Usuarios.FindAsync(id);
                    if (usuarioExistente == null)
                    {
                        return NotFound();
                    }

                    // Atualizar a senha se fornecida
                    if (!string.IsNullOrEmpty(NovaSenha))
                    {
                        usuarioExistente.Senha = BCrypt.Net.BCrypt.HashPassword(NovaSenha);
                    }

                    // Atualizar os demais campos permitidos
                    usuarioExistente.Nome = usuario.Nome;
                    usuarioExistente.NomeFantasia = usuario.NomeFantasia;
                    usuarioExistente.Cep = usuario.Cep;
                    usuarioExistente.Endereco = usuario.Endereco;
                    usuarioExistente.Telefone01 = usuario.Telefone01;
                    usuarioExistente.WppTel1 = usuario.WppTel1;
                    usuarioExistente.Telefone02 = usuario.Telefone02;
                    usuarioExistente.WppTel2 = usuario.WppTel2 ?? false;
                    usuarioExistente.Email = usuario.Email;

                    // Atualizar campos da Pessoa Jurídica se aplicável
                    if (usuarioExistente.TipodePerfil == TipoPerfil.PessoaJuridica)
                    {
                        usuarioExistente.RepresentanteLegal = usuario.RepresentanteLegal;
                        usuarioExistente.EmailRepresentante = usuario.EmailRepresentante;
                        usuarioExistente.RazaoSocial = usuario.RazaoSocial;
                    }

                    _context.Update(usuarioExistente);
                    await _context.SaveChangesAsync();

                    // Atualizar os claims do usuário logado
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuarioExistente.Nome ?? usuarioExistente.NomeFantasia ?? "Usuário"),
                        new Claim(ClaimTypes.NameIdentifier, usuarioExistente.Id.ToString()),
                        new Claim(ClaimTypes.Email, usuarioExistente.Email),
                        new Claim(ClaimTypes.Role, usuarioExistente.TipoUsuario.ToString())
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    var props = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        ExpiresUtc = DateTime.UtcNow.ToLocalTime().AddHours(8),
                        IsPersistent = true
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

                    TempData["MensagemSucesso"] = "Perfil atualizado com sucesso!";
                    return RedirectToAction("Perfil");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Erro ao atualizar perfil: {ex.Message}");
                }
            }

            return View(usuario);
        }

        // GET: Usuarios/DesativarConta (Confirmação)
        [Authorize]
        public async Task<IActionResult> DesativarConta()
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (usuarioIdClaim == null || !int.TryParse(usuarioIdClaim.Value, out int usuarioId))
            {
                return RedirectToAction("Login");
            }

            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/DesativarConta (Desativar)
        [HttpPost, ActionName("DesativarConta")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DesativarContaConfirmado(int id)
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (usuarioIdClaim == null || !int.TryParse(usuarioIdClaim.Value, out int usuarioIdLogado))
            {
                return RedirectToAction("Login");
            }

            // Validação de segurança: usuário só pode desativar sua própria conta
            if (id != usuarioIdLogado)
            {
                return Forbid();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            try
            {
                // Desativar a conta ao invés de deletá-la
                usuario.ContaAtiva = false;
                _context.Update(usuario);
                await _context.SaveChangesAsync();

                // Deslogar o usuário
                await HttpContext.SignOutAsync();

                TempData["MensagemSucesso"] = "Sua conta foi desativada com sucesso.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao desativar conta: {ex.Message}";
                return RedirectToAction("Perfil");
            }
        }

        // POST: Recuperação de senha - Validar documento e email
        [AllowAnonymous]
        [HttpPost]
        public IActionResult ValidarRecuperacao([FromBody] RecuperacaoRequest req)
        {
            if (string.IsNullOrEmpty(req.Documento) || string.IsNullOrEmpty(req.Email))
            {
                return Json(new
                {
                    sucesso = false,
                    mensagem = "Documento e email são obrigatórios."
                });
            }

            // Remover formatação do documento
            string documentoLimpo = req.Documento.Replace(".", "").Replace("-", "").Replace("/", "");

            var usuario = _context.Usuarios
                .FirstOrDefault(x => x.Documento.Replace(".", "").Replace("-", "").Replace("/", "") == documentoLimpo
                                    && x.Email.ToLower() == req.Email.ToLower());

            if (usuario == null)
            {
                return Json(new
                {
                    sucesso = false,
                    mensagem = "Documento ou email não encontrados."
                });
            }

            // Guarda ID do usuário em sessão
            HttpContext.Session.SetInt32("idRecuperacao", usuario.Id);

            string nomeExibicao = !string.IsNullOrEmpty(usuario.Nome)
                ? usuario.Nome
                : usuario.NomeFantasia;

            return Json(new
            {
                sucesso = true,
                nome = nomeExibicao
            });
        }

        // POST: Recuperação de senha - Salvar nova senha
        [AllowAnonymous]
        [HttpPost]
        public IActionResult SalvarNovaSenha([FromBody] SenhaRequest req)
        {
            int? id = HttpContext.Session.GetInt32("idRecuperacao");

            if (id == null)
            {
                return Json(new
                {
                    sucesso = false,
                    mensagem = "Sessão expirada. Tente novamente."
                });
            }

            if (string.IsNullOrEmpty(req.NovaSenha) || req.NovaSenha.Length < 6)
            {
                return Json(new
                {
                    sucesso = false,
                    mensagem = "A senha deve ter no mínimo 6 caracteres."
                });
            }

            var usuario = _context.Usuarios.Find(id);

            if (usuario == null)
            {
                return Json(new
                {
                    sucesso = false,
                    mensagem = "Usuário não encontrado."
                });
            }

            usuario.Senha = BCrypt.Net.BCrypt.HashPassword(req.NovaSenha);
            _context.SaveChanges();

            // Limpar sessão
            HttpContext.Session.Remove("idRecuperacao");

            return Json(new
            {
                sucesso = true,
                mensagem = "Senha alterada com sucesso!"
            });
        }
    }

    // DTOs para requisições
    public class RecuperacaoRequest
    {
        public string Documento { get; set; }
        public string Email { get; set; }
    }

    public class SenhaRequest
    {
        public string NovaSenha { get; set; }
    }
}
