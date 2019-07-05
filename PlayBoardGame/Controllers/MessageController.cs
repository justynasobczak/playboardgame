using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlayBoardGame.Infrastructure;
using PlayBoardGame.Models;
using PlayBoardGame.Models.ViewModels;

namespace PlayBoardGame.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly IMessageRepository _messageRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<MessageController> _logger;

        public MessageController(IMessageRepository messageRepository, UserManager<AppUser> userManager,
            ILogger<MessageController> logger)
        {
            _messageRepository = messageRepository;
            _userManager = userManager;
            _logger = logger;
        }
        public IActionResult List()
        {
            var currentUserId = GetCurrentUserId().Result;
            var currentUser = _userManager.FindByIdAsync(currentUserId).Result;
            var currentUserTimeZone = currentUser.TimeZone;
            var timeZone = ToolsExtensions.ConvertTimeZone(currentUserTimeZone, _logger);
 
            var messages = GetMessagesWithDates(_messageRepository.Messages, timeZone);
            return View(new MessagesListViewModel { Messages = messages});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MessagesListViewModel vm)
        {
            var message = new Message
            {
                Text = vm.Text,
                Created = DateTime.UtcNow,
                AuthorId = GetCurrentUserId().Result
            };
            _messageRepository.SaveMessage(message);
            return RedirectToAction(nameof(List));
        }
        
        private async Task<string> GetCurrentUserId()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            return user.Id;
        }
        
        private IQueryable<Message> GetMessagesWithDates(IQueryable<Message> messages, TimeZoneInfo timeZone)
        {
            foreach (var message in messages)
            {
                message.Created = TimeZoneInfo.ConvertTimeFromUtc(message.Created, timeZone);
            }
            return messages;
        }
    }
}