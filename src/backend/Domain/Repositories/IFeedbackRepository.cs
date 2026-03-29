using Domain.Abstractions.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IFeedbackRepository : IRepositoryBase<Feedback, int>
    {
        Task<IEnumerable<Feedback>> GetAllPendingAsync(); //chua xy ly status = pending
        Task<Feedback> GetByIdAsync(long id);
        Task MarkAsReadAsync(long feedbackId);
        Task MarkAsRepliedAsync(long feedbackId, string replyNote);
    }
}
