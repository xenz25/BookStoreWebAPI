using AutoMapper;
using BookStore.API.Models;
using BookStore.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace BookStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public BooksController(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Books>>> GetAllBooks()
        {
            return await _bookRepository.GetAllBooksAsync();
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<IActionResult> GetBook(int id)
        {
            var book = await _bookRepository.GetBook(id);

            if(book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<IActionResult> SaveBook(BooksDTO book)
        {

            //Books bookCreated = new Books()
            //{
            //    Title = book.Title,
            //    Description = book.Description
            //};

            var bookCreated = _mapper.Map<Books>(book);

            var result = await _bookRepository.SaveBook(bookCreated);

            if (result > 0)
            {
                return CreatedAtAction(nameof(GetBook), new { id = bookCreated.Id }, bookCreated);
            }

            return Conflict();
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<IActionResult> UpdateBook([FromRoute] int id, [FromBody] Books book)
        {
            if(id != book.Id)
            {
                return BadRequest();
            }

            var result = await _bookRepository.UpdateBook(id, book);

            if(result > 0)
            {
                return NoContent();
            }

            return NotFound();
        }

        [HttpPatch("{id}")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<IActionResult> PatchBook(int id, JsonPatchDocument book)
        {
            var result = await _bookRepository.PatchBook(id, book);

            if(result > 0)
            {
                return NoContent();
            } 
            else if(result == -2)
            {
                return BadRequest();
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        [Produces("application/json")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        public async Task<IActionResult> DeleteBook([FromRoute] int id)
        {
            var result = await _bookRepository.DeleteBook(id);

            if (result > 0) return NoContent();

            return NotFound();
        }

    }
}
