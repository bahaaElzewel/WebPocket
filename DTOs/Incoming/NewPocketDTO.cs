using System.ComponentModel.DataAnnotations;

namespace WebPocket.DTOs.Incoming
{
    public class NewPocketDTO
    {
        [Required(ErrorMessage = "PocketName is required")]
        public string PocketName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}