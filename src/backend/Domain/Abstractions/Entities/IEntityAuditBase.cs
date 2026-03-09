using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions.Entities
{
    public interface IEntityAuditBase<TKey> : IEntityBase<TKey>, IAuditable where TKey : struct
    {
    }
}
