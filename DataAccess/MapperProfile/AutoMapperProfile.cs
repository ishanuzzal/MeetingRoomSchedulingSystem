using AutoMapper;
using DataAccess.Entities;
using DataAccess.ViewModel.admin;
using DataAccess.ViewModel.Booking;
using DataAccess.ViewModel.Room;
using DataAccess.ViewModel.User;
using Microsoft.AspNetCore.Routing.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.MapperProfile
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile() {
            //CreateMap<src,dest>
            CreateMap<AddRoomView, Room>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.RoomName));

            CreateMap<AddRoomView, Room>().ReverseMap()
                .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.Name));

            CreateMap<UpdateRoomView, Room>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.RoomName))
                .ForMember(dest => dest.ImageName, opt => opt.Ignore());

            CreateMap<UpdateRoomView, Room>().ReverseMap()
                .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.Name));

            CreateMap<ShowRoomView, Room>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.RoomName)).ReverseMap();

            CreateMap<ShowRoomView, UpdateRoomView>();

            CreateMap<AppUser, UsersMailApiView>();

            CreateMap<AddBookingAdminView, Booking>();

            CreateMap<AddBookingUserView, Booking>();

            CreateMap<Booking, ShowBookingView>();

            CreateMap<Booking, UpdateBookingView>();

            CreateMap<Booking, PendingBookingApprovalView>();   

            CreateMap<Booking,BookingAllDetailsView>();

            CreateMap<Booking, AddBookingAdminView>();

            CreateMap<AppUser, AllUsersView>();

            CreateMap<Booking,AUserBookingView>();  

            CreateMap<PasswordResetRequest,AdminPasswordResetView>();

            CreateMap<PasswordResetRequest, AdminPasswordResetView>().ReverseMap();
        }
    }
}
