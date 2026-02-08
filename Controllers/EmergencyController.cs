using food_allergen_prediction_backend.Data;
using food_allergen_prediction_backend.DTOs;
using food_allergen_prediction_backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace food_allergen_prediction_backend.Controllers
{
    [ApiController]
    [Route("emergency")]
    public class EmergencyController(AppDbContext db) : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var user = (User)HttpContext.Items["User"]!;
            return Ok(db.EmergencyContacts.Where(e => e.UserId == user.Id));
        }

        [HttpPost]
        public async Task<IActionResult> Add(EmergencyDto dto)
        {
            var user = (User)HttpContext.Items["User"]!;

            var contact = new EmergencyContact
            {
                UserId = user.Id,
                Name = dto.Name,
                Phone = dto.Phone
            };

            db.EmergencyContacts.Add(contact);
            await db.SaveChangesAsync();

            return Ok(contact);
        }
    }
}
