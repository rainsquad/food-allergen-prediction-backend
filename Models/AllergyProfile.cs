using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace food_allergen_prediction_backend.Models
{
    public class AllergyProfile
    {
        [Key]
        public Guid Id { get; set; }

        public List<string> Allergies { get; set; } = [];

        public string Severity { get; set; } = "Medium";

        [ForeignKey("User")]
        public Guid UserId { get; set; }
    }
}
