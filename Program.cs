using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", async () =>
{
    var connectionString =
        Environment.GetEnvironmentVariable(
            "SQLAZURECONNSTR_AZURE_SQL_CONNECTIONSTRING");

    string databaseStatus = "Not Configured";
    string databaseClass = "warning";
    string customersHtml = "";

    if (!string.IsNullOrEmpty(connectionString))
    {
        try
        {
            await using var connection =
                new SqlConnection(connectionString);

            await connection.OpenAsync();

            databaseStatus = "Connected";
            databaseClass = "success";

            var command = new SqlCommand(
                "SELECT Id, Name, Email FROM Customers ORDER BY Id",
                connection);

            await using var reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                customersHtml += $@"
                    <tr>
                        <td>
                            <div class=""user-id"">
                                {reader.GetValue(0)}
                            </div>
                        </td>

                        <td>
                            <div class=""user-name"">
                                {reader.GetValue(1)}
                            </div>
                        </td>

                        <td>
                            <span class=""email"">
                                {reader.GetValue(2)}
                            </span>
                        </td>
                    </tr>
                    ";
            }
        }
        catch
        {
            databaseStatus = "Connection Failed";
            databaseClass = "danger";
        }
    }

    var html = @"
<!DOCTYPE html>

<html>

<head>

    <meta charset=""UTF-8"">

    <meta name=""viewport""
          content=""width=device-width, initial-scale=1.0"">

    <title>Test Service | Azure Lab</title>

    <style>

        * {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
        }

        body {
            font-family:
                -apple-system,
                BlinkMacSystemFont,
                ""Segoe UI"",
                Roboto,
                Arial,
                sans-serif;

            background: #f5f7fb;
            color: #1f2937;
        }

        /* NAVBAR */

        .navbar {
            height: 70px;
            background: #ffffff;
            border-bottom: 1px solid #e5e7eb;

            display: flex;
            align-items: center;
            justify-content: space-between;

            padding: 0 40px;
        }

        .brand {
            display: flex;
            align-items: center;
            gap: 12px;
        }

        .brand-icon {
            width: 40px;
            height: 40px;

            border-radius: 10px;

            background: #0078d4;

            color: white;

            display: flex;
            align-items: center;
            justify-content: center;

            font-weight: bold;
            font-size: 18px;
        }

        .brand-name {
            font-size: 19px;
            font-weight: 700;
        }

        .brand-subtitle {
            font-size: 12px;
            color: #6b7280;
        }

        .environment {
            background: #e8f3ff;
            color: #0078d4;

            padding: 7px 14px;

            border-radius: 20px;

            font-size: 12px;
            font-weight: 700;
        }

        /* LAYOUT */

        .container {
            max-width: 1200px;
            margin: auto;
            padding: 35px 25px;
        }

        .page-title {
            font-size: 28px;
            font-weight: 700;
        }

        .page-description {
            margin-top: 7px;
            color: #6b7280;
        }

        /* CARDS */

        .cards {
            display: grid;

            grid-template-columns:
                repeat(3, 1fr);

            gap: 20px;

            margin-top: 30px;
        }

        .card {
            background: white;

            border: 1px solid #e5e7eb;

            border-radius: 14px;

            padding: 22px;

            box-shadow:
                0 3px 10px
                rgba(0,0,0,0.04);
        }

        .card-header {
            display: flex;

            align-items: center;

            justify-content: space-between;
        }

        .card-title {
            color: #6b7280;

            font-size: 13px;

            font-weight: 600;

            text-transform: uppercase;

            letter-spacing: .5px;
        }

        .card-value {
            font-size: 22px;

            font-weight: 700;

            margin-top: 12px;
        }

        .icon {
            width: 42px;
            height: 42px;

            border-radius: 10px;

            display: flex;

            align-items: center;

            justify-content: center;

            font-size: 19px;
        }

        .icon-blue {
            background: #e8f3ff;
            color: #0078d4;
        }

        .icon-green {
            background: #eaf8ef;
            color: #15803d;
        }

        .icon-purple {
            background: #f2edff;
            color: #6d28d9;
        }

        /* STATUS */

        .status {
            display: inline-flex;

            align-items: center;

            gap: 7px;

            padding: 6px 11px;

            border-radius: 20px;

            font-size: 12px;

            font-weight: 700;

            margin-top: 10px;
        }

        .status-dot {
            width: 7px;
            height: 7px;

            border-radius: 50%;
        }

        .success {
            background: #eaf8ef;
            color: #15803d;
        }

        .success .status-dot {
            background: #16a34a;
        }

        .warning {
            background: #fff7e6;
            color: #b45309;
        }

        .warning .status-dot {
            background: #f59e0b;
        }

        .danger {
            background: #fff0f0;
            color: #dc2626;
        }

        .danger .status-dot {
            background: #dc2626;
        }

        /* SECTION */

        .section {
            margin-top: 25px;
        }

        .section-header {
            display: flex;

            align-items: center;

            justify-content: space-between;

            margin-bottom: 15px;
        }

        .section-title {
            font-size: 19px;

            font-weight: 700;
        }

        .view-api {
            text-decoration: none;

            color: #0078d4;

            font-size: 13px;

            font-weight: 600;
        }

        /* TABLE */

        .table-container {
            background: white;

            border-radius: 14px;

            border: 1px solid #e5e7eb;

            overflow: hidden;

            box-shadow:
                0 3px 10px
                rgba(0,0,0,0.04);
        }

        table {
            width: 100%;

            border-collapse: collapse;
        }

        th {
            background: #f9fafb;

            color: #6b7280;

            font-size: 12px;

            text-transform: uppercase;

            letter-spacing: .5px;

            text-align: left;

            padding: 15px 20px;

            border-bottom: 1px solid #e5e7eb;
        }

        td {
            padding: 16px 20px;

            border-bottom: 1px solid #f0f0f0;
        }

        tr:last-child td {
            border-bottom: none;
        }

        tr:hover {
            background: #fafcff;
        }

        .user-id {
            width: 34px;
            height: 34px;

            border-radius: 8px;

            background: #eef6ff;

            color: #0078d4;

            display: flex;

            align-items: center;

            justify-content: center;

            font-weight: 700;

            font-size: 13px;
        }

        .user-name {
            font-weight: 600;
        }

        .email {
            color: #6b7280;
        }

        /* INFO */

        .info-grid {
            display: grid;

            grid-template-columns:
                repeat(4, 1fr);

            gap: 15px;
        }

        .info-item {
            padding: 15px;

            background: #f9fafb;

            border-radius: 10px;
        }

        .info-label {
            font-size: 11px;

            color: #6b7280;

            text-transform: uppercase;
        }

        .info-value {
            margin-top: 6px;

            font-weight: 600;

            font-size: 14px;
        }

        /* FOOTER */

        footer {
            margin-top: 40px;

            padding: 20px 0;

            color: #9ca3af;

            font-size: 12px;

            text-align: center;
        }

        /* MOBILE */

        @media(max-width: 800px) {

            .cards {
                grid-template-columns: 1fr;
            }

            .info-grid {
                grid-template-columns:
                    repeat(2, 1fr);
            }

            .navbar {
                padding: 0 20px;
            }

        }

    </style>

</head>


<body>


<!-- NAVIGATION -->

<nav class=""navbar"">

    <div class=""brand"">

        <div class=""brand-icon"">
            TS
        </div>

        <div>

            <div class=""brand-name"">
                Test Service
            </div>

            <div class=""brand-subtitle"">
                Azure Application Lab
            </div>

        </div>

    </div>


    <div class=""environment"">
        LAB ENVIRONMENT
    </div>

</nav>


<!-- MAIN -->

<main class=""container"">


    <h1 class=""page-title"">
        Application Dashboard
    </h1>

    <p class=""page-description"">
        Azure App Service backup, restore and migration lab
    </p>


    <!-- STATUS CARDS -->

    <div class=""cards"">


        <!-- APPLICATION -->

        <div class=""card"">

            <div class=""card-header"">

                <div class=""card-title"">
                    Application
                </div>

                <div class=""icon icon-blue"">
                    ⚡
                </div>

            </div>


            <div class=""card-value"">
                Healthy
            </div>


            <div class=""status success"">

                <span class=""status-dot""></span>

                Running

            </div>

        </div>


        <!-- DATABASE -->

        <div class=""card"">

            <div class=""card-header"">

                <div class=""card-title"">
                    Database
                </div>

                <div class=""icon icon-green"">
                    DB
                </div>

            </div>


            <div class=""card-value"">
                {{databaseStatus}}
            </div>


            <div class=""status {{databaseClass}}"">

                <span class=""status-dot""></span>

                Azure SQL

            </div>

        </div>


        <!-- ENVIRONMENT -->

        <div class=""card"">

            <div class=""card-header"">

                <div class=""card-title"">
                    Environment
                </div>

                <div class=""icon icon-purple"">
                    ☁
                </div>

            </div>


            <div class=""card-value"">
                LAB
            </div>


            <div class=""status success"">

                <span class=""status-dot""></span>

                Test Environment

            </div>

        </div>


    </div>


    <!-- CUSTOMERS -->

    <div class=""section"">


        <div class=""section-header"">

            <div class=""section-title"">
                Customers
            </div>

            <a
                class=""view-api""
                href=""/health""
                target=""_blank"">

                View Health API →

            </a>

        </div>


        <div class=""table-container"">


            <table>

                <thead>

                    <tr>

                        <th>
                            ID
                        </th>

                        <th>
                            Customer
                        </th>

                        <th>
                            Email
                        </th>

                    </tr>

                </thead>


                <tbody>

                    {{customersHtml}}

                </tbody>

            </table>


        </div>


    </div>


    <!-- ENVIRONMENT INFORMATION -->

    <div class=""section"">


        <div class=""section-title"">
            Environment Information
        </div>


        <div class=""card"" style=""margin-top:15px"">


            <div class=""info-grid"">


                <div class=""info-item"">

                    <div class=""info-label"">
                        Platform
                    </div>

                    <div class=""info-value"">
                        Linux
                    </div>

                </div>


                <div class=""info-item"">

                    <div class=""info-label"">
                        Runtime
                    </div>

                    <div class=""info-value"">
                        .NET
                    </div>

                </div>


                <div class=""info-item"">

                    <div class=""info-label"">
                        Hosting
                    </div>

                    <div class=""info-value"">
                        Azure App Service
                    </div>

                </div>


                <div class=""info-item"">

                    <div class=""info-label"">
                        Deployment
                    </div>

                    <div class=""info-value"">
                        GitHub
                    </div>

                </div>


            </div>


        </div>


    </div>


</main>


<footer>

    Test Service · Azure App Service Lab

</footer>


</body>

</html>";

    html = html.Replace("{{databaseStatus}}", databaseStatus)
               .Replace("{{databaseClass}}", databaseClass)
               .Replace("{{customersHtml}}", customersHtml);

    return Results.Content(
        html,
        "text/html");
});


app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        application = "Healthy",
        environment = "LAB",
        timestamp = DateTime.UtcNow
    });
});


app.Run();
