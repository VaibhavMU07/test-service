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
    "background: linear-gradient(135deg,#eef6ff 0%,#f8fafc 45%,#eef2ff 100%);" +
    "color: #172033;" +
    "min-height: 100vh;" +
    "}" +

    ".navbar {" +
    "height: 72px;" +
    "background: rgba(255,255,255,.94);" +
    "border-bottom: 1px solid #e5e7eb;" +
    "display: flex;" +
    "align-items: center;" +
    "justify-content: space-between;" +
    "padding: 0 6%;" +
    "position: sticky;" +
    "top: 0;" +
    "z-index: 10;" +
    "}" +

    ".brand {" +
    "display: flex;" +
    "align-items: center;" +
    "gap: 10px;" +
    "font-size: 20px;" +
    "font-weight: 800;" +
    "color: #1261a0;" +
    "}" +

    ".brand-icon {" +
    "width: 38px;" +
    "height: 38px;" +
    "border-radius: 11px;" +
    "background: linear-gradient(135deg,#0078d4,#4f46e5);" +
    "color: white;" +
    "display: flex;" +
    "align-items: center;" +
    "justify-content: center;" +
    "font-weight: bold;" +
    "}" +

    ".badge {" +
    "background: #e8f3ff;" +
    "color: #1261a0;" +
    "padding: 8px 15px;" +
    "border-radius: 999px;" +
    "font-size: 12px;" +
    "font-weight: 700;" +
    "letter-spacing: .5px;" +
    "}" +

    ".container {" +
    "max-width: 1120px;" +
    "margin: auto;" +
    "padding: 55px 24px;" +
    "}" +

    ".hero {" +
    "text-align: center;" +
    "padding: 45px 20px 25px;" +
    "}" +

    ".hero-icon {" +
    "width: 78px;" +
    "height: 78px;" +
    "margin: auto;" +
    "border-radius: 22px;" +
    "background: linear-gradient(135deg,#0078d4,#4f46e5);" +
    "color: white;" +
    "font-size: 36px;" +
    "display: flex;" +
    "align-items: center;" +
    "justify-content: center;" +
    "box-shadow: 0 15px 35px rgba(0,120,212,.25);" +
    "}" +

    ".hero h1 {" +
    "font-size: 42px;" +
    "margin: 25px 0 12px;" +
    "letter-spacing: -1px;" +
    "}" +

    ".hero p {" +
    "font-size: 17px;" +
    "color: #64748b;" +
    "max-width: 650px;" +
    "margin: auto;" +
    "line-height: 1.7;" +
    "}" +

    ".buttons {" +
    "display: flex;" +
    "justify-content: center;" +
    "gap: 14px;" +
    "margin-top: 30px;" +
    "flex-wrap: wrap;" +
    "}" +

    ".btn {" +
    "display: inline-block;" +
    "padding: 13px 24px;" +
    "border-radius: 10px;" +
    "text-decoration: none;" +
    "font-weight: 700;" +
    "font-size: 14px;" +
    "transition: .2s;" +
    "}" +

    ".btn-primary {" +
    "background: linear-gradient(135deg,#0078d4,#2563eb);" +
    "color: white;" +
    "box-shadow: 0 8px 20px rgba(37,99,235,.22);" +
    "}" +

    ".btn-secondary {" +
    "background: white;" +
    "color: #1d4ed8;" +
    "border: 1px solid #bfdbfe;" +
    "}" +

    ".btn-health {" +
    "background: #ecfdf5;" +
    "color: #047857;" +
    "border: 1px solid #a7f3d0;" +
    "}" +

    ".feature-grid {" +
    "display: grid;" +
    "grid-template-columns: repeat(3,1fr);" +
    "gap: 20px;" +
    "margin-top: 45px;" +
    "}" +

    ".feature {" +
    "background: rgba(255,255,255,.92);" +
    "border: 1px solid #e5e7eb;" +
    "border-radius: 18px;" +
    "padding: 27px;" +
    "box-shadow: 0 10px 30px rgba(15,23,42,.06);" +
    "}" +

    ".feature-icon {" +
    "font-size: 26px;" +
    "margin-bottom: 15px;" +
    "}" +

    ".feature h3 {" +
    "margin: 0 0 8px;" +
    "}" +

    ".feature p {" +
    "color: #64748b;" +
    "line-height: 1.6;" +
    "font-size: 14px;" +
    "margin: 0;" +
    "}" +

    ".card {" +
    "background: rgba(255,255,255,.96);" +
    "border: 1px solid #e5e7eb;" +
    "border-radius: 18px;" +
    "padding: 35px;" +
    "box-shadow: 0 12px 35px rgba(15,23,42,.08);" +
    "}" +

    ".login-card {" +
    "max-width: 470px;" +
    "margin: 45px auto;" +
    "}" +

    "h1 { margin-top: 0; margin-bottom: 10px; }" +

    ".subtitle {" +
    "color: #64748b;" +
    "margin-bottom: 28px;" +
    "line-height: 1.6;" +
    "}" +

    "label {" +
    "display: block;" +
    "margin-top: 17px;" +
    "margin-bottom: 7px;" +
    "font-weight: 600;" +
    "font-size: 14px;" +
    "}" +

    "input {" +
    "width: 100%;" +
    "padding: 13px;" +
    "border: 1px solid #cbd5e1;" +
    "border-radius: 9px;" +
    "font-size: 15px;" +
    "outline: none;" +
    "}" +

    "input:focus {" +
    "border-color: #3b82f6;" +
    "box-shadow: 0 0 0 3px rgba(59,130,246,.12);" +
    "}" +

    "button {" +
    "width: 100%;" +
    "margin-top: 24px;" +
    "padding: 13px;" +
    "border: 0;" +
    "border-radius: 9px;" +
    "background: linear-gradient(135deg,#0078d4,#2563eb);" +
    "color: white;" +
    "font-size: 15px;" +
    "font-weight: bold;" +
    "cursor: pointer;" +
    "}" +

    ".link {" +
    "display: block;" +
    "text-align: center;" +
    "margin-top: 20px;" +
    "color: #2563eb;" +
    "text-decoration: none;" +
    "font-weight: 600;" +
    "}" +

    ".alert {" +
    "padding: 13px;" +
    "border-radius: 9px;" +
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
    "grid-template-columns: repeat(3,1fr);" +
    "gap: 20px;" +
    "margin-top: 25px;" +
    "}" +

    ".stat {" +
    "background: white;" +
    "padding: 25px;" +
    "border-radius: 15px;" +
    "border: 1px solid #e5e7eb;" +
    "box-shadow: 0 6px 20px rgba(15,23,42,.05);" +
    "}" +

    ".stat-title {" +
    "color: #64748b;" +
    "font-size: 13px;" +
    "font-weight: 600;" +
    "}" +

    ".stat-value {" +
    "font-size: 23px;" +
    "font-weight: 800;" +
    "margin-top: 8px;" +
    "color: #172033;" +
    "}" +

    ".status-dot {" +
    "display: inline-block;" +
    "width: 9px;" +
    "height: 9px;" +
    "border-radius: 50%;" +
    "background: #16a34a;" +
    "margin-right: 7px;" +
    "}" +

    ".info-card {" +
    "margin-top: 25px;" +
    "}" +

    "table {" +
    "width: 100%;" +
    "border-collapse: collapse;" +
    "margin-top: 18px;" +
    "}" +

    "th,td {" +
    "text-align: left;" +
    "padding: 14px;" +
    "border-bottom: 1px solid #e5e7eb;" +
    "}" +

    "th {" +
    "background: #f8fafc;" +
    "width: 35%;" +
    "color: #475569;" +
    "}" +

    ".db-value {" +
    "font-family: monospace;" +
    "font-size: 14px;" +
    "color: #1d4ed8;" +
    "word-break: break-all;" +
    "}" +

    ".logout {" +
    "display: inline-block;" +
    "margin-top: 25px;" +
    "padding: 11px 20px;" +
    "background: #ef4444;" +
    "color: white;" +
    "text-decoration: none;" +
    "border-radius: 9px;" +
    "font-weight: 700;" +
    "}" +

    ".footer {" +
    "text-align: center;" +
    "color: #94a3b8;" +
    "font-size: 13px;" +
    "padding: 30px;" +
    "}" +

    "@media(max-width:700px) {" +
    ".grid,.feature-grid { grid-template-columns: 1fr; }" +
    ".hero h1 { font-size: 32px; }" +
    ".navbar { padding: 0 20px; }" +
    "}" +

    "</style>" +
    "</head>" +

    "<body>" +

    "<nav class=\"navbar\">" +
    "<div class=\"brand\">" +
    "<div class=\"brand-icon\">⚡</div>" +
    "<div>Test Service</div>" +
    "</div>" +
    "<div class=\"badge\">AZURE LAB</div>" +
    "</nav>" +

    body +

    "<div class=\"footer\">" +
    "Azure App Service • .NET 6 • Azure SQL • Application Insights" +
    "</div>" +

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

        "<section class=\"hero\">" +

        "<div class=\"hero-icon\">☁</div>" +

        "<h1>Azure Application Lab</h1>" +

        "<p>" +
        "A simple cloud application running on Azure App Service " +
        "with Azure SQL Database and Application Insights." +
        "</p>" +

        "<div class=\"buttons\">" +

        "<a class=\"btn btn-primary\" href=\"/login\">" +
        "Sign In" +
        "</a>" +

        "<a class=\"btn btn-secondary\" href=\"/register\">" +
        "Create Account" +
        "</a>" +

        "<a class=\"btn btn-health\" href=\"/health\">" +
        "Health Check" +
        "</a>" +

        "</div>" +

        "</section>" +

        "<div class=\"feature-grid\">" +

        "<div class=\"feature\">" +
        "<div class=\"feature-icon\">⚡</div>" +
        "<h3>Azure App Service</h3>" +
        "<p>Hosted on Linux using the .NET 6 runtime.</p>" +
        "</div>" +

        "<div class=\"feature\">" +
        "<div class=\"feature-icon\">🗄️</div>" +
        "<h3>Azure SQL</h3>" +
        "<p>User registration and authentication data are stored in Azure SQL Database.</p>" +
        "</div>" +

        "<div class=\"feature\">" +
        "<div class=\"feature-icon\">📊</div>" +
        "<h3>Application Insights</h3>" +
        "<p>Application monitoring and telemetry are enabled for the lab environment.</p>" +
        "</div>" +

        "</div>" +

        "</div>";

    return Results.Content(
        Page("Azure Application Lab", body),
        "text/html");
});

app.MapGet("/login", () =>
{
    var body =
        "<div class=\"container\">" +

        "<div class=\"card login-card\">" +

        "<h1>Welcome Back</h1>" +

        "<div class=\"subtitle\">" +
        "Sign in to your Azure Application Lab account." +
        "</div>" +

        "<form method=\"post\" action=\"/login\">" +

        "<label>Username</label>" +
        "<input type=\"text\" name=\"username\" placeholder=\"Enter username\" required>" +

        "<label>Password</label>" +
        "<input type=\"password\" name=\"password\" placeholder=\"Enter password\" required>" +

        "<button type=\"submit\">Sign In</button>" +

        "</form>" +

        "<a class=\"link\" href=\"/register\">" +
        "New user? Create an account" +
        "</a>" +

        "<a class=\"link\" href=\"/\">" +
        "← Back to landing page" +
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
        "Create a new user. The account will be stored in Azure SQL Database." +
        "</div>" +

        "<form method=\"post\" action=\"/register\">" +

        "<label>Name</label>" +
        "<input type=\"text\" name=\"name\" placeholder=\"Your name\" required>" +

        "<label>Username</label>" +
        "<input type=\"text\" name=\"username\" placeholder=\"Choose username\" required>" +

        "<label>Email</label>" +
        "<input type=\"email\" name=\"email\" placeholder=\"you@example.com\" required>" +

        "<label>Password</label>" +
        "<input type=\"password\" name=\"password\" minlength=\"6\" placeholder=\"Minimum 6 characters\" required>" +

        "<button type=\"submit\">Create Account</button>" +

        "</form>" +

        "<a class=\"link\" href=\"/login\">" +
        "Already have an account? Sign in" +
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
            "✓ Account created successfully." +
            "</div>" +

            "<h1>Welcome, " + username + "</h1>" +

            "<div class=\"subtitle\">" +
            "Your account has been stored successfully in Azure SQL." +
            "</div>" +

            "<a class=\"btn btn-primary\" href=\"/login\" style=\"text-align:center;display:block;\">" +
            "Continue to Sign In" +
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
            "Username or email may already exist, or the database operation failed." +
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

        // Get the actual SQL server and database being used.
        var databaseServer = connection.DataSource;
        var databaseName = connection.Database;

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

            "<div style=\"margin-bottom:25px;\">" +
            "<div class=\"badge\" style=\"display:inline-block;\">SIGNED IN SUCCESSFULLY</div>" +
            "</div>" +

            "<h1>Application Dashboard</h1>" +

            "<div class=\"subtitle\">" +
            "Welcome back, " + name +
            ". Here is the current Azure application status." +
            "</div>" +

            "<div class=\"grid\">" +

            "<div class=\"stat\">" +
            "<div class=\"stat-title\">Application</div>" +
            "<div class=\"stat-value\">" +
            "<span class=\"status-dot\"></span>Healthy" +
            "</div>" +
            "</div>" +

            "<div class=\"stat\">" +
            "<div class=\"stat-title\">Database</div>" +
            "<div class=\"stat-value\">" +
            "<span class=\"status-dot\"></span>Connected" +
            "</div>" +
            "</div>" +

            "<div class=\"stat\">" +
            "<div class=\"stat-title\">Environment</div>" +
            "<div class=\"stat-value\">LAB</div>" +
            "</div>" +

            "</div>" +

            "<div class=\"card info-card\">" +

            "<h2>🗄️ Database Information</h2>" +

            "<div class=\"subtitle\">" +
            "This information is retrieved from the active SQL connection." +
            "</div>" +

            "<table>" +

            "<tr>" +
            "<th>Connection Status</th>" +
            "<td><span class=\"status-dot\"></span>Connected</td>" +
            "</tr>" +

            "<tr>" +
            "<th>SQL Server</th>" +
            "<td class=\"db-value\">" +
            databaseServer +
            "</td>" +
            "</tr>" +

            "<tr>" +
            "<th>Database Name</th>" +
            "<td class=\"db-value\">" +
            databaseName +
            "</td>" +
            "</tr>" +

            "</table>" +

            "</div>" +

            "<div class=\"card info-card\">" +

            "<h2>👤 User Information</h2>" +

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
                "<a class=\"link\" href=\"/\">Return Home</a>" +
                "</div>" +
                "</div>"),
            "text/html");
    }
});

app.Run();
