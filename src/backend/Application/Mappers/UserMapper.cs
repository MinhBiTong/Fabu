using Domain.Entities;
using Persistence.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers
{
    public static class UserMapper
    {
        public static User ToDomain(ApplicationUser user)
        {
            return new User
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName
            };
        }
    }
}
