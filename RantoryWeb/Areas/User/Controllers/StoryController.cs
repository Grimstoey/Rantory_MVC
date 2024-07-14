using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rantory.DataAccess;
using Rantory.Models;
using Rantory.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace RantoryWeb.Areas.User.Controllers
{
    [Area("User")]
    public class StoryController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public StoryController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            //Show all story are not finish
            var user = await _userManager.GetUserAsync(User);
            List<Story> storyFromDb = [];

            if (user is null)
            {
                return Redirect("/Identity/Account/Login");
            }
            else
            {
                storyFromDb = _dbContext.Stories.Where(story => story.UserId == user.Id).Include(story => story.Chapters).ToList();

                if(storyFromDb is null)
                {
                    return View();
                }
                else
                {
                    return View(storyFromDb);
                }
  
            }

        }


        [HttpGet]
        public async Task<IActionResult> CreateStory()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                
                return Redirect("/Identity/Account/Login");

            }

            var allNames = await _dbContext.ChapterNameSources.Select(cn => cn.Name).ToListAsync();
            Random random = new();
            ChapterNameSource? chapterNameFromDb = new();
            int number;
            string name = "";

            do
            {
                number = random.Next(0, allNames.Count);
                name = allNames[number];
                chapterNameFromDb = await _dbContext.ChapterNameSources.FirstOrDefaultAsync(cn => cn.Name == name && 
                                                                                            cn.StoryId == null && 
                                                                                            cn.ChapterId == null);
            }
            while (chapterNameFromDb is null);

            Story newStory = new() { UserId = user.Id };
            await _dbContext.Stories.AddAsync(newStory);
            await _dbContext.SaveChangesAsync();


            Chapter newChapter = new()
            {
                UserId = user.Id,
                ChapterName = name,
                StoryId = newStory.Id
            };
            await _dbContext.Chapters.AddAsync(newChapter);
            await _dbContext.SaveChangesAsync();

            chapterNameFromDb.StoryId = newStory.Id;
            chapterNameFromDb.ChapterId = newChapter.Id;
            _dbContext.ChapterNameSources.Update(chapterNameFromDb);
            await _dbContext.SaveChangesAsync();

            TempData["AddNewStory"] = "Create new story successfully!";

            return RedirectToAction(nameof(Index));

            
        }


        [HttpPost]
        public async Task<IActionResult> DeleteAllChapter(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var storyFromDb = await _dbContext.Stories.Include(s => s.Chapters).FirstOrDefaultAsync(s => s.Id == id);

            if (storyFromDb != null)
            {
                foreach (var chapter in storyFromDb.Chapters.ToList())
                {
                    var resetChapterName = await _dbContext.ChapterNameSources.FirstOrDefaultAsync(cn => cn.StoryId == storyFromDb.Id && cn.ChapterId == chapter.Id);

                    if (resetChapterName != null)
                    {
                        resetChapterName.StoryId = null;
                        resetChapterName.ChapterId = null;
                        _dbContext.ChapterNameSources.Update(resetChapterName);
                    }

                    _dbContext.Chapters.Remove(chapter);
                }

                _dbContext.Stories.Remove(storyFromDb);
                await _dbContext.SaveChangesAsync();

                TempData["DeleteSuccess"] = "Delete successfully!";

                return RedirectToAction(nameof(Index));
            }

            return NotFound();

        }

        public async Task<IActionResult> FinishStory(int id)
        {


            return View();
        }
    }
}
