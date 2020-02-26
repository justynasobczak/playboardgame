using Microsoft.AspNetCore.Razor.TagHelpers;
using PlayBoardGame.Models;

namespace PlayBoardGame.Infrastructure
{
    [HtmlTargetElement("a", Attributes = "invitation-bubble")]
    public class InvitationBubbleTagHelper : TagHelper
    {
        private readonly IFriendInvitationRepository _friendInvitationRepository;

        public InvitationBubbleTagHelper(IFriendInvitationRepository friendInvitationRepository)
        {
            _friendInvitationRepository = friendInvitationRepository;
        }

        [HtmlAttributeName("invitation-bubble")]
        public string Name { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            const string menuItem = " Friend requests ";
            var number = _friendInvitationRepository.GetNumberOfPendingInvitationsForCurrentUser(Name);
            output.Content.AppendHtml(number < 1
                ? $"<span>{menuItem}</span>"
                : $"<span>{menuItem}</span><span class=\"numberCircle\">{number.ToString()}</span>");
        }
    }
}