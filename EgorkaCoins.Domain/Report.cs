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
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "open"; 
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}