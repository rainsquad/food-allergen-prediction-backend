using System.ComponentModel.DataAnnotations;

namespace food_allergen_prediction_backend.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public AllergyProfile? AllergyProfile { get; set; }
        public List<EmergencyContact> EmergencyContacts { get; set; } = [];
    }
}
