using BlogApp.WebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;

namespace BlogApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Comment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentDetailDto>>> GetComments()
        {
            var comments = await _context.Comments
                .Select(c => new CommentDetailDto
                {
                    CommentId = c.CommentId,
                    Text = c.Text,
                    Date = c.Date,
                    PostId = c.PostId,
                    UserName = c.User.UserName // Include UserName
                })
                .ToListAsync();

            return comments;
        }


        // GET: api/Comment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDetailDto>> GetComment(int id)
        {
            var comment = await _context.Comments
                .Where(c => c.CommentId == id)
                .Select(c => new CommentDetailDto
                {
                    CommentId = c.CommentId,
                    Text = c.Text,
                    Date = c.Date,
                    PostId = c.PostId,
                    UserName = c.User.UserName // Include UserName
                })
                .FirstOrDefaultAsync();

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }

        public class CommentCreateDto
        {
            public string Text { get; set; }
            public int PostId { get; set; }
            public string Username { get; set; }
        }

        // POST: api/Comment
        [HttpPost]
        public async Task<ActionResult<CommentDetailDto>> PostComment([FromBody] CommentCreateDto commentCreateDto)
        {
            var post = await _context.Posts.FindAsync(commentCreateDto.PostId);
            if (post == null)
            {
                return NotFound("Post not found.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == commentCreateDto.Username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var comment = new Comment
            {
                Text = commentCreateDto.Text,
                Date = DateTime.UtcNow,
                PostId = post.PostId,
                UserId = user.Id
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            var createdCommentDto = new CommentDetailDto
            {
                CommentId = comment.CommentId,
                Text = comment.Text,
                Date = comment.Date,
                PostId = comment.PostId,
                UserName = user.UserName // Assuming UserName property
                // Map other properties as needed
            };

            return CreatedAtAction("GetComment", new { id = comment.CommentId }, createdCommentDto);
        }



        // PUT: api/Comment/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(int id, Comment comment)
        {
            if (id != comment.CommentId)
            {
                return BadRequest();
            }

            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Comments.Any(e => e.CommentId == id))
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

        // DELETE: api/Comment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Comment/ByPost/5
        [HttpGet("ByPost/{postId}")]
        public async Task<ActionResult<IEnumerable<CommentDetailDto>>> GetCommentsByPostId(int postId)
        {
            var comments = await _context.Comments
                .Where(c => c.PostId == postId)
                .Select(c => new CommentDetailDto
                {
                    CommentId = c.CommentId,
                    Text = c.Text,
                    Date = c.Date,
                    PostId = c.PostId,
                    UserName = c.User.UserName // Include UserName
                })
                .ToListAsync();

            return comments;
        }

    }
}
