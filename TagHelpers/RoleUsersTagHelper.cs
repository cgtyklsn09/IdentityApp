﻿using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace IdentityApp.TagHelpers
{
    [HtmlTargetElement("td", Attributes = "asp-role-users")]
    public class RoleUsersTagHelper : TagHelper
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public RoleUsersTagHelper(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HtmlAttributeName("asp-role-users")]
        public string RoleId { get; set; } = null!;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var userNames = new List<string>();
            var role = await _roleManager.FindByIdAsync(RoleId);

            if (role != null && role.Name != null)
            {
                foreach(var user in _userManager.Users)
                {
                    if(user.UserName != null && await _userManager.IsInRoleAsync(user, role.Name))
                        userNames.Add(user.UserName);                   
                }

                output.Content.SetHtmlContent(userNames.Count == 0 ? "No user": SetHtml(userNames));
            }
        }

        private string SetHtml(List<string> userNames)
        {
            var html = "<ul>";
            foreach (var user in userNames)
            {
                html += "<li>" + user + "</li>";
            }

            html += "</ul>";
            return html;
        }
    }
}
