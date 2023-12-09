// login.js
$(document).ready(function () {
    $('#show-login').on('click', function () {
        $('#loginModal').show();
    });

    $('#login-form').on('submit', function (e) {
        e.preventDefault();
        var username = $('#username').val();
        var password = $('#password').val();
        login(username, password);
    });
});

function login(username, password) {
    $.ajax({
        url: 'https://localhost:7212/api/Auth/login',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ username: username, password: password }),
        success: function (response) {
            localStorage.setItem('jwt_token', response.token);
            $('#loginModal').hide();
            $('#create-blog').show();
        },
        error: function (xhr, status, error) {
            alert('Login failed: ' + error);
        }
    });
}
