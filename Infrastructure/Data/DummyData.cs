using Domain;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class DummyData
    {
        public static async Task Initialize(ApplicationDbContext context, RoleManager<ApplicationRole> roleManager)
        {
            context.Database.EnsureCreated();

            //String adminId1 = "";
            //String adminId2 = "";
            //string password = "Qw123$";

            string role1 = "Business";
            string desc1 = "This is the business role";

            string role2 = "Member";
            string desc2 = "This is the members role";

            if (await roleManager.FindByNameAsync(role1) == null)
            {
                await roleManager.CreateAsync(new ApplicationRole(role1, desc1, DateTime.Now));
            }
            if (await roleManager.FindByNameAsync(role2) == null)
            {
                await roleManager.CreateAsync(new ApplicationRole(role2, desc2, DateTime.Now));
            }
        }
    }
}
