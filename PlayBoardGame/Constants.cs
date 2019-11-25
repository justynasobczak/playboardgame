﻿using System.Collections.Generic;

namespace PlayBoardGame
{
    public static class Constants
    {
        //Messages
        public const string GeneralSuccessMessage = "The operation succeeded.";
        public const string GeneralSendEmailErrorMessage = "Email was not sent, please contact support.";
        public const string UserOrPasswordErrorMessage = "Invalid user or password";

        public const string SendResetLinkSuccessMessage =
            "Email with the link for resetting password was sent. Please check your inbox.";

        public const string LackOfEmailMatchMessage = "Entered email address doesn't match any account.";
        public const string WrongOldPasswordMessage = "Invalid old password";
        public const string NotValidTokenMessage = "Invalid token";
        public const string FutureDateInPastMessage = "Please enter a date in the future.";
        public const string EndDateBeforeStartMessage = "Please enter a date later than start date.";
        public const string OverlappingMeetingsMessage = "There are some overlapping meetings";
        public const string WrongDateTimeFormat = "Please enter a date in format: yyyy/MM/dd HH:mm";
        public const string AuthProviderError = "Error from external authenticate provider, please contact support.";
        public const string LoginByExternalProviderMessage = "You registered by using external provider like Google etc., " +
                                                             "please change your password on the page of your provider.";
        public const string WrongFileExtension = "Wrong file extension";
        public const string WrongFileSize = "The file is too big, please upload smaller one";
        public const string GameConnectedWithMeetingErrorMessage = "Cannot remove the game which is connected with the meeting";
        
        //Tooltips
        public const string DeleteInvitationTooltip = "Delete an invitation";
        public const string AcceptInvitationTooltip = "Accept an invitation";
        public const string RejectInvitationTooltip = "Decline";
        public const string CancelInvitationTooltip = "Cancel a meeting";
        public const string CopyAddressTooltip = "Copy an address from profile";
        public const string EditMessageTooltip = "Edit a message";
        public const string DeleteMessageTooltip = "Delete a message";
        public const string DeleteGameFromShelfTooltip = "Remove the game from shelf";
        public const string AddGameToShelfTooltip = "Put the game on the shelf";

        //Logs
        public const string UnknownId = "Unknown Id";
        public const string UnknownError = "Unknown error occurred";
        public const string AuthProviderKnownError = "Error from external provider";
        public const string AuthProviderUnknownError = "Error loading external login information";
        public const string AuthProviderUnknownEmailError = "Error from external provider: unknown email";
        public const string CannotRemoveFile = "Cannot remove uploaded file";

        //Email contents
        public const string SubjectRegistrationEmail = "Your new Gameet account is ready";
        public const string TitleRegistrationEmail = "Welcome to Gameet!";

        public const string ContentRegistrationEmail = "Thank you for creating an account with The Gameet. " +
                                                       "With your new account, you can organize the meeting with the board games" +
                                                       " and communicte with the players.";
        public const string ButtonVisitSide = "Visit our side";
        
        public const string SubjectResetPasswordEmail = "Reset your Gameet account password";
        public const string TitleResetPasswordEmail = "Reset password";

        public const string ContentResetPasswordEmail = "To reset your password click the link below: ";
        public const string ButtonResetPassword = "Reset password";
        
        public const string SubjectInviteUserEmail = "Invitation from Gameet account";
        public const string TitleInviteUserEmail = "Invitation";

        public const string ContentInviteUserEmail = "You are invited to the meeting with board games";

        //Consts
        public const string DateTimeFormat = "yyyy/MM/dd HH:mm";
        
        public const long FileSizeLimit = 2097152;

        public const int PageSize = 8;

        public static readonly Dictionary<string, string> Countries = new Dictionary<string, string>
        {
            {"AF", "Afghanistan"},
            {"AL", "Albania"},
            {"AE", "U.A.E."},
            {"AR", "Argentina"},
            {"AM", "Armenia"},
            {"AU", "Australia"},
            {"AT", "Austria"},
            {"AZ", "Azerbaijan"},
            {"BE", "Belgium"},
            {"BD", "Bangladesh"},
            {"BG", "Bulgaria"},
            {"BH", "Bahrain"},
            {"BA", "Bosnia and Herzegovina"},
            {"BY", "Belarus"},
            {"BZ", "Belize"},
            {"BO", "Bolivia"},
            {"BR", "Brazil"},
            {"BN", "Brunei Darussalam"},
            {"CA", "Canada"},
            {"CH", "Switzerland"},
            {"CL", "Chile"},
            {"CN", "People's Republic of China"},
            {"CO", "Colombia"},
            {"CR", "Costa Rica"},
            {"CZ", "Czech Republic"},
            {"DE", "Germany"},
            {"DK", "Denmark"},
            {"DO", "Dominican Republic"},
            {"DZ", "Algeria"},
            {"EC", "Ecuador"},
            {"EG", "Egypt"},
            {"ES", "Spain"},
            {"EE", "Estonia"},
            {"ET", "Ethiopia"},
            {"FI", "Finland"},
            {"FR", "France"},
            {"FO", "Faroe Islands"},
            {"GB", "United Kingdom"},
            {"GE", "Georgia"},
            {"GR", "Greece"},
            {"GL", "Greenland"},
            {"GT", "Guatemala"},
            {"HK", "Hong Kong S.A.R."},
            {"HN", "Honduras"},
            {"HR", "Croatia"},
            {"HU", "Hungary"},
            {"ID", "Indonesia"},
            {"IN", "India"},
            {"IE", "Ireland"},
            {"IR", "Iran"},
            {"IQ", "Iraq"},
            {"IS", "Iceland"},
            {"IL", "Israel"},
            {"IT", "Italy"},
            {"JM", "Jamaica"},
            {"JO", "Jordan"},
            {"JP", "Japan"},
            {"KZ", "Kazakhstan"},
            {"KE", "Kenya"},
            {"KG", "Kyrgyzstan"},
            {"KH", "Cambodia"},
            {"KR", "Korea"},
            {"KW", "Kuwait"},
            {"LA", "Lao P.D.R."},
            {"LB", "Lebanon"},
            {"LY", "Libya"},
            {"LI", "Liechtenstein"},
            {"LK", "Sri Lanka"},
            {"LT", "Lithuania"},
            {"LU", "Luxembourg"},
            {"LV", "Latvia"},
            {"MO", "Macao S.A.R."},
            {"MA", "Morocco"},
            {"MC", "Principality of Monaco"},
            {"MV", "Maldives"},
            {"MX", "Mexico"},
            {"MK", "Macedonia (FYROM)"},
            {"MT", "Malta"},
            {"ME", "Montenegro"},
            {"MN", "Mongolia"},
            {"MY", "Malaysia"},
            {"NG", "Nigeria"},
            {"NI", "Nicaragua"},
            {"NL", "Netherlands"},
            {"NO", "Norway"},
            {"NP", "Nepal"},
            {"NZ", "New Zealand"},
            {"OM", "Oman"},
            {"PK", "Islamic Republic of Pakistan"},
            {"PA", "Panama"},
            {"PE", "Peru"},
            {"PH", "Republic of the Philippines"},
            {"PL", "Poland"},
            {"PR", "Puerto Rico"},
            {"PT", "Portugal"},
            {"PY", "Paraguay"},
            {"QA", "Qatar"},
            {"RO", "Romania"},
            {"RU", "Russia"},
            {"RW", "Rwanda"},
            {"SA", "Saudi Arabia"},
            {"CS", "Serbia and Montenegro (Former)"},
            {"SN", "Senegal"},
            {"SG", "Singapore"},
            {"SV", "El Salvador"},
            {"RS", "Serbia"},
            {"SK", "Slovakia"},
            {"SI", "Slovenia"},
            {"SE", "Sweden"},
            {"SY", "Syria"},
            {"TJ", "Tajikistan"},
            {"TH", "Thailand"},
            {"TM", "Turkmenistan"},
            {"TT", "Trinidad and Tobago"},
            {"TN", "Tunisia"},
            {"TR", "Turkey"},
            {"TW", "Taiwan"},
            {"UA", "Ukraine"},
            {"UY", "Uruguay"},
            {"US", "United States"},
            {"UZ", "Uzbekistan"},
            {"VE", "Bolivarian Republic of Venezuela"},
            {"VN", "Vietnam"},
            {"YE", "Yemen"},
            {"ZA", "South Africa"},
            {"ZW", "Zimbabwe"}
        };
    }
}