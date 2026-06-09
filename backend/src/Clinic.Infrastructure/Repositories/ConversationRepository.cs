using Clinic.Application.Interfaces;
using Clinic.Domain;
using Clinic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Repositories
{
    public class ConversationRepository(AppDbContext db) : IConversationRepository
    {
        public async Task<Conversation> AddAsync(Conversation conversation, CancellationToken ct = default)
        {
            db.Conversations.Add(conversation);
            await db.SaveChangesAsync(ct);
            return conversation;
        }

        public async Task<Conversation> UpdateAsync(Conversation conversation, CancellationToken ct = default)
        {
            db.Conversations.Update(conversation);
            await db.SaveChangesAsync(ct);
            return conversation;
        }

        public async Task<Conversation?> GetByIdWithMessagesAsync(int id, CancellationToken ct = default)
            => await db.Conversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == id, ct);
        public async Task<IReadOnlyList<Conversation>> GetAllAsync(CancellationToken ct = default)
            => await db.Conversations.AsNoTracking()
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(ct);
        public async Task<Conversation?> GetByIdAsync(int id, CancellationToken ct = default)
            => await db.Conversations.FindAsync([id], ct);
    }
}
