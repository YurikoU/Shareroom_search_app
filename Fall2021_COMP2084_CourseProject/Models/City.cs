﻿using System.ComponentModel.DataAnnotations;

namespace Fall2021_COMP2084_CourseProject.Models
{
    public class City
    {
        public int Id { get; set; }//Primary key

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a new city name.")]
        [MaxLength(50, ErrorMessage = "A city name must be up to 50 characters.")]
        [Display(Name = "City Name")]
        public string Name { get; set; }
    }
}
