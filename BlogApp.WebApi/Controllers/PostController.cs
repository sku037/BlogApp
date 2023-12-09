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
        public async Task<ActionResult<IEnumerable<PostDto>>> GetPosts()
        {
            var posts = await _context.Posts
                .Select(p => new PostDto
                {
                    PostId = p.PostId,
                    PostTitle = p.PostTitle,
                    PublishDate = p.PublishDate
                    // Map other properties as needed
                })
                .ToListAsync();

            return posts;
        }

        // GET: api/Post/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostDetailDto>> GetPost(int id)
        {
            var post = await _context.Posts
                .Include(p => p.Blog) // Include the Blog
                .ThenInclude(b => b.User) // Include the User related to the Blog
                .Where(p => p.PostId == id)
                .Select(p => new PostDetailDto
                {
                    PostId = p.PostId,
                    PostTitle = p.PostTitle,
                    Content = p.Content,
                    PublishDate = p.PublishDate,
                    Username = p.Blog.User.UserName 
                })
                .FirstOrDefaultAsync();

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }


        // POST: api/Post
        [HttpPost]
        public async Task<ActionResult<PostDetailDto>> PostPost([FromBody] PostCreateDto postCreateDto)
        {
            var blog = await _context.Blogs.FindAsync(postCreateDto.BlogId);
            if (blog == null)
            {
                return NotFound("Blog not found.");
            }

            var post = new Post
            {
                PostTitle = postCreateDto.PostTitle,
                Content = postCreateDto.Content,
                PublishDate = DateTime.UtcNow,
                BlogId = blog.BlogId,
                // ititiate Comments и Tags
                Comments = new List<Comment>(),
                Tags = new List<Tag>()
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            var createdPostDto = new PostDetailDto
            {
                PostId = post.PostId,
                PostTitle = post.PostTitle,
                Content = post.Content,
                PublishDate = post.PublishDate
                // Map other properties as needed
            };

            return CreatedAtAction("GetPost", new { id = post.PostId }, createdPostDto);
        }

        // PostCreateDto
        public class PostCreateDto
        {
            public string PostTitle { get; set; }
            public string Content { get; set; }
            public int BlogId { get; set; }
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

            post.PostTitle = postEditDto.PostTitle;
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

        // PostEditDto
        public class PostEditDto
        {
            public string PostTitle { get; set; }
            public string Content { get; set; }
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

        // GET: api/Post/ByBlog/5
        [HttpGet("ByBlog/{blogId}")]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetPostsByBlogId(int blogId)
        {
            var posts = await _context.Posts
                .Where(p => p.BlogId == blogId)
                .Select(p => new PostDto
                {
                    PostId = p.PostId,
                    PostTitle = p.PostTitle,
                    PublishDate = p.PublishDate
                    // Map other properties as needed
                })
                .ToListAsync();

            return posts;
        }
    }
}
