using Microsoft.AspNetCore.Mvc;

namespace PlayBoardGame.Controllers
{
    public class ErrorController : Controller
    {
        public ViewResult Error() => View();
    }
}
