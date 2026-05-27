using System;
using System.Collections.Generic;
using System.Text;

namespace EgorkaCoins.Helpers.DTOs
{
    public class AdminStatsDto
    {
        public int TotalUsers { get; set; }
        public int OrdersToday { get; set; }
        public decimal TotalRevenue { get; set; }
        public int OpenReports { get; set; }
    }
}