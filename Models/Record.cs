using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Practice.Models
{
    [Table("Record")]
    public class Record
    {
        public int Id { get; set; }
        [StringLength(500)]
        public string Text { get; set; }

        public virtual ICollection<RecordTag> RecordTags { get; set; } = new List<RecordTag>();
    }
}
