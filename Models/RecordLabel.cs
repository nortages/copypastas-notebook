﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Practice.Models
{
    [Table("RecordLabel")]
    public class RecordLabel
    {
        public int Id { get; set; }
        public int RecordId { get; set; }
        public int TagId { get; set; }

        public virtual Tag Tag { get; set; }
        public virtual Record Record { get; set; }
    }
}
