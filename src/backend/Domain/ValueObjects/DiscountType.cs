using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public enum DiscountType
    {
        Percentage = 1,
        FixedAmount = 2,
        BonusDataMB = 3,
        BonusBalance = 4, 
        FreeService = 5
    }
}
