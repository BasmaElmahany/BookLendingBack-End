using AutoMapper;
using BookLendingBackUp.Application.DTOs;
using BookLendingBackUp.Domain.Entities;
using BookLendingBackUp.Infrastructure.Entities;


namespace BookLendingBackUp.Infrastructure.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
          //  CreateMap<RegisterUserDto, ApplicationUser>();

            CreateMap<RegisterUserDto, ApplicationUser>()
    .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
    .ForMember(dest => dest.DesiredRole, opt => opt.MapFrom(src => src.Role));

            CreateMap<CatalogDTO, Catalog>()
    .ForMember(dest => dest.Books, opt => opt.Ignore());
            CreateMap<Catalog, CatalogDTO>();

            CreateMap<BookDTO, Book>();

            CreateMap<Book, BookDTO>();

        }
    }
}
