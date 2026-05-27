using System;
using System.Collections.Generic;
using System.Text;

namespace EgorkaCoins.Helpers.DTOs
{
    public class CreateReportRequest
    {
        public int ReportedUserId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}