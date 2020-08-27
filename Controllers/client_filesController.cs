namespace demo.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;

    using demo.Models;

    using Microsoft.AspNet.Identity;

    [Authorize]
    public class client_filesController : Controller
    {
        private readonly demoEntities db = new demoEntities();

        // GET: client_files/Create
        public ActionResult Create()
        {
            return this.View();
        }

        // POST: client_files/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase[] postedFile)
        {
            var users = this.db.client_data.ToList();
            var userid = users.Find(x => x.AspNetUser.Id == this.User.Identity.GetUserId());
            if (userid != null)
            {
                foreach (var postedFileBase in postedFile)
                {
                    var postedFileExtension = Path.GetExtension(postedFileBase.FileName);
                if (string.Equals(postedFileExtension, ".jpg", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(postedFileExtension, ".png", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(postedFileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(postedFileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase) 
                    || string.Equals(postedFileExtension,".pdf",StringComparison.OrdinalIgnoreCase))
                {
                    byte[] bytes;
                    using (var br = new BinaryReader(postedFileBase.InputStream))
                    {
                        bytes = br.ReadBytes(postedFileBase.ContentLength);
                    }

                    this.db.client_files.Add(
                        new client_files
                            {
                                name = Path.GetFileName(postedFileBase.FileName),
                                content_type = postedFileBase.ContentType,
                                data = bytes,
                                user_id = userid.Id,
                                checked_file = false
                            });
                }

                this.db.SaveChanges();
                }
                
                return this.RedirectToAction("Index");
            }

            return this.View(postedFile);
        }

        // GET: client_files/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var client_files = this.db.client_files.Find(id);
            if (client_files == null) return this.HttpNotFound();
            return this.View(client_files);
        }

        // POST: client_files/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var client_files = this.db.client_files.Find(id);
            this.db.client_files.Remove(client_files);
            this.db.SaveChanges();
            return this.RedirectToAction("Index");
        }

        // GET: client_files/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var client_files = this.db.client_files.Find(id);
            if (client_files == null) return this.HttpNotFound();
            return this.View(client_files);
        }

        // GET: client_files/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var client_files = this.db.client_files.Find(id);
            if (client_files == null) return this.HttpNotFound();
            this.ViewBag.user_id = new SelectList(this.db.client_data, "Id", "first_name", client_files.user_id);
            return this.View(client_files);
        }

        // POST: client_files/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "Id,name,content_type,data,user_id,checked_file")]
            client_files client_files)
        {
            if (this.ModelState.IsValid)
            {
                this.db.Entry(client_files).State = EntityState.Modified;
                this.db.SaveChanges();
                return this.RedirectToAction("Index");
            }

            this.ViewBag.user_id = new SelectList(this.db.client_data, "Id", "first_name", client_files.user_id);
            return this.View(client_files);
        }

        // GET: client_files
        public ActionResult Index()
        {
            var users = this.db.client_data.ToList();
            var userid = users.Find(x => x.AspNetUser.Id == this.User.Identity.GetUserId());
            var client_files = new List<client_files>();
            var client_files1 = new List<client_files>();
            var client_files2 = new List<client_files>();
            if (userid != null)
            {
                 if (User.IsInRole("client"))
                 {
                     client_files = this.db.client_files.Include(c => c.client_data).ToList();
                     client_files1 = client_files.FindAll(x => x.user_id == userid.Id);
                     var i = 0;
                     foreach (var q2 in client_files1)
                     {
                        q2.Id = ++i;
                        client_files2.Add(q2);
                     }
                     return this.View(client_files2);
                 }
                 if (User.IsInRole("Admin") || User.IsInRole("delivery"))
                 {
                     client_files = this.db.client_files.Include(c => c.client_data).ToList();
                     var i = 0;
                     foreach (var q1 in client_files)
                     {
                         q1.Id = ++i;
                        client_files2.Add(q1);
                     }
                     return this.View(client_files2);
                 }
            }
            return this.View(client_files);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) this.db.Dispose();
            base.Dispose(disposing);
        }
    }
}