using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MusPortal.Models;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;


namespace MusPortal.Controllers
{
    public class RegisterController : Controller
    {
        private MusicPortalContext db = new MusicPortalContext();
        //
        // GET: /Register/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Register/LogIn
        public ActionResult LogIn()
        {
            ModelState.Clear();
            return View();
        }

        //
        // POST: /Register/LogIn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogIn(User user)
        {
            var login = db.Users.FirstOrDefault(log => log.Login == user.Login);

            if (login != null)
            {
                var salt = login.Salt;
                // Формирует хэшированный пароль, подходящий для хранения в файле конфигурации, 
                // в зависимости от указанного пароля и алгоритма хэширования.
                string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(
                    salt + user.Password /* Пароль для хэширования */,
                    "MD5" /* Используемый хэш-алгоритм */);
                var usr = db.Users.FirstOrDefault(u => u.Login == user.Login && u.Password == hash);
                if (usr != null)
                {
                    if (login.IsAdmin == true)
                    {
                        Session["UserName"] = usr.Name.ToString();
                        Session.Timeout = 60; // Длительность сеанса (тайм-аут завершения сеанса)
                        return RedirectToAction("Index", "Users");
                    }
                    if (login.IsUser == false)
                    {
                        ViewBag.Message = "You have successfully registered. Expect to activate administrator.";
                        Session["UserName"] = null;
                        ViewBag.IsUser = "";
                        return View("Index");
                    }
                    else
                    {
                        //Session["Id"] = usr.Id.ToString();
                        Session["UserName"] = usr.Name.ToString();
                        Session.Timeout = 60; // Длительность сеанса (тайм-аут завершения сеанса)

                        ViewBag.IsUser = "user";
                        return RedirectToAction("Index", "MusicPortal");
                    }
                }
                else
                {
                    //ViewBag.Message("", "Login or Password is wrong.");
                    ViewBag.Message = "Login or Password is wrong";
                    return View(user);
                }
            }
            ViewBag.Message = "Login or Password is wrong.";
            return View(user);
        }

        //
        // GET: /Register/SignUp
        public ActionResult SignUp()
        {
            return View();
        }

        //
        // POST: /Register/SignUp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(User user)
        {
            var login = db.Users.FirstOrDefault(log => log.Login == user.Login);
            if (login != null)
            {
                ViewBag.Message = "This login is used";
                return View(user);
            }
            if (ModelState.IsValid)
            {
                byte[] saltbuf = new byte[16];

                // Реализует криптографический генератор случайных чисел, используя реализацию, предоставляемую поставщиком служб шифрования (CSP). 
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetBytes(saltbuf);

                StringBuilder sb = new StringBuilder(16);
                for (int i = 0; i < 16; i++)
                    sb.Append(string.Format("{0:X2}", saltbuf[i]));
                string salt = sb.ToString();

                // Формирует хэшированный пароль, подходящий для хранения в файле конфигурации, в зависимости от указанного пароля и алгоритма хэширования.
                string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(
                    salt + user.Password /* Пароль для хэширования */,
                    "MD5" /* Используемый хэш-алгоритм */);

                user.Salt = salt;
                user.Password = hash;
                user.ConfirmPassword = hash;

                db.Users.Add(user);
                db.SaveChanges();
                ModelState.Clear();
                return RedirectToAction("LogIn", "Register");
            }
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}