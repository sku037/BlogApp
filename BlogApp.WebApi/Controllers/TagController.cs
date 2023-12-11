using BlogApp.WebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TagController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Tag
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetTags()
        {
            return await _context.Tags
                .Select(t => new TagDto
                {
                    TagId = t.TagId,
                    Name = t.Name
                })
                .ToListAsync();
        }

        // GET: api/Tag/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TagDto>> GetTag(int id)
        {
            var tag = await _context.Tags
                .Where(t => t.TagId == id)
                .Select(t => new TagDto
                {
                    TagId = t.TagId,
                    Name = t.Name
                    
                })
                .FirstOrDefaultAsync();

            if (tag == null)
            {
                return NotFound();
            }

            return tag;
        }

        // POST: api/Tag
        [HttpPost]
        public async Task<ActionResult<TagDto>> PostTag([FromBody] TagCreateDto tagCreateDto)
        {
            var tag = new Tag { Name = tagCreateDto.Name };
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTag", new { id = tag.TagId }, new TagDto { TagId = tag.TagId, Name = tag.Name });
        }


        // PUT: api/Tag/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTag(int id, Tag tag)
        {
            if (id != tag.TagId)
            {
                return BadRequest();
            }

            _context.Entry(tag).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Tags.Any(e => e.TagId == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Tag/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
