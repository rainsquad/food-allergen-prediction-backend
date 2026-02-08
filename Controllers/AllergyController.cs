using food_allergen_prediction_backend.Data;
using food_allergen_prediction_backend.DTOs;
using food_allergen_prediction_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace food_allergen_prediction_backend.Controllers
{
    [ApiController]
    [Route("allergy")]
    public class AllergyController(AppDbContext db) : ControllerBase
    {
        [HttpGet]
        public IActionResult Get(Guid userID)
        {
            //var user = db.Users.FirstOrDefaultAsync(u => u.Id == userID);
            //if (user == null)
            //    return BadRequest("User not found");

            var profile = db.AllergyProfiles.FirstOrDefault(a => a.UserId == userID);
            return Ok(profile);
        }

        [HttpPost]
        public async Task<IActionResult> Save(AllergyDto dto)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == dto.UserID);
            if (user == null)
                return BadRequest("User not found");

            var profile = await db.AllergyProfiles
                .FirstOrDefaultAsync(a => a.UserId == user.Id);

            if (profile == null)
            {
                profile = new AllergyProfile
                {
                    UserId = user.Id
                };
                db.AllergyProfiles.Add(profile);
            }

            profile.Allergies = dto.Allergies;
            profile.Severity = dto.Severity;

            await db.SaveChangesAsync();
            return Ok(profile);
        }
    }
}
