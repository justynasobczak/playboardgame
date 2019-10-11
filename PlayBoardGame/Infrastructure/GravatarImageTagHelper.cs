using System;
using System.Security.Cryptography;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PlayBoardGame.Infrastructure
{
    [HtmlTargetElement("img", Attributes = "gravatar-email")]
    public class GravatarImageTagHelper : TagHelper
    {
        [HtmlAttributeName("gravatar-email")] public string Email { get; set; }
        [HtmlAttributeName("gravatar-mode")] private Mode Mode { get; } = Mode.Robohash;
        [HtmlAttributeName("gravatar-rating")] private Rating Rating { get; } = Rating.g;
        [HtmlAttributeName("gravatar-size")] private int Size { get; } = 50;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(Email));
                var hash = BitConverter.ToString(result).Replace("-", "").ToLower();
                var url = $"https://www.gravatar.com/avatar/{hash}";
                var queryBuilder = new QueryBuilder
                {
                    {"s", Size.ToString()}, {"d", GetModeValue(Mode)}, {"r", Rating.ToString()}
                };
                url += queryBuilder.ToQueryString();
                output.Attributes.SetAttribute("src", url);
            }
        }

        private static string GetModeValue(Mode mode)
        {
            return mode == Mode.NotFound ? "404" : mode.ToString().ToLower();
        }
    }

    public enum Rating
    {
        g, //suitable for display on all websites with any audience type
        pg,
        r,
        x
    }

    public enum Mode
    {
        [Display(Name = "404")] NotFound,
        [Display(Name = "Mp")] Mp,
        [Display(Name = "Identicon")] Identicon,
        [Display(Name = "Monsterid")] Monsterid,
        [Display(Name = "Wavatar")] Wavatar,
        [Display(Name = "Retro")] Retro,
        [Display(Name = "Robohash")] Robohash,
        [Display(Name = "Blank")] Blank
    }
}