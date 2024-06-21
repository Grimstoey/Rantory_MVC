using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Rantory.DataAccess;
using Rantory.Models;
using Rantory.Models.ViewModels;

namespace RantoryWeb.Areas.User.Controllers
{
    [Area("User")]
    public class ChapterController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChapterController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            //Show all chapter are not finish
            var user = await _userManager.GetUserAsync(User);

            if(user is not null)
            {

            }

            return View();
        }



        public async Task<IActionResult> CreateChapter()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is not null)
            {

                Chapter chapter = new()
                {
                    UserId = user.Id
                };

                return View(chapter);
            }

            return Redirect("/Identity/Account/Login");


        }

        [HttpPost]
        public async Task<IActionResult> CreateChapter(Chapter chapter)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(chapter.UserId);

                var currentUser = await _userManager.GetUserAsync(User);

                if (user is not null && currentUser is not null)
                {
                    if (user.Id != currentUser.Id)
                    {
                        return NotFound();
                    }

                    chapter.UserId = user.Id;

                    UserChapterVM userChapterVM = new()
                    {
                        ApplicationUser = currentUser
                    };

                    userChapterVM.Chapters.Add(chapter);

                    await _dbContext.Chapters.AddAsync(chapter);
                    await _dbContext.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), userChapterVM);
                }
            }

            return View();
        }
    }
}
