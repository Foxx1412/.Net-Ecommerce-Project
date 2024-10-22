using Project_1.Core.Entities;

namespace Project_1.Core.Interfaces
{
    public interface ITagRepository
    {
        Task<IEnumerable<Tag>> GetAllTagsAsync(int pageNumber, int pageSize);
        Task<Tag> GetTagByIdAsync(int id);
        Task AddTagAsync(Tag tag);
        Task UpdateTagAsync(Tag tag);
        Task DeleteTagAsync(int id);
    }
}
