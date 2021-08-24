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
        [StringLength(500)] public string Text { get; set; }
        public int? OriginalRecordId { get; set; }

        public virtual Record OriginalRecord { get; set; }
        [ForeignKey("OriginalRecordId")]
        public virtual ICollection<Record> SimilarRecords { get; set; }
        public virtual ICollection<RecordTag> RecordTags { get; set; }
    }
}
