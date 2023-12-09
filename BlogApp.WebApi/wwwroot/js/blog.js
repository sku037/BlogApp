// blog.js
$(document).ready(function () {
    loadBlogs();
});

function loadBlogs() {
    $.ajax({
        url: 'https://localhost:7212/api/Blog',
        type: 'GET',
        dataType: 'json',
        success: function (blogs) {
            var blogsTable = $('#blogs');
            blogsTable.empty();
            blogs.forEach(function (blog) {
                var blogRow = $('<tr>').append(
                    $('<td>').text(blog.title),
                    $('<td>').text(blog.description),
                    $('<td>').append(
                        $('<button>').text('Edit').on('click', function () {
                            editBlog(blog.blogId);
                        }),
                        $('<button>').text('Delete').on('click', function () {
                            deleteBlog(blog.blogId);
                        })
                    )
                );
                blogsTable.append(blogRow);
            });
        },
        error: function () {
            alert('Error loading blogs');
        }
    });
}
