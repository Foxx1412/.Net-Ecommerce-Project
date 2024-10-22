using Microsoft.EntityFrameworkCore;
using Project_1.Core.Entities;
using Project_1.Core.Interfaces;
using Project_1.Infrastructure.Data;

namespace Project_1.Infrastructure.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext _context;

        public TagRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync(int pageNumber, int pageSize)
        {
            return await _context.Tags
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Tag> GetTagByIdAsync(int id)
        {
            return await _context.Tags
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddTagAsync(Tag tag)
        {
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTagAsync(Tag tag)
        {
            _context.Entry(tag).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTagAsync(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag != null)
            {
                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();
            }
        }
    }
}
