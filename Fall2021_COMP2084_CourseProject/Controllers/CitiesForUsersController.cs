﻿using Fall2021_COMP2084_CourseProject.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fall2021_COMP2084_CourseProject.Controllers
{
    public class CitiesForUsersController : Controller
    {
        //Add DBContext object to use the database
        private readonly ApplicationDbContext _context;

        //Constructor that takes an instance of our DB connection object
        public CitiesForUsersController(ApplicationDbContext context)
        {
            //Assign the incoming DB connection so we can use it in any method in this controller
            _context = context;
        }

        public IActionResult Index()
        {
            //Use Cities Dbset to fetch list of cities to display to users
            var cities = _context.Cities.OrderBy(c => c.Name).ToList(); //Convert the  query into the list   

            return View(cities);
        }
    }
}