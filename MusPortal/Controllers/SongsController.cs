using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MusPortal.Models;

namespace MusPortal.Controllers
{
    public class SongsController : Controller
    {
        private MusicPortalContext db = new MusicPortalContext();

        // GET: Songs
        public ActionResult Index()
        {
            var songs = db.Songs.Include(s => s.Genres);
            return View(songs.ToList());
        }

        // GET: Songs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Song song = db.Songs.Find(id);
            if (song == null)
            {
                return HttpNotFound();
            }
            return View(song);
        }

        // GET: Songs/Create
        public ActionResult Create()
        {
            ViewBag.GenreId = new SelectList(db.Genres, "Id", "Name");
            return View();
        }

        // POST: Songs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase fileUpload, Song song)
        {
            if (ModelState.IsValid)
            {
                if (fileUpload == null)
                {
                    ViewBag.IsFile = "Choose file!";
                }
                else
                {
                    string filename = Path.GetFileName(fileUpload.FileName);
                    song.Path = "~/Songsss/" + filename;
                    string tempfolder = Server.MapPath("~/Songsss");
                    if (filename != null)
                    {
                        fileUpload.SaveAs(Path.Combine(tempfolder, filename));
                    }
                    db.Songs.Add(song);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.GenreId = new SelectList(db.Genres, "Id", "Name", song.GenreId);
            return View(song);
        }

        // GET: Songs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Song song = db.Songs.Find(id);
            if (song == null)
            {
                return HttpNotFound();
            }
            ViewBag.GenreId = new SelectList(db.Genres, "Id", "Name", song.GenreId);
            return View(song);
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Path,GenreId")] Song song)
        {
            if (ModelState.IsValid)
            {
                db.Entry(song).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GenreId = new SelectList(db.Genres, "Id", "Name", song.GenreId);
            return View(song);
        }

        // GET: Songs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Song song = db.Songs.Find(id);
            if (song == null)
            {
                return HttpNotFound();
            }
            return View(song);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Song song = db.Songs.Find(id);
            db.Songs.Remove(song);
            db.SaveChanges();

            // если надо удалить все файлы
            //string[] filePaths = Directory.GetFiles(@"c:\MyDir\"); // передаем сюды пусть к папке с файлами "~/Songsss"
            //foreach (string filePath in filePaths)
            //    System.IO.File.Delete(filePath);

            FileInfo fi = new FileInfo(Server.MapPath(song.Path));
            fi.Delete();

            return RedirectToAction("Index");
        }

        // GET: Songs/Download
        [HttpGet]
        public FileResult DownloadFile(int? id)
        {
            if (id == null)
            {
                ViewBag.Message = "File not found";
            }
            Song song = db.Songs.Find(id);
            string path = Server.MapPath(song.Path); // get full path .mp3
            var fileName = Path.GetFileName(song.Path);
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);

            //var type = MimeMapping.GetMimeMapping(path); //audio/mpeg
            //return File(fileBytes, type, song.Name);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
