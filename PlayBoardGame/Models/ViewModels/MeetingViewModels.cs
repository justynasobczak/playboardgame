using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PlayBoardGame.Models.ViewModels
{
    public class MeetingViewModels
    {
        public class CreateEditMeetingViewModel
        {
            public int MeetingID { get; set; }

            [Required]
            public string Title { get; set; }
            
            [Display(Name = "Start date and time")]
            public DateTime StartDateTime { get; set; }
        
            [Display(Name = "End date and time")]
            public DateTime EndDateTime { get; set; }
            
            public string OrganizerId { get; set; }
            
            public string Notes { get; set; }
            
            public bool IsEditable { get; set; }
            
            public List<AppUser> Organizers { get; set; }
            
            public AddressViewModels Address { get; set; }
        }
    }
}