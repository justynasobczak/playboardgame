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

        public IActionResult List(int id)
        {
            if (id == 0)
            {
                return RedirectToAction(nameof(MeetingController.List), "Meeting");
            }

            var currentUserId = GetCurrentUserId().Result;
            var currentUser = _userManager.FindByIdAsync(currentUserId).Result;
            //var currentUserTimeZone = currentUser.TimeZone;
            //var timeZone = ToolsExtensions.ConvertTimeZone(currentUserTimeZone, _logger);
            var timeZone = currentUser.TimeZone;

            var messages = _messageRepository.GetMessagesForMeeting(id).ToList();
            foreach (var message in messages)
            {
                //message.Created = TimeZoneInfo.ConvertTimeFromUtc(message.Created, timeZone);
                message.Created = ToolsExtensions.ConvertToTimeZoneFromUtc(message.Created, timeZone, _logger);
                message.Author.Email = message.Author.Email.Trim().ToLower();
            }

            return View(new MessagesListViewModel {Messages = messages, MeetingId = id});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MessagesListViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var message = new Message
                {
                    Text = vm.Text,
                    Created = DateTime.UtcNow,
                    AuthorId = GetCurrentUserId().Result,
                    MeetingId = vm.MeetingId
                };
                TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
                _messageRepository.SaveMessage(message);
            }

            return RedirectToAction(nameof(List), new {id = vm.MeetingId});
        }

        [HttpPost]
        public IActionResult ShowMessage(int id)
        {
            var message = _messageRepository.GetMessage(id);
            var vm = new EditMessageViewModel {Text = message.Text, MessageId = message.MessageId};
            return PartialView("MessagePopup", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditMessageViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var message = new Message
                {
                    MessageId = vm.MessageId,
                    Text = vm.Text
                };
                TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
                _messageRepository.SaveMessage(message);
            }

            return RedirectToAction(nameof(List));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var deletedMessage = _messageRepository.DeleteMessage(id);
            if (deletedMessage != null)
            {
                TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
            }

            return RedirectToAction(nameof(List), new {id = deletedMessage.MeetingId});
        }

        private async Task<string> GetCurrentUserId()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            return user.Id;
        }
    }
}