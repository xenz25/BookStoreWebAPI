using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookStore.API.Models
{
    public class BookStoreContext : IdentityDbContext<ApplicationUsers>
    {
        public BookStoreContext(DbContextOptions<BookStoreContext> options) : 
            base(options)
        {
        }

        public DbSet<Books> Books { get; set; }
    }
}
