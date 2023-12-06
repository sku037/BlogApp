using BlogApp.WebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Post
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            return await _context.Posts
                                 .Include(p => p.Comments)
                                 .Include(p => p.Tags)
                                 .ToListAsync();
        }

        // GET: api/Post/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _context.Posts
                                     .Include(p => p.Comments)
                                     .Include(p => p.Tags)
                                     .FirstOrDefaultAsync(p => p.PostId == id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        // POST: api/Post
        [HttpPost]
        public async Task<ActionResult<Post>> PostPost([FromBody] PostCreateDto postCreateDto)
        {
            var blog = await _context.Blogs.FindAsync(postCreateDto.BlogId);
            if (blog == null)
            {
                return NotFound("Blog not found.");
            }

            var post = new Post
            {
                Content = postCreateDto.Content,
                PublishDate = DateTime.UtcNow,
                BlogId = blog.BlogId,
                // ititiate Comments и Tags
                Comments = new List<Comment>(),
                Tags = new List<Tag>()
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPost", new { id = post.PostId }, post);
        }

        // PostCreateDto
        public class PostCreateDto
        {
            public string Content { get; set; }
            public int BlogId { get; set; }
        }

        // PostEditDto
        public class PostEditDto
        {
            public string Content { get; set; }
        }

        // PUT: api/Post/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(int id, [FromBody] PostEditDto postEditDto)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            post.Content = postEditDto.Content;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Posts.Any(e => e.PostId == id))
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

        // DELETE: api/Post/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("ByBlog/{blogId}")] 
        public async Task<ActionResult<IEnumerable<Post>>> GetPostsByBlogId(int blogId)
        {
            var posts = await _context.Posts
                .Where(p => p.BlogId == blogId)
                .ToListAsync();

            return posts;
        }
    }
}
