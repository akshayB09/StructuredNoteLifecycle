using StructuredNoteLifecycle.Application;
using StructuredNoteLifecycle.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapGet("/swagger", () => Results.Text(@"<!DOCTYPE html>
<html>
  <head>
    <meta charset=""utf-8"" />
    <title>Swagger UI</title>
    <link rel=""stylesheet"" href=""https://cdn.jsdelivr.net/npm/swagger-ui-dist/swagger-ui.css"" />
  </head>
  <body>
    <div id=""swagger-ui""></div>
    <script src=""https://cdn.jsdelivr.net/npm/swagger-ui-dist/swagger-ui-bundle.js""></script>
    <script src=""https://cdn.jsdelivr.net/npm/swagger-ui-dist/swagger-ui-standalone-preset.js""></script>
    <script>
      window.onload = function() {
        SwaggerUIBundle({
          url: '/openapi/v1.json',
          dom_id: '#swagger-ui',
          presets: [SwaggerUIBundle.presets.apis, SwaggerUIStandalonePreset],
          layout: 'StandaloneLayout'
        });
      };
    </script>
  </body>
</html>",
        "text/html; charset=utf-8"));
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
