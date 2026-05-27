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

        public List<Report> GetMine(int reporterUserId)
        {
            using var db = new AppDbContext();
            return db.Reports
                .Where(r => r.ReporterUserId == reporterUserId)
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

        public (CreateReportResult Result, Report? Report) Create(int reporterUserId, CreateReportRequest request)
        {
            using var db = new AppDbContext();

            if (reporterUserId == request.ReportedUserId)
                return (CreateReportResult.SelfReport, null);

            var reporterUser = db.Users.FirstOrDefault(u => u.Id == reporterUserId);
            if (reporterUser == null)
                return (CreateReportResult.ReporterUserNotFound, null);

            var reportedUser = db.Users.FirstOrDefault(u => u.Id == request.ReportedUserId);
            if (reportedUser == null)
                return (CreateReportResult.ReportedUserNotFound, null);

            var duplicateOpenReport = db.Reports.Any(r =>
                r.ReporterUserId == reporterUserId &&
                r.ReportedUserId == request.ReportedUserId &&
                (r.Status == "open" || r.Status == "in_review"));

            if (duplicateOpenReport)
                return (CreateReportResult.DuplicateOpenReport, null);

            var report = new Report
            {
                ReportedUserId = request.ReportedUserId,
                ReportedUsername = reportedUser.Username,
                ReporterUserId = reporterUserId,
                ReporterUsername = reporterUser.Username,
                Reason = request.Reason.Trim(),
                Status = "open",
                CreatedAt = DateTime.UtcNow
            };

            db.Reports.Add(report);
            db.SaveChanges();
            return (CreateReportResult.Success, report);
        }

        public (UpdateReportStatusResult Result, Report? Report) SetStatus(
            int id,
            string status,
            int reviewedByUserId,
            string reviewedByUsername,
            string? moderatorComment)
        {
            using var db = new AppDbContext();

            var report = db.Reports.FirstOrDefault(r => r.Id == id);
            if (report == null)
                return (UpdateReportStatusResult.NotFound, null);

            var nextStatus = status.Trim().ToLower();
            if (nextStatus != "in_review" &&
                nextStatus != "resolved" &&
                nextStatus != "rejected")
            {
                return (UpdateReportStatusResult.InvalidStatus, null);
            }

            if (report.Status == "resolved" || report.Status == "rejected")
                return (UpdateReportStatusResult.AlreadyClosed, null);

            report.Status = nextStatus;
            report.StatusChangedAt = DateTime.UtcNow;
            report.ReviewedByUserId = reviewedByUserId;
            report.ReviewedByUsername = reviewedByUsername;

            if (nextStatus == "resolved" || nextStatus == "rejected")
            {
                report.ResolvedAt = DateTime.UtcNow;
                report.ModeratorComment = moderatorComment?.Trim() ?? string.Empty;
            }
            else
            {
                report.ResolvedAt = null;
                report.ModeratorComment = string.Empty;
            }

            db.SaveChanges();
            return (UpdateReportStatusResult.Success, report);
        }

        public int CountOpen()
        {
            using var db = new AppDbContext();
            return db.Reports.Count(r => r.Status == "open");
        }
    }

    public enum CreateReportResult
    {
        Success,
        ReporterUserNotFound,
        ReportedUserNotFound,
        SelfReport,
        DuplicateOpenReport
    }

    public enum UpdateReportStatusResult
    {
        Success,
        NotFound,
        InvalidStatus,
        AlreadyClosed
    }
}
