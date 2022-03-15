using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSOAuth.Business.Interface;
using SSOAuth.Models.VMs;

namespace SSOAuth.Controllers
{
    //[Route("User")]
    public class UserController : Controller
    {
        private IUserService _userService;
        private IHttpContextAccessor _httpContext;
        public UserController(IUserService userService, IHttpContextAccessor httpContext)
        {
            this._userService = userService;
            this._httpContext = httpContext;
        }
        // GET: User
        public async Task<ActionResult> Index()
        {
            
            

            var res = await _userService.GetAllUsers();
            return View(res);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserAccessVM user)
        {
            try
            {
                var res = await _userService.CreateUser(user.UserName, user.Password, user.Email, user.FirstName, user.LastName, 0);

                return RedirectToAction("Edit", new { id = res.Id});
            }
            catch
            {
                return View();
            }
        }

        // GET: User/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var res = await _userService.GetUser(id);
            return View(res);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: User/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: User/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}