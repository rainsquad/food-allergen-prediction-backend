using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("food")]
public class FoodController : ControllerBase
{
    private readonly FoodAllergenModelService _ml;

    public FoodController(FoodAllergenModelService ml)
    {
        _ml = ml;
    }

    [HttpPost("check")]
    public IActionResult Check([FromBody] FoodCheckRequest request)
    {
        var features = _ml.Tokenize(request.FoodName);
        var prediction = _ml.Predict(features);
        var allergyClasses = _ml.MapToClasses(prediction);

        return Ok(allergyClasses);
    }
}
