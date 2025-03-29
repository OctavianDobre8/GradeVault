using GradeVault.Server.Database;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// EXACT registration syntax:
builder.Services.AddDbContext<AppDatabaseContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36)),
        mysqlOptions =>
        {
            mysqlOptions.EnableRetryOnFailure();
            mysqlOptions.SchemaBehavior(MySqlSchemaBehavior.Translate,
                (schema, table) => schema.Replace(" ", "_").ToLower());
        }
    ));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

// Fallback to index.html for client-side routing
app.MapFallbackToFile("/index.html");

app.Run();