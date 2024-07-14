using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rantory.DataAccess;
using Rantory.Models;

namespace RantoryWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ChapterNameController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public ChapterNameController(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateChapterName()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return Redirect("/Identity/Account/Login");
            }
            else
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Admin"))
                {
                    ChapterNameSource chapterNameSource = new();

                    return View(chapterNameSource);
                }
                else
                {
                    return Forbid();
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateChapterName(ChapterNameSource chapterName)
        {
            if(!ModelState.IsValid)
            {
                return View(chapterName);
            }
            else
            {
                List<string> allChapterName = _dbContext.ChapterNameSources.Select(cn => cn.Name.ToLower()).ToList();

                if(allChapterName.Contains(chapterName.Name.ToLower()))
                {
                    TempData["CreationFailed"] = "Chapter name creation failed.";

                    return View(chapterName);
                }
                else
                {
                    await _dbContext.ChapterNameSources.AddAsync(chapterName);
                    await _dbContext.SaveChangesAsync();

                    TempData["SuccessAddChapterName"] = "Chapter name created successfully.";
                    TempData["NewChapterName"] = chapterName.Name.ToString();

                    ModelState.Clear();


                    return RedirectToAction(nameof(CreateChapterName));
                }                

            }
        }

    }
}
