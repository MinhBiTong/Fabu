using Application.Interfaces;
using Domain.Abstractions;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class PostpaidBillService : IPostpaidBillService
    {
        private readonly IUnitOfWork _unitOfWork;

        public async Task UpdateBillStatusAfterPayment(long billId, decimal paidAmount)
        {
            var bill = await _unitOfWork.PostpaidBills.GetByIdAsync(billId);
            if (bill == null) return;

            bill.PaidAmount += paidAmount;

            if (bill.PaidAmount >= bill.TotalAmount)
                bill.Status = StatusPostpaid.Paid;
            else if (bill.PaidAmount > 0)
                bill.Status = StatusPostpaid.Partial;

            // Không cần gọi Update, UoW.CommitAsync sẽ tự xử lý
        }
    }
}
