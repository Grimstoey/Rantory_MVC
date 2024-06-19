using Microsoft.AspNetCore.Mvc;
using Rantory.DataAccess;

namespace RantoryWeb.Areas.User.Controllers
{
    [Area("User")]
    public class StoryController : Controller
    {
        public ApplicationDbContext _dbContext;

        public StoryController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
