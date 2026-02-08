using food_allergen_prediction_backend.Data;
using food_allergen_prediction_backend.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ------------------------
// Services
// ------------------------

// Use a single DbContext registration
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddSingleton<FoodAllergenModelService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS (for React Native / Expo)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// ------------------------
// Apply EF Migrations automatically
// ------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // This will create missing tables on first deploy
}

// ------------------------
// Middleware
// ------------------------
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAll");
// app.UseMiddleware<BasicAuthMiddleware>();
app.MapControllers();

// ------------------------
// Listen on Render's port
// ------------------------
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000"; // default fallback
app.Urls.Add($"http://*:{port}");

app.Run();
