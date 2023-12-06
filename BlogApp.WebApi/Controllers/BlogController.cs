using BlogApp.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BlogApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BlogController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Blog
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Blog>>> GetBlogs()
        {
            return await _context.Blogs.ToListAsync();
        }

        // GET: api/Blog/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Blog>> GetBlog(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);

            if (blog == null)
            {
                return NotFound();
            }

            return blog;
        }

        // POST: api/Blog
        [HttpPost]
        public async Task<ActionResult<Blog>> PostBlog([FromBody] BlogCreateDto blogCreateDto)
        {
            // Find User.
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == blogCreateDto.Username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Convert blogCreateDto to Blog.
            var blog = new Blog
            {
                Title = blogCreateDto.Title,
                Description = blogCreateDto.Description,
                CreatedDate = DateTime.UtcNow, 
                UserId = user.Id, 
                Posts = new List<Post>() 
            };

            // add new blog.
            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();

            // return blog with url
            return CreatedAtAction("GetBlog", new { id = blog.BlogId }, blog);
        }

        // DTO.
        public class BlogCreateDto
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string Username { get; set; }
        }
        public class BlogEditDto
        {
            public string Title { get; set; }
            public string Description { get; set; }
        }



        // PUT: api/Blog/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBlog(int id, [FromBody] BlogEditDto blogEditDto)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            blog.Title = blogEditDto.Title;
            blog.Description = blogEditDto.Description;

            _context.Entry(blog).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // DELETE: api/Blog/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.BlogId == id);
        }
    }
}
