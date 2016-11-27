using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiTokenAuth.DAL;
using WebApiTokenAuth.Models;

namespace WebApiTokenAuth.Controllers
{
    public class AccountController : ApiController
    {
        private readonly AuthRepository _repository;

        public AccountController()
        {
            _repository = new AuthRepository();
        }

        [AllowAnonymous]
        public IHttpActionResult Register(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _repository.RegisterUser(user);

            if (result == false)
            {
                ModelState.AddModelError("Error", "something went wrong please try again later.");
                return BadRequest(ModelState);
            }

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
