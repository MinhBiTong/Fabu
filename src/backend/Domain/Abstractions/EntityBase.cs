using Domain.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions
{
    public abstract class EntityBase<TKey> : IEntityBase<TKey> where TKey : struct
    {
        public TKey Id { get; set; }
    }
}
