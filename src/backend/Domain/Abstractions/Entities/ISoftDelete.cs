using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions.Entities
{
    public interface ISoftDelete
    {
        public bool IsDeleted { get; set; }

        public DateTimeOffset DeletedAt { get; set; }

        public void MarkAsDeleted();
    }
}
