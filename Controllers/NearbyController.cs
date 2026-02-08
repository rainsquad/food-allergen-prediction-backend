using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace food_allergen_prediction_backend.Controllers
{
    [ApiController]
    [Route("nearby")]
    public class NearbyController(HttpClient client, IConfiguration config)
    : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get(double lat, double lng)
        {
            var url =
                $"https://maps.googleapis.com/maps/api/place/nearbysearch/json" +
                $"?location={lat},{lng}&radius=2000&type=restaurant&key={config["Google:ApiKey"]}";

            var json = await client.GetStringAsync(url);
            return Ok(JsonSerializer.Deserialize<object>(json));
        }
    }
}
