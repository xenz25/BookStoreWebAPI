using System.ComponentModel.DataAnnotations;

namespace BookStore.API.Models
{
    public class BooksDTO
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
