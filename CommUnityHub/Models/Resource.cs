using System.ComponentModel.DataAnnotations;

namespace CommUnityHub.Models
{

    //class developed by Harsheta Sharma
    //This class represents the Resource object for each resource in the database from the EF core model


    public class Resource
    {
        [Required]
        public string Name { get; set; }


        public string Description { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }


        [Required]
        public string City { get; set; }

        [Required]
        public string HoursOfOperation { get; set; }

        [Required]
        public string Category { get; set; }

        [Url]
        public string Webiste { get; set; }

    }
}
