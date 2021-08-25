using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;

#nullable disable

namespace Practice.Models
{
    [Table("Record")]
    public class Record
    {
        public int Id { get; set; }
        [StringLength(500)] public string Text { get; set; }
        public int? OriginalRecordId { get; set; }

        [CanBeNull] public virtual Record OriginalRecord { get; set; }
        [ForeignKey(nameof(OriginalRecordId))] public virtual ICollection<Record> SimilarRecords { get; set; }
        public virtual ICollection<RecordTag> RecordTags { get; set; }
    }
}
