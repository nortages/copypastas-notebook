using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Practice.Models
{
    [Table("TagCategory")]
    public class TagCategory
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}