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

            if(user is null)
            {
                return Redirect("/Identity/Account/Login");
            }
            else
            {
                storyFromDb = _dbContext.Stories.Where(story => story.UserId == user.Id).Include(story => story.Chapters).ToList();
                return View(storyFromDb);
            }
            
        }


        [HttpGet]
        public async Task<IActionResult> CreateStory()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is not null)
            {
                ChapterStoryVM chapterStoryVM = new()
                {
                    Story = new Story { UserId = user.Id },
                    Chapter = new Chapter { UserId = user.Id }
                };

                return View(chapterStoryVM);
            }

            return Redirect("/Identity/Account/Login");
        }

        [HttpPost]
        public async Task<IActionResult> CreateStory(ChapterStoryVM chapterStoryVM)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user is not null && chapterStoryVM.Story.UserId == user.Id)
                {

                    await _dbContext.Stories.AddAsync(chapterStoryVM.Story);
                    await _dbContext.SaveChangesAsync();

                    chapterStoryVM.Chapter.StoryId = chapterStoryVM.Story.Id;

                    await _dbContext.Chapters.AddAsync(chapterStoryVM.Chapter);
                    await _dbContext.SaveChangesAsync();

                    return RedirectToAction(nameof(Index), new { id = chapterStoryVM.Story.Id });
                }
                else
                {
                    return NotFound();
                }
            }

            return View(chapterStoryVM);
        }


        public async Task<IActionResult> UpdateStory(Story? story)
        {

            //Show all chapter in story

            var user = await _userManager.GetUserAsync(User);

            if (user is not null && story is not null)
            {
                var storyFromDb = _dbContext.Stories.Where(s => s.UserId == user.Id && s.Id == story.Id).Include(story => story.Chapters).First();


                return View(storyFromDb);
            }
            else
            {
                return NotFound();
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> UpdateStory(Story story)
        //{
        //    return View();
        //}


        public async Task<IActionResult> CreateChapter(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is not null)
            {
                var storyFromDb = _dbContext.Stories.FirstOrDefault(story => story.Id == id && story.UserId == user.Id);

                if (storyFromDb is not null)
                {
                    Chapter newChapter = new()
                    {
                        UserId = user.Id
                    };

                    ChapterStoryVM chapterStoryVM = new()
                    {
                        Story = storyFromDb,
                        Chapter = newChapter
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
                return Redirect("/Identity/Account/Login");
            }

        }

        [HttpPost]
        public async Task<IActionResult> CreateChapter(ChapterStoryVM chapterStoryVM)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is not null && user.Id == chapterStoryVM.Story.UserId)
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }
                else
                {
                    Chapter newChapter = new()
                    {
                        Id = chapterStoryVM.Chapter.Id,
                        ChapterName = chapterStoryVM.Chapter.ChapterName,
                        Content = chapterStoryVM.Chapter.Content,
                        UserId = user.Id,
                        StoryId = chapterStoryVM.Story.Id
                    };

                    await _dbContext.Chapters.AddAsync(newChapter);
                    await _dbContext.SaveChangesAsync();

                    var storyFromDb = _dbContext.Stories.FirstOrDefault(story => story.UserId == user.Id && story.Id == chapterStoryVM.Story.Id);

                    if (storyFromDb is null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        storyFromDb.Chapters.Add(newChapter);

                        _dbContext.Stories.Update(storyFromDb);
                        await _dbContext.SaveChangesAsync();
                        return RedirectToAction(nameof(UpdateStory), storyFromDb);
                    }


                }
            }
            return NotFound();
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

                        return RedirectToAction(nameof(EditChapter), new { chapterId = chapterFromDb.Id, storyId = storyFromDb.Id });

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


        [HttpPost]
        public async Task<IActionResult> DeleteAllChapter(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var storyFromDb = await _dbContext.Stories.Include(c=> c.Chapters).FirstOrDefaultAsync(s => s.Id == id);

            if(user is not null && storyFromDb is not null && storyFromDb.UserId == user.Id)
            {
                _dbContext.Chapters.RemoveRange(storyFromDb.Chapters);
                _dbContext.Stories.Remove(storyFromDb);
                await _dbContext.SaveChangesAsync();

                TempData["DeleteSuccess"] = "Delete successfully!";

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return NotFound();
            }
        }
    }
}
