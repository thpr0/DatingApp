using System.Linq;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
            public AutoMapperProfiles(){
                CreateMap<User,UserForListDto>()
                .ForMember(dest=>dest.PhotoUrl,opt=>{
                    opt.MapFrom(src=>src.Photos.FirstOrDefault(p=>p.IsMain).Url);
                }).ForMember(dest=>dest.Age,opt=>{
                    opt.ResolveUsing(d=>d.DateOfbirth.CalculateAge());
                });
                CreateMap<User,UserForDetailDto>().ForMember(dest=>dest.PhotoUrl,opt=>{
                    opt.MapFrom(src=>src.Photos.FirstOrDefault(p=>p.IsMain).Url);
                }).ForMember(dest=>dest.Age,opt=>{
                    opt.ResolveUsing(d=>d.DateOfbirth.CalculateAge());});
                CreateMap<Photo,PhotosForDetailDto>();
                CreateMap<UserForUpdateDto,User>();
                CreateMap<Photo,PhotoToReturnDto>();
                CreateMap<PhotoForCreationDto,Photo>();
                CreateMap<UserForRegisterDto,User>();
                CreateMap<MessageForCreationDto,Message>().ReverseMap();
            CreateMap<Message, MessageToReturnDto>();
            }
    }
}