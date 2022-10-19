using BookStore.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookStore.API.Services
{
    public interface IBookRepository
    {
        public Task<List<Books>> GetAllBooksAsync();

        public Task<Books> GetBook(int id);

        public Task<int> SaveBook(Books book);

        public Task<int> UpdateBook(int id, Books book);

        public Task<int> PatchBook(int id, JsonPatchDocument book);

        public Task<int> DeleteBook(int id);
    }

    public class BookRepository : IBookRepository
    {
        private readonly BookStoreContext _context;

        public BookRepository(BookStoreContext context)
        {
            _context = context;
        }

        public async Task<List<Books>> GetAllBooksAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<Books> GetBook(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task<int> SaveBook(Books book)
        {
            _context.Books.Add(book);

            var result = await _context.SaveChangesAsync();

            return result;
        }

        public async Task<int> UpdateBook(int id, Books book)
        {
            try
            {
                #region METHOD 1
                //var retrievedBook = await this.GetBook(id);

                //if (retrievedBook == null)
                //{
                //    throw new Exception();
                //}

                //retrievedBook.Title = book.Title;
                //retrievedBook.Description = book.Description;

                //var result = await _context.SaveChangesAsync();
                #endregion

                #region METHOD 2
                _context.Update(book);
                var result = await _context.SaveChangesAsync();
                #endregion

                if (result > 0) return result;

                throw new Exception();
            }
            catch(Exception)
            {
                return -1;
            }
        }

        // TODO: Create a model for return to make it more uniform
        public async Task<int> PatchBook(int id, JsonPatchDocument book)
        {
            try
            {
                var retrievedBook = await _context.Books.FindAsync(id);
                if (retrievedBook == null)
                {
                    return -1;
                }

                book.ApplyTo(retrievedBook);
                var result = await _context.SaveChangesAsync();

                if (result > 0) return result;

                throw new Exception();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return -2;
            }
        }

        public async Task<int> DeleteBook(int id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    throw new Exception();
                }

                _context.Remove(book);
                var result = await _context.SaveChangesAsync();

                if (result > 0) return result;

                throw new Exception();
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public bool IsBookExist(int id)
        {
            return _context.Books.Any(book => book.Id == id);
        }
    }
}
