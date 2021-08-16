using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
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

        [JsonIgnore]
        public virtual ICollection<RecordLabel> RecordLabels { get; set; } = new List<RecordLabel>();
    }
}
