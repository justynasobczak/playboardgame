using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PlayBoardGame.Controllers
{
    [Authorize]
    public class StartController : Controller
    {
        public ViewResult Index() => View();
    }
}