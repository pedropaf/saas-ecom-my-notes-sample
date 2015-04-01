using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MyNotes.Models;
using MyNotes.Views.SaasEcom.ViewModels;
using SaasEcom.Core.DataServices.Storage;
using SaasEcom.Core.Infrastructure.Facades;
using SaasEcom.Core.Infrastructure.PaymentProcessor.Stripe;

namespace MyNotes.Controllers
{
    public class NotesController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        private SubscriptionsFacade _subscriptionsFacade;
        private SubscriptionsFacade SubscriptionsFacade
        {
            get
            {
                return _subscriptionsFacade ?? (_subscriptionsFacade = new SubscriptionsFacade(
                    new SubscriptionDataService<ApplicationDbContext, ApplicationUser>
                        (HttpContext.GetOwinContext().Get<ApplicationDbContext>()),
                    new SubscriptionProvider(ConfigurationManager.AppSettings["StripeApiSecretKey"]),
                    new CardProvider(ConfigurationManager.AppSettings["StripeApiSecretKey"],
                        new CardDataService<ApplicationDbContext, ApplicationUser>(Request.GetOwinContext().Get<ApplicationDbContext>())),
                    new CardDataService<ApplicationDbContext, ApplicationUser>(Request.GetOwinContext().Get<ApplicationDbContext>()),
                    new CustomerProvider(ConfigurationManager.AppSettings["StripeApiSecretKey"]),
                    new SubscriptionPlanDataService<ApplicationDbContext, ApplicationUser>(Request.GetOwinContext().Get<ApplicationDbContext>()),
                    new ChargeProvider(ConfigurationManager.AppSettings["StripeApiSecretKey"])));
            }
        }

        // GET: Notes
        public async Task<ActionResult> Index()
        {
            var userNotes =
                await
                    _db.Users.Where(u => u.Id == User.Identity.GetUserId())
                        .Include(u => u.Notes)
                        .Select(u => u.Notes)
                        .ToListAsync();

            return View(userNotes);
        }

        // GET: Notes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ICollection<Note> userNotes = (await _db.Users.Where(u => u.Id == User.Identity.GetUserId()).Include(u => u.Notes).Select(u => u.Notes).FirstOrDefaultAsync());
            if (userNotes == null)
            {
                return HttpNotFound();
            }

            Note note = userNotes.FirstOrDefault(n => n.Id == id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        // GET: Notes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Notes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Title,Text,CreatedAt")] Note note)
        {
            if (ModelState.IsValid)
            {
                if (await UserHasEnoughSpace(User.Identity.GetUserId()))
                {
                    note.CreatedAt = DateTime.UtcNow;
                    _db.Notes.Add(note);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData.Add("flash", new FlashWarningViewModel("You can not add more notes, upgrade your subscription plan or delete some notes."));
                }
            }

            return View(note);
        }

        private async Task<bool> UserHasEnoughSpace(string userId)
        {
            var subscription = (await SubscriptionsFacade.UserActiveSubscriptionsAsync(userId)).FirstOrDefault();

            if (subscription == null)
            {
                return false;
            }

            var userNotes = await _db.Users.Where(u => u.Id == userId).Include(u => u.Notes).Select(u => u.Notes).CountAsync();

            return subscription.SubscriptionPlan.GetPropertyInt("MaxNotes") > userNotes;
        }

        // GET: Notes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!await NoteBelongToUser(User.Identity.GetUserId(), noteId: id.Value))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            Note note = await _db.Notes.FindAsync(id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        // POST: Notes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Text,CreatedAt")] Note note)
        {
            if (ModelState.IsValid && await NoteBelongToUser(User.Identity.GetUserId(), note.Id))
            {
                _db.Entry(note).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(note);
        }

        // GET: Notes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!await NoteBelongToUser(User.Identity.GetUserId(), noteId: id.Value))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Note note = await _db.Notes.FindAsync(id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            if (!await NoteBelongToUser(User.Identity.GetUserId(), noteId: id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Note note = await _db.Notes.FindAsync(id);
            _db.Notes.Remove(note);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        private async Task<bool> NoteBelongToUser(string userId, int noteId)
        {
            return await _db.Users.Where(u => u.Id == userId).Where(u => u.Notes.Any(n => n.Id == noteId)).AnyAsync();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
