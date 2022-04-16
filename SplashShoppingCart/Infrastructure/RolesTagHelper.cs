using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SplashShoppingCart.Models;

namespace SplashShoppingCart.Infrastructure
{
    [HtmlTargetElement("td", Attributes = "user-role")]
    public class RolesTagHelper : TagHelper
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<AppUser> userManager;

        public RolesTagHelper(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        [HtmlAttributeName("user-role")]
        public string RoleId { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            List<string> Names = new List<string>();
            //find the particular role with the id
            IdentityRole role = await roleManager.FindByIdAsync(RoleId);

            if(role != null)
            {
                //loop through all the users
                foreach (var user in userManager.Users)
                {
                    //check if users that are in this particular role
                    if (user != null && await userManager.IsInRoleAsync(user, role.Name))
                    {
                        //add the user to name for display purpose
                        Names.Add(user.UserName);
                    }
                }
            }
            output.Content.SetContent(Names.Count == 0 ? "No Users" : string.Join(",", Names));
        }
    }
}
