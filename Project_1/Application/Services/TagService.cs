using Project_1.Core.Entities;
using Project_1.Core.Interfaces;

namespace Project_1.Application.Services
{
    public class TagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync(int pageNumber, int pageSize)
        {
            return await _tagRepository.GetAllTagsAsync(pageNumber, pageSize);
        }

        public async Task<Tag> GetTagByIdAsync(int id)
        {
            return await _tagRepository.GetTagByIdAsync(id);
        }

        public async Task AddTagAsync(Tag tag)
        {
            await _tagRepository.AddTagAsync(tag);
        }

        public async Task UpdateTagAsync(Tag tag)
        {
            await _tagRepository.UpdateTagAsync(tag);
        }

        public async Task<bool> DeleteTagAsync(int id)
        {
            var tag = await _tagRepository.GetTagByIdAsync(id);
            if (tag == null)
            {
                return false;
            }

            await _tagRepository.DeleteTagAsync(id);
            return true;
        }
    }
}
