using AutoMapper;
using CarRental.API.DTOs;
using CarRental.API.Entities;

namespace CarRental.API.Profiles.Resolvers
{
    public class PasswordResolver : IValueResolver<User, UserDTO, string>
    {
        public string Resolve(User source, UserDTO destination, string destMember, ResolutionContext context)
        {
            return new string('*', source.Password.Length);
        }
    }
}
