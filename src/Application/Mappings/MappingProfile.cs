using AutoMapper;
using MCQSystem.Application.DTOs;
using MCQSystem.Domain.Entities;

namespace MCQSystem.Application.Mappings;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<Exam, ExamDto>().ReverseMap();
        CreateMap<QuestionOption, QuestionOptionDto>().ReverseMap();
        CreateMap<Question, QuestionDto>().ReverseMap();
        CreateMap<ApplicationUser, UserDto>();
        CreateMap<Result, ResultDto>();
    }
}
