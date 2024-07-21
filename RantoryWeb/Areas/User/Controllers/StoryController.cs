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

                if (storyFromDb is null)
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
                    var resetChapterName = await _dbContext.ChapterNameSources.FirstOrDefaultAsync(cn => cn.StoryId == storyFromDb.Id &&
                                                                                                         cn.ChapterId == chapter.Id);

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

        [HttpGet]
        public async Task<IActionResult> FinishStory(int? id, string? title)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is not null)
            {
                var storyFromDb = _dbContext.Stories.Include(chapter => chapter.Chapters).FirstOrDefault(story => story.Id == id && story.UserId == user.Id);

                if (id is not null && title is null)
                {
                    if (storyFromDb is not null)
                    {
                        storyFromDb.Title = null;

                        return View(storyFromDb);
                    }

                }
                else if (title is not null && storyFromDb is not null && id is not null && storyFromDb.Id == id)
                {
                    TempData["title"] = title;

                    return View(storyFromDb);
                }
            }


            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> FinishStory(Story story)
        {
            var user = await _userManager.GetUserAsync(User);
            var storyFromDb = await _dbContext.Stories.Include(chapter => chapter.Chapters).FirstOrDefaultAsync(s => s.Id == story.Id);

            if (storyFromDb is not null && user is not null && storyFromDb.UserId == user.Id)
            {
                bool emptyContent = storyFromDb.Chapters.Any(chapter => chapter.Content == null);

                if (story is not null && story.Title is not null && story.Title != "Untitled" && emptyContent == false)
                {
                    return RedirectToAction(nameof(CheckSameTitle), new
                    {
                        id = story.Id,
                        title = story.Title,
                        userId = story.UserId
                    });
                }
                else if (story is not null && story.Title is not null && story.Title != "Untitled" && emptyContent == true)
                {

                    TempData["unableToName"] = "Unable to name.";
                    TempData["emptyContent"] = "There is no content in the chapter.";

                    return RedirectToAction(nameof(FinishStory), new { id = storyFromDb.Id });
                }
                else if (story is not null && story.Title is null && emptyContent == true)
                {
                    TempData["nameIsNull"] = "Name not entered";
                    TempData["emptyContent"] = "There is no content in the chapter.";

                    return RedirectToAction(nameof(FinishStory), new { id = storyFromDb.Id });
                }
                else
                {
                    TempData["nameIsNull"] = "Name not entered";
                    return RedirectToAction(nameof(FinishStory), new { id = storyFromDb.Id });
                }

            }


            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> CheckSameTitle(int id, string title, string userId)
        {
            List<string?> allTitleFromDb = await _dbContext.Stories.Select(story => story.Title != null ? story.Title.ToLower() : null).ToListAsync();
            var storyFromDb = await _dbContext.Stories.Include(chapter => chapter.Chapters).FirstOrDefaultAsync(s => s.Id == id);

            if (storyFromDb is not null)
            {
                Story story = new()
                {
                    Id = id,
                    Title = title,
                    UserId = userId,
                };

                if (story.Title is not null && story.Id == storyFromDb.Id)
                {
                    int countSameStoryTitle = allTitleFromDb.Where(name => name == story.Title.ToLower()).Count();

                    if (allTitleFromDb.Contains(story.Title.ToLower()))
                    {
                        TempData["sameName"] = "Same name.";

                        if (countSameStoryTitle > 2)
                        {
                            TempData["countSameName"] = countSameStoryTitle.ToString();
                        }
                        else if (countSameStoryTitle == 1)
                        {
                            TempData["countSameName"] = "one";
                        }
                        else if (countSameStoryTitle == 2)
                        {
                            TempData["countSameName"] = "two";
                        }

                        return RedirectToAction(nameof(FinishStory), new { id = storyFromDb.Id, title = story.Title });
                    }
                    else
                    {
                        if (story.Title is not null && story.Title != "Untitled" && story.Id == storyFromDb.Id && story.UserId == storyFromDb.UserId)
                        {
                            storyFromDb.Title = story.Title;
                            storyFromDb.FinishStatus = true;

                            _dbContext.Stories.Update(storyFromDb);
                            await _dbContext.SaveChangesAsync();

                            return RedirectToAction(nameof(ShowCompletedStories));
                        }
                    }
                }
            }

            return NotFound();
        }

        
        public async Task<IActionResult> ShowCompletedStories()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is not null)
            {
                List<Story> completeStory = await _dbContext.Stories.Include(c => c.Chapters).Where(story => story.FinishStatus == true &&
                                                                                    story.UserId == user.Id)
                                                                    .ToListAsync();
                return View(completeStory);
            }

            return NotFound();

        }

        [HttpPost]
        public async Task<IActionResult> DeleteStory(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var storyFromDb = await _dbContext.Stories.Include(s => s.Chapters).FirstOrDefaultAsync(s => s.Id == id);

            if (storyFromDb != null)
            {
                foreach (var chapter in storyFromDb.Chapters.ToList())
                {
                    var resetChapterName = await _dbContext.ChapterNameSources.FirstOrDefaultAsync(cn => cn.StoryId == storyFromDb.Id &&
                                                                                                         cn.ChapterId == chapter.Id);

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

                return RedirectToAction(nameof(ShowCompletedStories));
            }

            return NotFound();

        }

        [HttpGet]
        public async Task<IActionResult> ReadStory(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is not null)
            {
                var storyFromDb = await _dbContext.Stories.Include(story => story.Chapters)
                                                      .FirstOrDefaultAsync(s => s.Id == id && s.UserId == user.Id);

                if (storyFromDb is not null)
                {
                    return View(storyFromDb);
                }
            }


            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> ReadContent(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if(user is not null)
            {
                var storyFromDb = await _dbContext.Stories.Include(story => story.Chapters)
                                                          .FirstOrDefaultAsync(s => s.UserId == user.Id);

                if(storyFromDb is not null)
                {
                    var chapterFromDb = await _dbContext.Chapters.FirstOrDefaultAsync(chapter => chapter.Id == id &&
                                                                                             chapter.UserId == user.Id &&
                                                                                             chapter.StoryId == storyFromDb.Id);

                    if(chapterFromDb is not null && chapterFromDb.Content is not null)
                    {
                        ChapterStoryVM chapterStoryVM = new()
                        {
                            Chapter = chapterFromDb,
                            Story = storyFromDb
                        };

                        return View(chapterStoryVM);
                    }
                }
                
            }


            return NotFound();
        }
    }
}
