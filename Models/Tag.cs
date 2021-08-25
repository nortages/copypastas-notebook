using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Practice.Models
{
    [Table("Tag")]
    public class Tag
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Name { get; set; }
        
        public int? CategoryId { get; set; }
        [CanBeNull] public virtual TagCategory Category { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<RecordTag> RecordTags { get; set; } = new List<RecordTag>();
    }
}
