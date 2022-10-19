using System.ComponentModel.DataAnnotations;

namespace BookStore.API.Models
{
    public class SignUp : SignIn
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }


        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

    }
}
