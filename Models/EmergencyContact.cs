using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace food_allergen_prediction_backend.Models
{
    public class EmergencyContact
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        [ForeignKey("User")]
        public Guid UserId { get; set; }
    }
}
