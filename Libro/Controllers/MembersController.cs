using Libro.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Libro.Controllers
{
    [Authorize(Roles = "Librarian")]
    public class MembersController : Controller
    {
        private readonly IMemberRepository _memberRepository;

        public MembersController(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var members = await _memberRepository.GetAllAsync();
            return View(members);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var member = await _memberRepository.GetByIdWithLoansAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleSuspend(int id)
        {
            var member = await _memberRepository.GetByIdWithLoansAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            member.IsSuspended = !member.IsSuspended;
            await _memberRepository.SaveChangesAsync();

            TempData["SuccessMessage"] = member.IsSuspended
                ? "Member account suspended."
                : "Member account reactivated.";

            return RedirectToAction("Details", new { id });
        }
    }
}