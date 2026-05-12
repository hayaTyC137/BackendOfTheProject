using System;
using System.Collections.Generic;
using System.Text;
using EgorkaCoins.DataAccess.Context;
using EgorkaCoins.Domain;
using EgorkaCoins.Helpers.DTOs;

namespace EgorkaCoins.BusinessLogic.Core
{
    public class ReportActions
    {
        public List<Report> GetAll()
        {
            using var db = new AppDbContext();
            return db.Reports
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }

        public List<Report> GetOpen()
        {
            using var db = new AppDbContext();
            return db.Reports
                .Where(r => r.Status == "open")
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }

        public Report? Create(int reporterUserId, CreateReportRequest request)
        {
            using var db = new AppDbContext();

            var reportedUser = db.Users.FirstOrDefault(u => u.Id == request.ReportedUserId);
            if (reportedUser == null) return null;

            var report = new Report
            {
                ReportedUserId = request.ReportedUserId,
                ReportedUsername = reportedUser.Username,
                ReporterUserId = reporterUserId,
                Reason = request.Reason,
                Status = "open",
                CreatedAt = DateTime.UtcNow
            };

            db.Reports.Add(report);
            db.SaveChanges();
            return report;
        }

        public Report? Resolve(int id)
        {
            using var db = new AppDbContext();

            var report = db.Reports.FirstOrDefault(r => r.Id == id);
            if (report == null) return null;

            report.Status = "resolved";
            report.ResolvedAt = DateTime.UtcNow;
            db.SaveChanges();
            return report;
        }

        public int CountOpen()
        {
            using var db = new AppDbContext();
            return db.Reports.Count(r => r.Status == "open");
        }
    }
}