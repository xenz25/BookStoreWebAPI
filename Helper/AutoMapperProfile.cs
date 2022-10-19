using AutoMapper;
using BookStore.API.Models;

namespace BookStore.API.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Books, BooksDTO>().ReverseMap();
        }
    }
}
