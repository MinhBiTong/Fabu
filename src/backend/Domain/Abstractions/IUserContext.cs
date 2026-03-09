using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions
{
    public interface IUserContext
    {
        string? UserId { get; }
        string UserName { get; }
        bool IsAuthenticated { get; }
    }
}
