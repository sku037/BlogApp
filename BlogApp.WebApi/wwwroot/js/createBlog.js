// createBlog.js
$(document).ready(function () {
    $('#create-blog').on('click', function () {
        $('#create-blog-form').show();
    });

    $('#blog-form').on('submit', function (e) {
        e.preventDefault();
        var title = $('#blog-title').val();
        var description = $('#blog-description').val();
        createBlog(title, description);
    });
});

function createBlog(title, description) {
    $.ajax({
        url: 'https://localhost:7212/api/Blog',
        type: 'POST',
        headers: {
            "Authorization": "Bearer " + localStorage.getItem('jwt_token')
        },
        contentType: 'application/json',
        data: JSON.stringify({ title: title, description: description }),
        success: function (response) {
            $('#create-blog-form').hide();
            loadBlogs();
        },
        error: function (xhr, status, error) {
            alert('Creating blog failed: ' + error);
        }
    });
}
