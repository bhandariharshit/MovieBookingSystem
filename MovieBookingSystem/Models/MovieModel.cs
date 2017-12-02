using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MovieBookingSystem.Models
{
    public class MovieModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> TotalSeatCount { get; set; }
        [DisplayName("No of seats available")]
        public Nullable<int> NoOfSeatsAvailable { get; set; }
        [DisplayName("Enter No of seats to be booked")]
        [Required]
        public int? NoofSeatsTobeBooked { get; set; }
    }
}