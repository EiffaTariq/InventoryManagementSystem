using IMS.Enums;
using System.ComponentModel.DataAnnotations;

namespace IMS.Models.DTOs.Request
{
    public class UpdateStatusDto
    {
        [Required]
        public POStatus NewStatus { get; set; }
    }
}