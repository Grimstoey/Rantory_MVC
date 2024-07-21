using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IActionResult> Index(int id)
        {
            //Show all chapter in story

            var user = await _userManager.GetUserAsync(User);
           

            if (user is not null)
            {
                var storyFromDb = await _dbContext.Stories.Include(story => story.Chapters)
                                                          .FirstOrDefaultAsync(s => s.Id == id && s.UserId == user.Id);

                return View(storyFromDb);

            }

            return NotFound();

        }

        public async Task<IActionResult> CreateChapter(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is not null)
            {
                var storyFromDb = _dbContext.Stories.FirstOrDefault(story => story.Id == id && story.UserId == user.Id);

                if (storyFromDb is not null)
                {
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

                    Chapter newChapter = new()
                    {
                        UserId = user.Id,
                        ChapterName = name,
                        StoryId = storyFromDb.Id
                    };
                    await _dbContext.Chapters.AddAsync(newChapter);
                    await _dbContext.SaveChangesAsync();

                    storyFromDb.Chapters.Add(newChapter);
                    _dbContext.Stories.Update(storyFromDb);
                    await _dbContext.SaveChangesAsync();

                    chapterNameFromDb.StoryId = storyFromDb.Id;
                    chapterNameFromDb.ChapterId = newChapter.Id;
                    _dbContext.ChapterNameSources.Update(chapterNameFromDb);
                    await _dbContext.SaveChangesAsync();

                    return RedirectToAction(nameof(Index), new {id = storyFromDb.Id});
                }
                else
                {
                    return NotFound();
                }

            }
            else
            {
                return Redirect("/Identity/Account/Login");
            }

        }

        [HttpGet]
        public async Task<IActionResult> EditChapter(int chapterId, int storyId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is not null)
            {
                var storyFromDb = await _dbContext.Stories.FirstOrDefaultAsync(sdb => sdb.UserId == user.Id && sdb.Id == storyId);
                var chapterFromDb = await _dbContext.Chapters
                                                    .FirstOrDefaultAsync(cdb => cdb.UserId == user.Id &&
                                                                                cdb.Id == chapterId &&
                                                                                cdb.StoryId == storyId);

                List<Chapter> chapterList = _dbContext.Chapters.Where(cdb => cdb.UserId == user.Id && cdb.StoryId == storyId)
                                                               .OrderBy(c => c.Id)
                                                               .ToList();

                if (storyFromDb is not null && chapterFromDb is not null)
                {
                    int chapterIndex = chapterList.IndexOf(chapterFromDb) + 1;

                    ChapterStoryVM chapterStoryVM = new()
                    {
                        Chapter = chapterFromDb,
                        Story = storyFromDb,
                        ChapterIndex = chapterIndex
                    };

                    return View(chapterStoryVM);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }

        }


        [HttpPost]
        public async Task<IActionResult> EditChapter(ChapterStoryVM chapterStoryVM)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is not null && user.Id == chapterStoryVM.Story.UserId &&
                                    user.Id == chapterStoryVM.Chapter.UserId)
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }
                else
                {
                    var storyFromDb = await _dbContext.Stories
                                                      .FirstOrDefaultAsync(sdb => sdb.UserId == chapterStoryVM.Story.UserId &&
                                                                                  sdb.Id == chapterStoryVM.Story.Id);

                    var chapterFromDb = await _dbContext.Chapters
                                                       .FirstOrDefaultAsync(cdb => cdb.UserId == chapterStoryVM.Chapter.UserId &&
                                                                                   cdb.Id == chapterStoryVM.Chapter.Id &&
                                                                                   cdb.StoryId == chapterStoryVM.Story.Id);

                    if (storyFromDb is not null && chapterFromDb is not null)
                    {
                        chapterFromDb.ChapterName = chapterStoryVM.Chapter.ChapterName;
                        chapterFromDb.Content = chapterStoryVM.Chapter.Content;
                        _dbContext.Chapters.Update(chapterFromDb);
                        await _dbContext.SaveChangesAsync();

                        TempData["ChapterSuccess"] = "Chapter updated successfully!";
                        TempData["ChapterName"] = chapterFromDb.ChapterName;

                        return RedirectToAction(nameof(Index), new {id = storyFromDb.Id});

                    }
                    else
                    {
                        return NotFound();
                    }

                }
            }
            else
            {
                return NotFound();
            }
        }
    }
}
