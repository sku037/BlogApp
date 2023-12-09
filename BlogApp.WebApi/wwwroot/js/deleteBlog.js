// deleteBlog.js
function deleteBlog(blogId) {
    $.ajax({
        url: 'https://localhost:7212/api/Blog/' + blogId,
        type: 'DELETE',
        headers: {
            "Authorization": "Bearer " + localStorage.getItem('jwt_token')
        },
        success: function (response) {
            loadBlogs();
        },
        error: function (xhr, status, error) {
            alert('Deleting blog failed: ' + error);
        }
    });
}
