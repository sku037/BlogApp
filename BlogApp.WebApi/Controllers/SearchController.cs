using BlogApp.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BlogApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // GET: api/Search/posts-by-tag?tag={tag}
        [HttpGet("posts-by-tag")]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetPostsByTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                return BadRequest("Tag is required.");
            }

            var postsWithSpecifiedTag = await _context.Posts
                .Include(post => post.Tags) 
                .Where(post => post.Tags.Any(t => t.Name == tag))
                .Select(post => new PostDto
                {
                    PostId = post.PostId,
                    PostTitle = post.PostTitle,
                    BlogId = post.BlogId 
                    
                })
                .ToListAsync();

            if (postsWithSpecifiedTag == null || !postsWithSpecifiedTag.Any())
            {
                return NotFound("No posts found with the specified tag.");
            }

            return postsWithSpecifiedTag;
        }

        [HttpGet("posts-by-user")]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetPostsByUser(string userName)
        {
            var userPosts = await _context.Blogs
                .Where(blog => blog.User.UserName.Contains(userName)) // Filter by username
                .SelectMany(blog => blog.Posts, (blog, post) => new { blog, post }) // Get all posts for the user
                .Select(bp => new PostDto
                {
                    PostId = bp.post.PostId,
                    PostTitle = bp.post.PostTitle,
                    BlogId = bp.blog.BlogId
                })
                .ToListAsync();

            if (!userPosts.Any())
            {
                return NotFound("No posts found for the specified user.");
            }

            return userPosts;
        }

    }
}