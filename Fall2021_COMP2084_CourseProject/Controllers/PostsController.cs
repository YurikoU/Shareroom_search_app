﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fall2021_COMP2084_CourseProject.Data;
using Fall2021_COMP2084_CourseProject.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using Microsoft.AspNetCore.Authorization;
/*
GET : Before clicking the submit button
POST: After submitting the form
*/

namespace Fall2021_COMP2084_CourseProject.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            //Process the database connection
            _context = context;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            //Access the parent table so the view can automatically show the category name from the parent table
            var applicationDbContext = _context.Posts.Include(p => p.City).OrderBy(p => p.PostedDate);

            //Retrieve the data and convert it to the list
            return View(await applicationDbContext.ToListAsync());
        }


        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return View("404");
            }

            var post = await _context.Posts
                .Include(p => p.City)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return View("404");
            }

            return View(post);
        }

        //Only the logged in users and administrator can accesses
        [Authorize]
        // GET: Posts/Create
        public IActionResult Create()
        {
            //Get the list of Cities and store them in ViewData
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name");
            return View();
        }

        //Only the logged in users and administrator can accesses
        [Authorize]
        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PostedDate,Rent,Description,PhoneOnPost,EmailOnPost,UserId,CityId")] Post post, IFormFile Photo)
        {
            //Check if the input is valid
            if (ModelState.IsValid)
            {
                //Once an user uploads a photo, save it
                if (Photo != null)
                {
                    var fileName = UploadPhoto(Photo);

                    //Set the photo property name of the new Post object as the unique name
                    post.Photo = fileName;
                }

                //UserId is the user's email address to log in
                post.UserId = User.Identity.Name;

                //Add the Post object
                _context.Add(post);

                //PostedDate is today's date
                post.PostedDate = System.DateTime.Now;

                //Save the changes on the database connection
                await _context.SaveChangesAsync();

                //Once done, it will redirect the user to the index page
                return RedirectToAction(nameof(Index));
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", post.CityId);
            return View(post);
        }


        private static string UploadPhoto(IFormFile Photo)
        {
            //Temporary file location for an uploaded photo
            var filePath = Path.GetTempFileName();

            //Generate a unique name adding GUID, so it doesn't overwrite the existing photo data
            var fileName = Guid.NewGuid() + "-" + Photo.FileName;

            //Set the destination path dynamically to work both locally and on Azure
            var uploadPath = System.IO.Directory.GetCurrentDirectory() + "\\wwwroot\\img\\posts\\" + fileName;

            //Copy the file image and save it into "img" folder
            using (var fileStream = new FileStream(uploadPath, FileMode.Create))
            {
                Photo.CopyTo(fileStream);
            }

            return fileName;
        }


        //Only the logged in users and administrator can accesses
        [Authorize]
        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return View("404");
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return View("404");
            }
            //Get the list of Cities and store them in ViewData
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", post.CityId);
            return View(post);
        }

        //Only the logged in users and administrator can accesses
        [Authorize]
        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PostedDate,Rent,Description,PhoneOnPost,EmailOnPost,UserId,CityId")] Post post, IFormFile Photo, string CurrentPhoto)
        {
            if (id != post.Id)
            {
                return View("404");
            }

            //Check if the input is valid
            if (ModelState.IsValid)
            {
                try
                {
                    if (Photo != null)
                    {   
                        //If there is an uploaded photo, store it into the Photo property
                        var fileName = UploadPhoto(Photo);
                        post.Photo = fileName;
                    }
                    else
                    {
                        //Keep the existing photo unless there is no new one uploaded
                        post.Photo = CurrentPhoto;
                    }

                    //Update the Post object
                    _context.Update(post);

                    //Save the changes on the database connection
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return View("404");
                    }
                    else
                    {
                        throw;
                    }
                }

                //Once done, it will redirect the user to the index page
                return RedirectToAction(nameof(Index));
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", post.CityId);
            return View(post);
        }

        //Only the logged in users and administrator can accesses
        [Authorize]
        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return View("404");
            }

            var post = await _context.Posts
                .Include(p => p.City)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return View("404");
            }

            return View(post);
        }

        //Only the logged in users and administrator can accesses
        [Authorize]
        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
