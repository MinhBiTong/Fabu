using Domain.Entities;
using Domain.Repositories;
using Persistence.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class FeedbackRepository : BaseRepository<Feedback, int>, IFeedbackRepository
    {
        public FeedbackRepository(AppDbContext context) : base(context)
        {
        }

        public Task<IEnumerable<Feedback>> GetAllPendingAsync()
        {
            throw new NotImplementedException();
        }

        public Task MarkAsReadAsync(long feedbackId)
        {
            throw new NotImplementedException();
        }

        public Task MarkAsRepliedAsync(long feedbackId, string replyNote)
        {
            throw new NotImplementedException();
        }
    }
}
