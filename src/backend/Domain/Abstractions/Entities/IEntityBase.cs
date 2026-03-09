using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions.Entities
{
    public interface IEntityBase<TKey> where TKey : struct
    {
       TKey Id { get; set; }
    }
}
