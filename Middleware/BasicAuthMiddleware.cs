using food_allergen_prediction_backend.Data;
using Microsoft.EntityFrameworkCore;

namespace food_allergen_prediction_backend.Middleware
{
    public class BasicAuthMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context, AppDbContext db)
        {
            if (context.Request.Path.StartsWithSegments("/auth"))
            {
                await next(context);
                return;
            }

            var email = context.Request.Headers["X-Email"].FirstOrDefault();
            var password = context.Request.Headers["X-Password"].FirstOrDefault();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Missing credentials");
                return;
            }

            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid credentials");
                return;
            }

            context.Items["User"] = user;
            await next(context);
        }
    }
}
