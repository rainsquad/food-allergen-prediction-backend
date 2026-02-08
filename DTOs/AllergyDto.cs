namespace food_allergen_prediction_backend.DTOs
{
    public record AllergyDto(List<string> Allergies, string Severity, Guid UserID);
}
