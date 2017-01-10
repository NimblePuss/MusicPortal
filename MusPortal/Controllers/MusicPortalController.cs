using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MusPortal.Models;

namespace MusPortal.Controllers
{
    public class MusicPortalController : Controller
    {
        private MusicPortalContext db = new MusicPortalContext();
        // GET: MusicPortal
        public ActionResult Index()
        {
            var songs = db.Songs.Include(s => s.Genres);
            return View(songs.ToList());
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
        // GET: Exit
        [HttpGet]
        public ActionResult Exit()
        {
            Session.Abandon(); //exit
            ViewBag.IsUser = "";
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

    }
}