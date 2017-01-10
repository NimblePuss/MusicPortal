using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace MusPortal.Models
{
    public class MusicPortalDbInitializer : CreateDatabaseIfNotExists<MusicPortalContext>
    {
        protected override void Seed(MusicPortalContext db)
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
                salt + "12345" /* Пароль для хэширования */,
                "MD5" /* Используемый хэш-алгоритм */);

            db.Users.Add(new User {Name = "admin", Login = "admin", Password = hash, ConfirmPassword = hash, Email = "admin@gmail.com", Salt = salt, IsAdmin = true, IsUser = false});

            db.Genres.Add(new Genre {Name = "Pop"});
            db.Genres.Add(new Genre { Name = "Rock" });
            db.Genres.Add(new Genre { Name = "Rap" });

            base.Seed(db);
        }
    }
}