using Libro.Models;
using Libro.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Libro.Controllers
{
    [Authorize(Roles = "Member")]
    public class FavoriteController : Controller
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IMemberRepository _memberRepository;

        public FavoriteController(IFavoriteRepository favoriteRepository, IMemberRepository memberRepository)
        {
            _favoriteRepository = favoriteRepository;
            _memberRepository = memberRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Toggle(int bookId)
        {
            var member = await GetCurrentMemberAsync();

            if (member == null)
            {
                return NotFound();
            }

            var existing = await _favoriteRepository.GetAsync(bookId, member.Id);

            if (existing != null)
            {
                _favoriteRepository.Remove(existing);
                TempData["SuccessMessage"] = "Removed from favorites.";
            }
            else
            {
                var favorite = new Favorite
                {
                    BookId = bookId,
                    MemberId = member.Id,
                    AddedDate = DateTime.Now
                };
                await _favoriteRepository.AddAsync(favorite);
                TempData["SuccessMessage"] = "Added to favorites.";
            }

            await _favoriteRepository.SaveChangesAsync();

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
        public async Task<IActionResult> MyFavorites()
        {
            var member = await GetCurrentMemberAsync();
            if (member == null) return NotFound();

            var favorites = await _favoriteRepository.GetMyFavoritesAsync(member.Id);
            return View(favorites);
        }

        private async Task<Member?> GetCurrentMemberAsync()
        {
            var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (identityUserId == null) return null;
            return await _memberRepository.GetByIdentityUserIdAsync(identityUserId);
        }
    }
}