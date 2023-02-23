using ECommerce.Models;
using ECommerce.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ECommerce.Controllers
{

    [Authorize(Roles = "Admin,Super Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;

        private UserManager<ApplicationUser> userManager;

        private readonly ILogger<AdministrationController> logger;



        public AdministrationController(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger<AdministrationController> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
        }


        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = userManager.Users;
            return View(users);
        }


        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                //ViewBag.ErrorMessage = $"Role with id = {id} cannot be found";
                return NotFound("User Not Found");
            }

            var userClaims = await userManager.GetClaimsAsync(user);
            var usersRoles = await userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Firstname = user.FName,
                Userame = user.UserName,
                Email = user.Email,
                Claims = userClaims.Select(c => c.Type+" : "+c.Value).ToList(),
                Roles = usersRoles

            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                //ViewBag.ErrorMessage = $"Role with id = {id} cannot be found";
                return NotFound("User Not Found");
            }

            else
            {
                user.FName = model.Firstname;
                user.UserName = model.Userame;
                user.Email = model.Email;

                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }


                return View(model);
            }
        }


        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound("User Not Found");
            }

            else
            {
                var result = await userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View();
            }
        }



        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }


            var model = new List<ManageRolesViewModel>();

            foreach (var roles in roleManager.Roles)
            {
                var mangeRolesViewModel = new ManageRolesViewModel
                {
                    RoleId = roles.Id,
                    RoleName = roles.Name
                };

                if (await userManager.IsInRoleAsync(user, roles.Name))
                {
                    mangeRolesViewModel.IsSelected = true;
                }
                else
                {
                    mangeRolesViewModel.IsSelected = false;
                }

                model.Add(mangeRolesViewModel);
            }

            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<ManageRolesViewModel> model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var roles = await userManager.GetRolesAsync(user);

            var result = await userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot Remove user Existing roles");
                return View(model);
            }

            result = await userManager.AddToRolesAsync(user, model.Where(x => x.IsSelected).Select(y => y.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot Remove user Existing roles");
                return View(model);
            }

            return RedirectToAction("EditUser", new { id = userId });

        }











        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }


        [Authorize(Policy = "AdminCreatePolicy")]
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                IdentityResult result = await roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administration");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }


        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                //ViewBag.ErrorMessage = $"Role with id = {id} cannot be found";
                return NotFound();
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);

        }



        [Authorize(Policy = "AdminEditPolicy")]
        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                //ViewBag.ErrorMessage = $"Role with id = {id} cannot be found";
                return NotFound();
            }

            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }


                return View(model);
            }
        }


        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound("User Not Found");
            }

            else
            {
                var result = await roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View();
            }
        }


        //[Authorize(Roles = "Super Admin")]
        [HttpGet]
        public async Task<IActionResult> EditUserInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return NotFound();
            }

            var model = new List<UserRoleViewModel>();

            foreach (var user in userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            return View(model);
        }


        //[Authorize(Roles = "Super Admin")]
        [HttpPost]
        public async Task<IActionResult> EditUserInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return NotFound();
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }

                else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }

                else
                {
                    continue;
                }


                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("EditRole", new { Id = roleId });
                    }
                }
            }


            return RedirectToAction("EditRole", new { Id = roleId });
        }




        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {     

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var existingUserClaims = await userManager.GetClaimsAsync(user);

            var model = new UserClaimsViewModel
            {
                UserId = userId
            };

            foreach (Claim claim in ClaimsStore.Allclaims)
            {
                UserClaims userClaims = new UserClaims
                {
                    ClaimType = claim.Type
                };

                if (existingUserClaims.Any(c => c.Type == claim.Type && c.Value=="true"))
                {
                    userClaims.IsSelected=true;
                }

                model.Claims.Add(userClaims);
            }

            return View(model);
        }





        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                return NotFound();
            }

            var claims = await userManager.GetClaimsAsync(user);

            var result = await userManager.RemoveClaimsAsync(user, claims);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot Remove user Existing Claims");
                return View(model);
            }

            result = await userManager.AddClaimsAsync(user, model.Claims.Select(c=> new Claim(c.ClaimType,c.IsSelected ? "true" : "false")));


            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot Remove user Existing roles");
                return View(model);
            }

            return RedirectToAction("EditUser", new { id = model.UserId
            });

        }

    }
}