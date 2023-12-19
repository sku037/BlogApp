using BlogApp.WebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace BlogApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        
        public PostController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/Post/{blogId}
        [HttpGet("ByBlogId/{blogId}")]
        public async Task<ActionResult<IEnumerable<PostDetailDto>>> GetPosts(int blogId)
        {
            var posts = await _context.Posts
                .Where(p => p.BlogId == blogId) // Filter by BlogId
                .Include(p => p.Blog) // Include the Blog
                .ThenInclude(b => b.User) // Include the User related to the Blog
                .Include(p => p.Tags) // Include the Tags
                .Select(p => new PostDetailDto
                {
                    PostId = p.PostId,
                    BlogId = p.BlogId,
                    PostTitle = p.PostTitle,
                    Content = p.Content,
                    PublishDate = p.PublishDate,
                    Username = p.Blog.User.UserName, // Map the username
                    Tags = p.Tags.Select(t => new TagDto { TagId = t.TagId, Name = t.Name }).ToList(), // Map Tags to TagDto
                    ImagePath = p.ImagePath
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
                .Include(p => p.Tags) // Include the Tags
                .Where(p => p.PostId == id)
                .Select(p => new PostDetailDto
                {
                    PostId = p.PostId,
                    BlogId = p.BlogId,
                    PostTitle = p.PostTitle,
                    Content = p.Content,
                    PublishDate = p.PublishDate,
                    Username = p.Blog.User.UserName,
                    Tags = p.Tags.Select(t => new TagDto { TagId = t.TagId, Name = t.Name }).ToList(), // Map Tags to TagDto
                    ImagePath = p.ImagePath
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

            // Create or find tags in the database
            var tags = new List<Tag>();
            if (postCreateDto.TagNames != null)
            {
                foreach (var tagName in postCreateDto.TagNames)
                {
                    var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                    if (tag == null)
                    {
                        tag = new Tag { Name = tagName };
                        _context.Tags.Add(tag);
                    }
                    tags.Add(tag);
                }
            }

            var post = new Post
            {
                PostTitle = postCreateDto.PostTitle,
                Content = postCreateDto.Content,
                PublishDate = DateTime.UtcNow,
                BlogId = blog.BlogId,
                Tags = tags,
                ImagePath = postCreateDto.ImagePath
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            var createdPostDto = new PostDetailDto
            {
                PostId = post.PostId,
                PostTitle = post.PostTitle,
                Content = post.Content,
                PublishDate = post.PublishDate,
                TagNames = post.Tags.Select(t => t.Name).ToList(),
                ImagePath = post.ImagePath
            };

            return CreatedAtAction("GetPost", new { id = post.PostId }, createdPostDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(int id, [FromBody] PostEditDto postEditDto)
        {
            var post = await _context.Posts
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.PostId == id);

            if (post == null)
            {
                return NotFound();
            }

            post.PostTitle = postEditDto.PostTitle;
            post.Content = postEditDto.Content;
            // Check if a new image path is provided and update it
            if (!string.IsNullOrEmpty(postEditDto.ImagePath))
            {
                post.ImagePath = postEditDto.ImagePath;
            }

            // Update tags
            if (postEditDto.TagNames != null)
            {
                var currentTagNames = post.Tags.Select(t => t.Name).ToList();
                var tagsToRemove = post.Tags.Where(t => !postEditDto.TagNames.Contains(t.Name)).ToList();

                foreach (var tag in tagsToRemove)
                {
                    post.Tags.Remove(tag);
                }

                foreach (var tagName in postEditDto.TagNames.Except(currentTagNames))
                {
                    var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                    if (tag == null)
                    {
                        tag = new Tag { Name = tagName };
                        await _context.Tags.AddAsync(tag);
                        await _context.SaveChangesAsync();
                    }

                    if (!post.Tags.Any(t => t.TagId == tag.TagId))
                    {
                        post.Tags.Add(tag);
                    }
                }
            }

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
            var post = await _context.Posts
                .Include(p => p.Comments) // Include comments
                .FirstOrDefaultAsync(p => p.PostId == id);

            if (post == null)
            {
                return NotFound();
            }

            // Remove comments if they exist
            if (post.Comments != null && post.Comments.Any())
            {
                _context.Comments.RemoveRange(post.Comments);
            }

            // Remove the post
            _context.Posts.Remove(post);

            // Save changes to the database
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

        // GET: api/Post/ByBlog/5/WithTags
        [HttpGet("ByBlogWithTags/{blogId}")]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetPostsByBlogIdWithTags(int blogId)
        {
            var posts = await _context.Posts
                .Where(p => p.BlogId == blogId)
                .Select(p => new PostDto
                {
                    PostId = p.PostId,
                    PostTitle = p.PostTitle,
                    PublishDate = p.PublishDate,
                    Tags = p.Tags.Select(t => new TagDto { TagId = t.TagId, Name = t.Name }).ToList()
                })
                .ToListAsync();

            return posts;
        }

        [HttpPost("upload")]
        public async Task<ActionResult<string>> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var imageUrl = Path.Combine("images", uniqueFileName); // Relative URL to image
                //var imageUrl = $"{this.Request.Scheme}://{this.Request.Host}/images/{uniqueFileName}";
                return Ok(imageUrl);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

    }
}
