using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication8.Models
{
    public class Movie
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Movie title ")]
        [StringLength(60, MinimumLength = 3)]
        public string Title { get; set; }

        [Display(Name = "Release date ")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [Required]
        [Display(Name = "Genre ")]
        [StringLength(30)]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        public string Genre { get; set; }

        [Display(Name = "Price ")]
        [Range(1, 100)]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        //[StringLength(5)]
        //[RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        //public string Rating { get; set; }
    }
}
