using System.ComponentModel.DataAnnotations;

namespace CommUnityHub.Models
{
    public class Offer
    {
        public int OfferId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }     // short name of the offer

        [Required]
        [StringLength(50)]
        public string Category { get; set; }  // e.g. Tutoring, Food, Housing

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(100)]
        public string Location { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Pending";  // Pending / Approved / Rejected
    }
}