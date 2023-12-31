﻿using BlogApp.WebApi.Models;
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
        private readonly IApplicationDbContext _context;

        public BlogController(IApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Blog
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogDto>>> GetBlogs()
        {
            var blogs = await _context.Blogs
                .Include(b => b.User) // Include the User data
                .Select(b => new BlogDto
                {
                    BlogId = b.BlogId,
                    Title = b.Title,
                    Description = b.Description,
                    CreatedDate = b.CreatedDate,
                    Username = b.User.UserName 
                })
                .ToListAsync();

            return blogs;
        }

        // GET: api/Blog/5
[HttpGet("{id}")]
public async Task<ActionResult<BlogDto>> GetBlog(int id)
{
    var blog = await _context.Blogs
        .Include(b => b.User) // Include the User entity
        .Where(b => b.BlogId == id)
        .Select(b => new BlogDto
        {
            BlogId = b.BlogId,
            Title = b.Title,
            Description = b.Description,
            CreatedDate = b.CreatedDate,
            Username = b.User.UserName // Map the User's username
        })
        .FirstOrDefaultAsync();

    if (blog == null)
    {
        return NotFound();
    }

    return blog;
}

        // POST: api/Blog
        [HttpPost]
        public async Task<ActionResult<BlogDto>> PostBlog([FromBody] BlogCreateDto blogCreateDto)
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

            var createdBlogDto = new BlogDto
            {
                BlogId = blog.BlogId,
                Title = blog.Title,
                Description = blog.Description,
                CreatedDate = blog.CreatedDate
            };

            // return blog with url
            return CreatedAtAction("GetBlog", new { id = blog.BlogId }, createdBlogDto);
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
