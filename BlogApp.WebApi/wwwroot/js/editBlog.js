// editBlog.js
$(document).ready(function () {
    // Assuming you have an edit form in your HTML (hidden initially)
    // You might need to adjust selectors based on your HTML structure
    $('#edit-blog-form').on('submit', function (e) {
        e.preventDefault();
        var blogId = $('#edit-blog-id').val(); // Hidden input field with blogId
        var title = $('#edit-blog-title').val();
        var description = $('#edit-blog-description').val();
        editBlog(blogId, title, description);
    });
});

function editBlog(blogId, title, description) {
    $.ajax({
        url: 'https://localhost:7212/api/Blog/' + blogId,
        type: 'PUT',
        headers: {
            "Authorization": "Bearer " + localStorage.getItem('jwt_token')
        },
        contentType: 'application/json',
        data: JSON.stringify({ title: title, description: description }),
        success: function (response) {
            // Hide the edit form and reload the list of blogs
            $('#edit-blog-form').hide();
            loadBlogs();
        },
        error: function (xhr, status, error) {
            alert('Editing blog failed: ' + error);
        }
    });
}

// Function to show the edit form with the blog details
function showEditForm(blogId) {
    // Fetch the blog details from the server or from the existing row in the table
    // Populate the edit form fields
    $('#edit-blog-id').val(blogId);
    // For example, if you have the blog title and description in the table row
    var blogRow = $('#blog-row-' + blogId); // You should assign this id in blog.js
    $('#edit-blog-title').val(blogRow.find('.blog-title').text());
    $('#edit-blog-description').val(blogRow.find('.blog-description').text());

    // Show the edit form
    $('#edit-blog-form').show();
}
