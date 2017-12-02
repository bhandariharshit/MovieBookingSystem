using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MovieBookingSystem.Data;
using Microsoft.AspNet.Identity;
using MovieBookingSystem.Models;

namespace MovieBookingSystem.Controllers
{
    public class MoviesBookingController : Controller
    {
        private MovieBookingDBEntities db = new MovieBookingDBEntities();

        // GET: MoviesBooking
        public ActionResult Index()
        {
            return View(db.Movies.Where(x=>x.IsActive==true).ToList());
        }

        // GET: MoviesBooking/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movy movy = db.Movies.Find(id);
            if (movy == null)
            {
                return HttpNotFound();
            }
            return View(movy);
        }

        // GET: MoviesBooking/Create
        public ActionResult Create()
        {
            string userId = User.Identity.GetUserId();
            var movieDetails = (from mb in db.MovieBookingDetails where mb.UserId == userId select new { mb.MovieId, mb.Movy.Name, mb.NoOfSeatsBooked }).ToList();
            List<MovieDetailModel> lstMovie = new List<MovieDetailModel>();

            var newList = movieDetails.GroupBy(x => x.Name).Select(grp => grp.ToList()).ToList();

            foreach (var item in newList)
            {
                MovieDetailModel model = new MovieDetailModel();
                model.Name = item.Select(x=>x.Name).FirstOrDefault();
                model.NoOfSeatsBooked = item.Select(x => x.NoOfSeatsBooked).FirstOrDefault();
                lstMovie.Add(model);
            }
            return View(lstMovie);

        }

        // POST: MoviesBooking/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,CreateDate,IsActive,FromDate,EndDate,TotalSeatCount,NoOfSeatsAvailable")] Movy movy)
        {
            if (ModelState.IsValid)
            {
                db.Movies.Add(movy);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(movy);
        }

        // GET: MoviesBooking/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movy movy = db.Movies.Find(id);
            MovieModel movieModel = new MovieModel();
            movieModel.Name = movy.Name;
            movieModel.NoOfSeatsAvailable = movy.NoOfSeatsAvailable;
            movieModel.Id = movy.Id;
            if (movy == null)
            {
                return HttpNotFound();
            }
            return View(movieModel);
        }

        // POST: MoviesBooking/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MovieModel movy)
        {
            try
            {
                if (ModelState.IsValid && User.Identity.GetUserId()!=null)
                {
                    // Use Transaction Here

                    Movy obj = (from m in db.Movies where m.Id == movy.Id select m).FirstOrDefault();
                    if (obj.NoOfSeatsAvailable >= movy.NoofSeatsTobeBooked && movy.NoofSeatsTobeBooked>0)
                    {
                        MovieBookingDetail objMovieBookingDetail = new MovieBookingDetail();
                        objMovieBookingDetail.MovieId = movy.Id;
                        objMovieBookingDetail.UserId = User.Identity.GetUserId();
                        objMovieBookingDetail.NoOfSeatsBooked = (int)movy.NoofSeatsTobeBooked;
                        db.Entry(objMovieBookingDetail).State = EntityState.Added;
                        obj.NoOfSeatsAvailable = obj.NoOfSeatsAvailable - movy.NoofSeatsTobeBooked;
                        db.Entry(obj).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ViewBag.Message = "Seats not available";
                        return View(obj);
                    }
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return View(movy);
        }

        // GET: MoviesBooking/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movy movy = db.Movies.Find(id);
            if (movy == null)
            {
                return HttpNotFound();
            }
            return View(movy);
        }

        // POST: MoviesBooking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Movy movy = db.Movies.Find(id);
            db.Movies.Remove(movy);
            db.SaveChanges();
            return RedirectToAction("Index");
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
