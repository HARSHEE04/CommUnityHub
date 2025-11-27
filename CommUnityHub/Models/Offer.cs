using System.ComponentModel.DataAnnotations;

namespace CommUnityHub.Models
{
    public class Offer
    {
        public int OfferId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string ContactEmail { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Location { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Pending";
    }
}
