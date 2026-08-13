using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

string? GetConnectionString()
{
    return Environment.GetEnvironmentVariable(
        "SQLAZURECONNSTR_AZURE_SQL_CONNECTIONSTRING");
}

string HashPassword(string password)
{
    using var sha256 = SHA256.Create();
    var bytes = Encoding.UTF8.GetBytes(password);
    var hash = sha256.ComputeHash(bytes);
    return Convert.ToHexString(hash);
}

bool VerifyPassword(string password, string storedHash)
{
    return HashPassword(password)
        .Equals(storedHash, StringComparison.OrdinalIgnoreCase);
}

string Page(string title, string body)
{
    return "<!DOCTYPE html>" +
    "<html>" +
    "<head>" +
    "<meta charset=\"UTF-8\">" +
    "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">" +
    "<title>" + title + "</title>" +

    "<style>" +
    "* { box-sizing: border-box; }" +

    "body {" +
    "margin: 0;" +
    "font-family: Arial, Helvetica, sans-serif;" +
    "background: #f3f6fb;" +
    "color: #1f2937;" +
    "}" +

    ".navbar {" +
    "height: 70px;" +
    "background: white;" +
    "border-bottom: 1px solid #e5e7eb;" +
    "display: flex;" +
    "align-items: center;" +
    "justify-content: space-between;" +
    "padding: 0 40px;" +
    "}" +

    ".brand {" +
    "font-size: 20px;" +
    "font-weight: bold;" +
    "color: #0078d4;" +
    "}" +

    ".badge {" +
    "background: #e8f3ff;" +
    "color: #0078d4;" +
    "padding: 8px 15px;" +
    "border-radius: 20px;" +
    "font-size: 13px;" +
    "font-weight: bold;" +
    "}" +

    ".container {" +
    "max-width: 1100px;" +
    "margin: 50px auto;" +
    "padding: 20px;" +
    "}" +

    ".card {" +
    "background: white;" +
    "border-radius: 16px;" +
    "padding: 35px;" +
    "box-shadow: 0 8px 30px rgba(0,0,0,.08);" +
    "}" +

    ".login-card {" +
    "max-width: 450px;" +
    "margin: 70px auto;" +
    "}" +

    "h1 { margin-top: 0; margin-bottom: 10px; }" +

    ".subtitle {" +
    "color: #6b7280;" +
    "margin-bottom: 30px;" +
    "}" +

    "label {" +
    "display: block;" +
    "margin-top: 18px;" +
    "margin-bottom: 7px;" +
    "font-weight: 600;" +
    "}" +

    "input {" +
    "width: 100%;" +
    "padding: 13px;" +
    "border: 1px solid #d1d5db;" +
    "border-radius: 8px;" +
    "font-size: 15px;" +
    "}" +

    "button {" +
    "width: 100%;" +
    "margin-top: 25px;" +
    "padding: 13px;" +
    "border: 0;" +
    "border-radius: 8px;" +
    "background: #0078d4;" +
    "color: white;" +
    "font-size: 15px;" +
    "font-weight: bold;" +
    "cursor: pointer;" +
    "}" +

    ".link {" +
    "display: block;" +
    "text-align: center;" +
    "margin-top: 20px;" +
    "color: #0078d4;" +
    "text-decoration: none;" +
    "}" +

    ".alert {" +
    "padding: 13px;" +
    "border-radius: 8px;" +
    "margin-bottom: 20px;" +
    "}" +

    ".danger {" +
    "background: #fee2e2;" +
    "color: #991b1b;" +
    "}" +

    ".success {" +
    "background: #dcfce7;" +
    "color: #166534;" +
    "}" +

    ".grid {" +
    "display: grid;" +
    "grid-template-columns: repeat(3, 1fr);" +
    "gap: 20px;" +
    "margin-top: 25px;" +
    "}" +

    ".stat {" +
    "background: white;" +
    "padding: 25px;" +
    "border-radius: 14px;" +
    "box-shadow: 0 5px 20px rgba(0,0,0,.06);" +
    "}" +

    ".stat-title { color: #6b7280; font-size: 14px; }" +
    ".stat-value { font-size: 25px; font-weight: bold; margin-top: 8px; }" +

    "table {" +
    "width: 100%;" +
    "border-collapse: collapse;" +
    "margin-top: 25px;" +
    "}" +

    "th, td {" +
    "text-align: left;" +
    "padding: 14px;" +
    "border-bottom: 1px solid #e5e7eb;" +
    "}" +

    "th { background: #f8fafc; }" +

    ".logout {" +
    "display: inline-block;" +
    "margin-top: 25px;" +
    "padding: 11px 20px;" +
    "background: #dc2626;" +
    "color: white;" +
    "text-decoration: none;" +
    "border-radius: 8px;" +
    "}" +

    "@media(max-width:700px) {" +
    ".grid { grid-template-columns: 1fr; }" +
    ".navbar { padding: 0 20px; }" +
    "}" +

    "</style>" +
    "</head>" +

    "<body>" +

    "<nav class=\"navbar\">" +
    "<div class=\"brand\">⚡ Test Service</div>" +
    "<div class=\"badge\">AZURE LAB</div>" +
    "</nav>" +

    body +

    "</body>" +
    "</html>";
}

IResult LoginFailed()
{
    var body =
        "<div class=\"container\">" +
        "<div class=\"card login-card\">" +
        "<div class=\"alert danger\">" +
        "Invalid username or password." +
        "</div>" +
        "<a class=\"link\" href=\"/\">Try again</a>" +
        "</div>" +
        "</div>";

    return Results.Content(
        Page("Login Failed", body),
        "text/html");
}

app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        application = "Healthy",
        environment = "LAB",
        timestamp = DateTime.UtcNow
    });
});

app.MapGet("/", () =>
{
    var body =
        "<div class=\"container\">" +
        "<div class=\"card login-card\">" +

        "<h1>Welcome Back</h1>" +
        "<div class=\"subtitle\">" +
        "Sign in to the Azure Application Lab" +
        "</div>" +

        "<form method=\"post\" action=\"/login\">" +

        "<label>Username</label>" +
        "<input type=\"text\" name=\"username\" required>" +

        "<label>Password</label>" +
        "<input type=\"password\" name=\"password\" required>" +

        "<button type=\"submit\">Login</button>" +

        "</form>" +

        "<a class=\"link\" href=\"/register\">" +
        "New user? Create an account" +
        "</a>" +

        "</div>" +
        "</div>";

    return Results.Content(
        Page("Login | Test Service", body),
        "text/html");
});

app.MapGet("/register", () =>
{
    var body =
        "<div class=\"container\">" +
        "<div class=\"card login-card\">" +

        "<h1>Create Account</h1>" +
        "<div class=\"subtitle\">" +
        "Register a new application user" +
        "</div>" +

        "<form method=\"post\" action=\"/register\">" +

        "<label>Name</label>" +
        "<input type=\"text\" name=\"name\" required>" +

        "<label>Username</label>" +
        "<input type=\"text\" name=\"username\" required>" +

        "<label>Email</label>" +
        "<input type=\"email\" name=\"email\" required>" +

        "<label>Password</label>" +
        "<input type=\"password\" name=\"password\" minlength=\"6\" required>" +

        "<button type=\"submit\">Create Account</button>" +

        "</form>" +

        "<a class=\"link\" href=\"/\">" +
        "Already have an account? Login" +
        "</a>" +

        "</div>" +
        "</div>";

    return Results.Content(
        Page("Register | Test Service", body),
        "text/html");
});

app.MapPost("/register", async (HttpRequest request) =>
{
    var form = await request.ReadFormAsync();

    var name = form["name"].ToString();
    var username = form["username"].ToString();
    var email = form["email"].ToString();
    var password = form["password"].ToString();

    if (string.IsNullOrWhiteSpace(name) ||
        string.IsNullOrWhiteSpace(username) ||
        string.IsNullOrWhiteSpace(email) ||
        string.IsNullOrWhiteSpace(password))
    {
        var body =
            "<div class=\"container\">" +
            "<div class=\"card login-card\">" +
            "<div class=\"alert danger\">" +
            "All fields are required." +
            "</div>" +
            "<a class=\"link\" href=\"/register\">Go back</a>" +
            "</div>" +
            "</div>";

        return Results.Content(
            Page("Registration Error", body),
            "text/html");
    }

    var connectionString = GetConnectionString();

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        return Results.Content(
            Page(
                "Database Error",
                "<div class=\"container\">" +
                "<div class=\"card login-card\">" +
                "<div class=\"alert danger\">" +
                "Database connection is not configured." +
                "</div>" +
                "</div>" +
                "</div>"),
            "text/html");
    }

    try
    {
        await using var connection =
            new SqlConnection(connectionString);

        await connection.OpenAsync();

        const string sql =
            "INSERT INTO Users " +
            "(Name, Username, Email, PasswordHash) " +
            "VALUES (@Name, @Username, @Email, @PasswordHash);";

        await using var command =
            new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("@Name", name);
        command.Parameters.AddWithValue("@Username", username);
        command.Parameters.AddWithValue("@Email", email);
        command.Parameters.AddWithValue(
            "@PasswordHash",
            HashPassword(password));

        await command.ExecuteNonQueryAsync();

        var body =
            "<div class=\"container\">" +
            "<div class=\"card login-card\">" +

            "<div class=\"alert success\">" +
            "Account created successfully." +
            "</div>" +

            "<h1>Welcome, " + username + "</h1>" +

            "<div class=\"subtitle\">" +
            "Your account has been stored in Azure SQL." +
            "</div>" +

            "<a class=\"link\" href=\"/\">" +
            "Continue to Login" +
            "</a>" +

            "</div>" +
            "</div>";

        return Results.Content(
            Page("Account Created", body),
            "text/html");
    }
    catch (SqlException ex)
    {
        Console.WriteLine("DATABASE ERROR: " + ex);

        var body =
            "<div class=\"container\">" +
            "<div class=\"card login-card\">" +

            "<div class=\"alert danger\">" +
            "Username or email may already exist, " +
            "or the database operation failed." +
            "</div>" +

            "<a class=\"link\" href=\"/register\">" +
            "Try again" +
            "</a>" +

            "</div>" +
            "</div>";

        return Results.Content(
            Page("Registration Error", body),
            "text/html");
    }
});

app.MapPost("/login", async (HttpRequest request) =>
{
    var form = await request.ReadFormAsync();

    var username = form["username"].ToString();
    var password = form["password"].ToString();

    var connectionString = GetConnectionString();

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        return Results.Content(
            Page(
                "Database Error",
                "<div class=\"container\">" +
                "<div class=\"card login-card\">" +
                "<div class=\"alert danger\">" +
                "Database connection is not configured." +
                "</div>" +
                "</div>" +
                "</div>"),
            "text/html");
    }

    try
    {
        await using var connection =
            new SqlConnection(connectionString);

        await connection.OpenAsync();

        const string sql =
            "SELECT Id, Name, Username, Email, PasswordHash " +
            "FROM Users WHERE Username = @Username;";

        await using var command =
            new SqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@Username",
            username);

        await using var reader =
            await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return LoginFailed();
        }

        var name =
            reader["Name"].ToString() ?? "";

        var email =
            reader["Email"].ToString() ?? "";

        var storedHash =
            reader["PasswordHash"].ToString() ?? "";

        if (!VerifyPassword(password, storedHash))
        {
            return LoginFailed();
        }

        var body =
            "<div class=\"container\">" +

            "<h1>Application Dashboard</h1>" +

            "<div class=\"subtitle\">" +
            "Welcome back, " + name +
            "</div>" +

            "<div class=\"grid\">" +

            "<div class=\"stat\">" +
            "<div class=\"stat-title\">Application</div>" +
            "<div class=\"stat-value\">Healthy</div>" +
            "</div>" +

            "<div class=\"stat\">" +
            "<div class=\"stat-title\">Database</div>" +
            "<div class=\"stat-value\">Connected</div>" +
            "</div>" +

            "<div class=\"stat\">" +
            "<div class=\"stat-title\">Environment</div>" +
            "<div class=\"stat-value\">LAB</div>" +
            "</div>" +

            "</div>" +

            "<div class=\"card\" style=\"margin-top:25px\">" +

            "<h2>User Information</h2>" +

            "<table>" +

            "<tr>" +
            "<th>Name</th>" +
            "<td>" + name + "</td>" +
            "</tr>" +

            "<tr>" +
            "<th>Username</th>" +
            "<td>" + username + "</td>" +
            "</tr>" +

            "<tr>" +
            "<th>Email</th>" +
            "<td>" + email + "</td>" +
            "</tr>" +

            "</table>" +

            "<a class=\"logout\" href=\"/\">" +
            "Logout" +
            "</a>" +

            "</div>" +

            "</div>";

        return Results.Content(
            Page("Dashboard | Test Service", body),
            "text/html");
    }
    catch (SqlException ex)
    {
        Console.WriteLine("DATABASE ERROR: " + ex);

        return Results.Content(
            Page(
                "Database Error",
                "<div class=\"container\">" +
                "<div class=\"card login-card\">" +
                "<div class=\"alert danger\">" +
                "Database error occurred." +
                "</div>" +
                "</div>" +
                "</div>"),
            "text/html");
    }
});

app.Run();
