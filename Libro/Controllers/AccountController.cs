using Libro.Data;
using Libro.Models;
using Libro.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Libro.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;


        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }
        // GET: AccountController
   
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        // POST: AccountController/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);
                if (model.Role == "Member")
                {
                    var member = new Member
                    {
                        FullName = model.Fullname,
                        IdentityUserId = user.Id,
                        ImageUrl = model.ImageUrl,
                        CreatedDate = DateTime.Now
                    };
                    _context.Members.Add(member);
                    await _context.SaveChangesAsync();
                }
                else if (model.Role == "Librarian")
                {
                    var librarian = new Librarian
                    {
                        FullName = model.Fullname,
                        IdentityUserId = user.Id,
                        ImageUrl = model.ImageUrl
                    };
                    _context.Librarians.Add(librarian);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Login");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    string friendlyMessage = error.Code switch
                    {
                        "DuplicateUserName" => "This email is already registered. Try logging in instead.",
                        "PasswordTooShort" => "Password must be at least 6 characters long.",
                        "PasswordRequiresDigit" => "Password must contain at least one number.",
                        "PasswordRequiresUpper" => "Password must contain at least one uppercase letter.",
                        "PasswordRequiresLower" => "Password must contain at least one lowercase letter.",
                        "PasswordRequiresNonAlphanumeric" => "Password must contain at least one special character (e.g. !, @, #).",
                        "InvalidEmail" => "Please enter a valid email address.",
                        _ => "Something went wrong. Please try again."
                    };

                    ModelState.AddModelError("", friendlyMessage);
                }
                return View(model);
            }
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RemeberMe,
                lockoutOnFailure: true
            );

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Too many failed attempts. Please try again in 15 minutes.");
                return View(model);
            }

            ModelState.AddModelError("", "Invalid login attempt");
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
