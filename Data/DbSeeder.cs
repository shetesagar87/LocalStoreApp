using CleanMvcApp.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace CleanMvcApp.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Seed roles
            string[] roleNames = { "Admin", "StoreOwner", "Customer" };
            
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Seed Admin user
            await SeedUserAsync(userManager, new ApplicationUser
            {
                UserName = "admin@localstore.com",
                Email = "admin@localstore.com",
                FullName = "System Administrator",
                Address = "123 Admin Street, City Center",
                PhoneNumber = "+1234567890",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }, "Admin@123", "Admin");

            // Seed StoreOwner users
            await SeedUserAsync(userManager, new ApplicationUser
            {
                UserName = "owner1@localstore.com",
                Email = "owner1@localstore.com",
                FullName = "John Smith",
                Address = "456 Market Street, Downtown",
                PhoneNumber = "+1234567891",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }, "Owner@123", "StoreOwner");

            await SeedUserAsync(userManager, new ApplicationUser
            {
                UserName = "owner2@localstore.com",
                Email = "owner2@localstore.com",
                FullName = "Sarah Johnson",
                Address = "789 Commerce Ave, Business District",
                PhoneNumber = "+1234567892",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }, "Owner@123", "StoreOwner");

            // Seed Customer users
            await SeedUserAsync(userManager, new ApplicationUser
            {
                UserName = "customer1@localstore.com",
                Email = "customer1@localstore.com",
                FullName = "Michael Brown",
                Address = "321 Residential Lane, Suburb Area",
                PhoneNumber = "+1234567893",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }, "Customer@123", "Customer");

            await SeedUserAsync(userManager, new ApplicationUser
            {
                UserName = "customer2@localstore.com",
                Email = "customer2@localstore.com",
                FullName = "Emily Davis",
                Address = "654 Park View, Green Valley",
                PhoneNumber = "+1234567894",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }, "Customer@123", "Customer");

            await SeedUserAsync(userManager, new ApplicationUser
            {
                UserName = "customer3@localstore.com",
                Email = "customer3@localstore.com",
                FullName = "David Wilson",
                Address = "987 Lake Side Drive, Waterfront",
                PhoneNumber = "+1234567895",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }, "Customer@123", "Customer");
        }

        private static async Task SeedUserAsync(
            UserManager<ApplicationUser> userManager,
            ApplicationUser user,
            string password,
            string role)
        {
            var existingUser = await userManager.FindByEmailAsync(user.Email);

            if (existingUser == null)
            {
                var result = await userManager.CreateAsync(user, password);
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}
