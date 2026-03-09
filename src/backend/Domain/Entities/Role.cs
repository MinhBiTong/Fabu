using Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Role : EntityBase<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
