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

        //Logs
        public const string UnknownId = "Unknown Id";
        public const string UnknownError = "Unknown error occurred";

        //Consts
        public const string DateTimeFormat = "yyyy/MM/dd HH:mm";
        
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