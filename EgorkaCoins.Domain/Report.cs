using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace EgorkaCoins.Domain
{
    public class Report
    {
        [Key]
        public int Id { get; set; }
        public int ReportedUserId { get; set; }
        public string ReportedUsername { get; set; } = string.Empty;
        public int ReporterUserId { get; set; }
        public string ReporterUsername { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "open";
        public DateTime CreatedAt { get; set; }
        public DateTime? StatusChangedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string ModeratorComment { get; set; } = string.Empty;
        public int? ReviewedByUserId { get; set; }
        public string ReviewedByUsername { get; set; } = string.Empty;
    }
}
