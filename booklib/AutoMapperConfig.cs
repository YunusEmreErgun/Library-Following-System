using AutoMapper;
using booklib.Entities;
using booklib.Models;

namespace bookLib
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<User, UserModel>().ReverseMap();
            CreateMap<User, CreateUserModel>().ReverseMap();
            CreateMap<User, EditUserModel>().ReverseMap();
            CreateMap<Book, BookModel>().ReverseMap();
            CreateMap<Book, BookEditModel>().ReverseMap();
            CreateMap<Book, BookSearchModel>().ReverseMap();
            CreateMap<Book, BorrowBookModel>().ReverseMap();
            CreateMap<Lib, LibViewModel>().ReverseMap();
        }
    }
}