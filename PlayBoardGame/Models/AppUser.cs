using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PlayBoardGame.Models
{
    public class AppUser : IdentityUser

    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string PostalCode { get; set; }

        public Country Country { get; set; }
        
        public string TimeZone { get; set; }

        public ICollection<GameAppUser> GameAppUser { get; } = new List<GameAppUser>();
        
        public ICollection<Meeting> OrganizedMeetings { get; set; }
        
        public ICollection<MeetingInvitedUser> MeetingInvitedUser { get; set; }
    }
    
    public enum Country
    {
        None,
        Afghanistan,
        [Display(Name = "Åland Islands")]
        AlandIslands,
        Albania,
        Algeria,
        [Display(Name = "American Samoa")]
        AmericanSamoa,
        Andorra,
        Angola,
        Anguilla,
        Antarctica,
        [Display(Name = "Antigua and Barbuda")]
        AntiguaandBarbuda,
        Argentina,
        Armenia,
        Aruba,
        Australia,
        Austria,
        Azerbaijan,
        Bahrain,
        Bangladesh,
        Barbados,
        Belarus,
        Belgium,
        Belize,
        Benin,
        Bermuda,
        Bhutan,
        [Display(Name = "Bolivia (Plurinational State of)")]
        BoliviaPlurinationalStateof,
        [Display(Name = "Bosnia and Herzegovina")]
        BosniaandHerzegovina,
        Botswana,
        [Display(Name = "Bouvet Island")]
        BouvetIsland,
        Brazil,
        [Display(Name = "British Indian Ocean Territory")]
        BritishIndianOceanTerritory,
        [Display(Name = "British Virgin Islands")]
        BritishVirginIslands,
        [Display(Name = "Brunei Darussalam")]
        BruneiDarussalam,
        Bulgaria,
        [Display(Name = "Burkina Faso")]
        BurkinaFaso,
        Burundi,
        [Display(Name = "Cabo Verde")]
        CaboVerde,
        Cambodia,
        Cameroon,
        Canada,
        [Display(Name = "Cayman Islands")]
        CaymanIslands,
        [Display(Name = "Central African Republic")]
        CentralAfricanRepublic,
        Chad,
        Chile,
        China,
        [Display(Name = "Christmas Island")]
        ChristmasIsland,
        [Display(Name = "Cocos (Keeling) Islands")]
        CocosKeelingIslands,
        Colombia,
        Comoros,
        Congo,
        [Display(Name = "Democratic Republic of the Congo")]
        DemocraticRepublicoftheCongo,
        [Display(Name = "Cook Islands")]
        CookIslands,
        [Display(Name = "Costa Rica")]
        CostaRica,
        [Display(Name = "Côte d'Ivoire")]
        CôtedIvoire,
        Croatia,
        Cuba,
        Curaçao,
        Cyprus,
        Czechia,
        Denmark,
        Djibouti,
        Dominica,
        [Display(Name = "Dominican Republic")]
        DominicanRepublic,
        Ecuador,
        Egypt,
        [Display(Name = "El Salvador")]
        ElSalvador,
        [Display(Name = "Equatorial Guinea")]
        EquatorialGuinea,
        Eritrea,
        Estonia,
        Eswatini,
        Ethiopia,
        [Display(Name = "Falkland Islands (Malvinas)")]
        FalklandIslandsMalvinas,
        [Display(Name = "Faroe Islands")]
        FaroeIslands,
        Fiji,
        Finland,
        France,
        [Display(Name = "French Guiana")]
        FrenchGuiana,
        [Display(Name = "French Polynesia")]
        FrenchPolynesia,
        [Display(Name = "French Southern Territories")]
        FrenchSouthernTerritories,
        Gabon,
        Gambia,
        Georgia,
        Germany,
        Ghana,
        Gibraltar,
        Greece,
        Greenland,
        Grenada,
        Guadeloupe,
        Guam,
        Guatemala,
        Guernsey,
        Guinea,
        [Display(Name = "Guinea-Bissau")]
        GuineaBissau,
        Guyana,
        Haiti,
        [Display(Name = "Heard Island and McDonald Islands")]
        HeardIslandandMcDonaldIslands,
        [Display(Name = "Holy See")]
        HolySee,
        Honduras,
        [Display(Name = "Hong Kong")]
        HongKong,
        Hungary,
        Iceland,
        India,
        Indonesia,
        [Display(Name = "Iran (Islamic Republic of Iran")]
        Iran,
        Iraq,
        Ireland,
        [Display(Name = "Isle of Man")]
        IsleofMan,
        Israel,
        Italy,
        Jamaica,
        Japan,
        Jersey,
        Jordan,
        Kazakhstan,
        Kenya,
        Kiribati,
        [Display(Name = "Korea (Democratic People's Republic of Korea")]
        NorthKorea,
        [Display(Name = "Korea, Republic of Korea")]
        SouthKorea,
        Kuwait,
        Kyrgyzstan,
        Laos,
        [Display(Name = "Lao People's Democratic Republic")]
        Lao,
        Latvia,
        Lebanon,
        Lesotho,
        Liberia,
        Libya,
        Liechtenstein,
        Lithuania,
        Luxembourg,
        Macao,
        Madagascar,
        Malawi,
        Malaysia,
        Maldives,
        Mali,
        Malta,
        [Display(Name = "Marshall Islands")]
        MarshallIslands,
        Martinique,
        Mauritania,
        Mauritius,
        Mayotte,
        Mexico,
        [Display(Name = "Micronesia (Federated States of Micronesia)")]
        Micronesia,
        [Display(Name = "Moldova, Republic of Moldova")]
        Moldova,
        Monaco,
        Mongolia,
        Montenegro,
        Montserrat,
        Morocco,
        Mozambique,
        Myanmar,
        Namibia,
        Nauru,
        Nepal,
        Netherlands,
        [Display(Name = "New Caledonia")]
        NewCaledonia,
        [Display(Name = "New Zealand")]
        NewZealand,
        Nicaragua,
        Niger,
        Nigeria,
        Niue,
        [Display(Name = "Norfolk Island")]
        NorfolkIsland,
        [Display(Name = "North Macedonia")]
        NorthMacedonia,
        [Display(Name = "Northern Mariana Islands")]
        NorthernMarianaIslands,
        Norway,
        Oman,
        Pakistan,
        Palau,
        [Display(Name = "Palestine, State of Palestine")]
        Palestine,
        Panama,
        [Display(Name = "Papua New Guinea")]
        PapuaNewGuinea,
        Paraguay,
        Peru,
        Philippines,
        Pitcairn,
        Poland,
        Portugal,
        [Display(Name = "Puerto Rico")]
        PuertoRico,
        Qatar,
        Réunion,
        Romania,
        [Display(Name = "Russian Federation")]
        RussianFederation,
        Rwanda,
        [Display(Name = "Saint Barthélemy")]
        SaintBarthélemy,
        [Display(Name = "Saint Helena, Ascension and Tristan da Cunha")]
        SaintHelenaAscensionandTristandaCunha,
        [Display(Name = "Saint Kitts and Nevis")]
        SaintKittsandNevis,
        [Display(Name = "Saint Lucia")]
        SaintLucia,
        [Display(Name = "Saint Martin (French part)")]
        SaintMartinFrenchpart,
        [Display(Name = "Saint Pierre and Miquelon")]
        SaintPierreandMiquelon,
        [Display(Name = "Saint Vincent and the Grenadines")]
        SaintVincentandtheGrenadines,
        Samoa,
        [Display(Name = "San Marino")]
        SanMarino,
        [Display(Name = "Sao Tome and Principe")]
        SaoTomeandPrincipe,
        [Display(Name = "Saudi Arabia")]
        SaudiArabia,
        Senegal,
        Serbia,
        Seychelles,
        [Display(Name = "Sierra Leone")]
        SierraLeone,
        Singapore,
        [Display(Name = "Sint Maarten (Dutch part)")]
        SintMaartenDutchpart,
        Slovakia,
        Slovenia,
        [Display(Name = "Solomon Islands")]
        SolomonIslands,
        Somalia,
        [Display(Name = "South Africa")]
        SouthAfrica,
        [Display(Name = "South Georgia and the South Sandwich Islands")]
        SouthGeorgiaandtheSouthSandwichIslands,
        [Display(Name = "South Sudan")]
        SouthSudan,
        Spain,
        [Display(Name = "Sri Lanka")]
        SriLanka,
        Sudan,
        Suriname,
        [Display(Name = "Svalbard and Jan Mayen")]
        SvalbardandJanMayen,
        Sweden,
        Switzerland,
        [Display(Name = "Syrian Arab Republic")]
        SyrianArabRepublic,
        [Display(Name = "Taiwan, Province of China")]
        Taiwan,
        Tajikistan,
        [Display(Name = "Tanzania, United Republic of Tanzania")]
        Tanzania,
        Thailand,
        [Display(Name = "Timor-Leste")]
        TimorLeste,
        Togo,
        Tokelau,
        Tonga,
        [Display(Name = "Trinidad and Tobago")]
        TrinidadandTobago,
        Tunisia,
        Turkey,
        Turkmenistan,
        [Display(Name = "Turks and Caicos Islands")]
        TurksandCaicosIslands,
        Tuvalu,
        Uganda,
        Ukraine,
        [Display(Name = "United Arab Emirates")]
        UnitedArabEmirates,
        [Display(Name = "United Kingdom of Great Britain and Northern Ireland")]
        UnitedKingdomofGreatBritainandNorthernIreland,
        [Display(Name = "United States of America")]
        UnitedStatesofAmerica,
        [Display(Name = "United States Minor Outlying Islands")]
        UnitedStatesMinorOutlyingIslands,
        Uruguay,
        Uzbekistan,
        Vanuatu,
        [Display(Name = "Venezuela (Bolivarian Republic of Venezuela)")]
        Venezuela,
        [Display(Name = "Viet Nam")]
        VietNam,
        [Display(Name = "Virgin Islands (British")]
        VirginIslandsBritish,
        [Display(Name = "Virgin Islands Virgin Islands (U.S.)")]
        VirginIslandsUS,
        WallisandFutuna,
        [Display(Name = "Western Sahara")]
        WesternSahara,
        Yemen,
        Zambia,
        Zimbabwe
    }
}
