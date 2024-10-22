using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_1.Application.Services;
using Project_1.Core.Entities;

namespace Project_1.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly TagService _tagService;

        public TagController(TagService tagService)
        {
            _tagService = tagService;
           
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTags(int pageNumber = 1, int pageSize = 10)
        {
            var tags = await _tagService.GetAllTagsAsync(pageNumber, pageSize);
            return Ok(tags);
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<Tag>> GetTagById(int id)
        {
            var tag = await _tagService.GetTagByIdAsync(id);
            if (tag == null)
            {
                return NotFound();
            }
            return Ok(tag);
        }


        [HttpPost]
        public async Task<IActionResult> PostTag(Tag tag)
        {
            await _tagService.AddTagAsync(tag);
         
            return Ok(new { success = true, message = "Tag added successfully." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTag(int id, Tag tag)
        {
            if (id != tag.Id)
            {
                return BadRequest();
            }

            await _tagService.UpdateTagAsync(tag);
            return Ok(new { success = true, message = "Tag updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var isDeleted = await _tagService.DeleteTagAsync(id);

            if (!isDeleted)
            {
                return NotFound(new { Message = $"Tag với ID {id} không tồn tại." });
            }

            return Ok(new { Message = "Tag đã được xóa thành công." });
        }
    }
}
