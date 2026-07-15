using System.ComponentModel.DataAnnotations;

namespace IMS.Models.DTOs.Request
{
    public class CreateCategoryDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [MaxLength(250)]
        public string Description { get; set; }
    }
}
