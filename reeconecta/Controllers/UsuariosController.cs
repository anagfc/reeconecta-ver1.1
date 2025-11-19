using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using reeconecta.Models;
using reeconecta.Models.DTOs;


namespace reeconecta.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Usuarios
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                //usuario.TipoUsuario = TipoUsuario.User;
                usuario.ContaAtiva = true;
                usuario.CriacaoConta = DateTime.Now;

                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // Recuperação da senha
        [AllowAnonymous]
        [HttpPost]
        public IActionResult ValidarRecuperacao([FromBody] RecuperacaoRequest req)
        {
            var usuario = _context.Usuarios
                .FirstOrDefault(x => x.Documento == req.Documento && x.Email == req.Email);

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
                    mensagem = "Sessão expirada."
                });
            }

            var usuario = _context.Usuarios.Find(id);

            usuario.Senha = BCrypt.Net.BCrypt.HashPassword(req.NovaSenha);

            _context.SaveChanges();

            return Json(new
            {
                sucesso = true
            });
        }



        // GET: Usuarios/Edit/5
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        public async Task<IActionResult> Relatorio  (int? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound();

            var anuncios = await _context.Produtos
                .Where(c => c.AnuncianteId == id)
                .OrderByDescending(c => c.CriacaoAnuncio)
                .ToListAsync();

            decimal totalVendido = anuncios?
                .Where(c => c.StatusAnuncio == StatusAnuncio.Vendido)
                .Sum(c => c.Preco) ?? 0;

            int itensVendidos = anuncios?
                .Count(c => c.StatusAnuncio == StatusAnuncio.Vendido) ?? 0;

            ViewBag.Usuario = usuario;
            ViewBag.TotalVendido = totalVendido.ToString("C");
            ViewBag.ItensVendidos = itensVendidos;

            return View(anuncios);

        }
    }
}
