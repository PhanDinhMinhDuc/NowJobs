using System.Collections.Generic;
using System;
using System.Linq;
using System.Data.Entity;

public class MatchingService
{
    private readonly ApplicationDbContext _db;

    public MatchingService(ApplicationDbContext db)
    {
        _db = db;
    }

    public MatchingResult CalculateMatchingScore(int jobId, int candidateProfileId)
    {
        var job = _db.Jobs
            .Include(j => j.JobJobSkills.Select(cs => cs.JobSkill))
            .Include(j => j.JobCategory)
            .Include(j => j.Experience)
            .Include(j => j.Location)
            .FirstOrDefault(j => j.JobID == jobId);

        var candidate = _db.CandidateProfiles
            .Include(c => c.CandidateProfileSkills.Select(cs => cs.Skill))
            .Include(c => c.JobCategory)
            .Include(c => c.Experience)
            .Include(c => c.Location)
            .FirstOrDefault(c => c.ID == candidateProfileId);

        if (job == null || candidate == null)
        {
            return null;
        }

        var result = new MatchingResult
        {
            JobId = jobId,
            CandidateProfileId = candidateProfileId
        };

        // 1. Tính điểm kinh nghiệm (30% tổng điểm)
        result.ExperienceScore = CalculateExperienceScore(job.ExperienceID, candidate.ExperienceID);
        result.Score += (double)result.ExperienceScore * 0.3;

        // 2. Tính điểm kỹ năng (40% tổng điểm)
        var jobSkillIds = job.JobJobSkills.Select(js => js.JobSkillId).ToList();
        var candidateSkillIds = candidate.CandidateProfileSkills.Select(cs => cs.SkillID).ToList();
        result.SkillScore = CalculateSkillScore(jobSkillIds, candidateSkillIds);
        result.Score += (double)result.SkillScore * 0.4;

        // 3. Tính điểm địa điểm (10% tổng điểm)
        result.LocationScore = CalculateLocationScore(job.LocationID, candidate.LocationID);
        result.Score += (double)result.LocationScore * 0.1;

        // 4. Tính điểm ngành nghề (20% tổng điểm)
        result.JobCategoryScore = CalculateJobCategoryScore(job.JobCategoryID, candidate.JobCategoryID);
        result.Score += (double)result.JobCategoryScore * 0.2;

        // Làm tròn điểm tổng
        result.Score = Math.Round(result.Score, 2);

        return result;
    }

    private decimal CalculateExperienceScore(int? jobExperienceId, int? candidateExperienceId)
    {
        if (!jobExperienceId.HasValue || !candidateExperienceId.HasValue)
            return 0;

        var jobExperience = _db.Experiences.Find(jobExperienceId.Value);
        var candidateExperience = _db.Experiences.Find(candidateExperienceId.Value);

        if (jobExperience == null || candidateExperience == null)
            return 0;

        // Giả sử ExperienceID càng cao thì kinh nghiệm càng nhiều
        if (candidateExperienceId >= jobExperienceId)
        {
            return 100; // Ứng viên đáp ứng hoặc vượt yêu cầu kinh nghiệm
        }
        else
        {
            // Tính tỷ lệ phần trăm kinh nghiệm so với yêu cầu
            return (decimal)candidateExperienceId.Value / jobExperienceId.Value * 100;
        }
    }

    private decimal CalculateSkillScore(List<int> jobSkillIds, List<int> candidateSkillIds)
    {
        if (jobSkillIds == null || !jobSkillIds.Any())
            return 0;

        if (candidateSkillIds == null || !candidateSkillIds.Any())
            return 0;

        var matchedSkills = jobSkillIds.Intersect(candidateSkillIds).Count();
        return (decimal)matchedSkills / jobSkillIds.Count * 100;
    }

    private decimal CalculateLocationScore(int? jobLocationId, int? candidateLocationId)
    {
        if (!jobLocationId.HasValue || !candidateLocationId.HasValue)
            return 0;

        return jobLocationId == candidateLocationId ? 100 : 0;
    }

    private decimal CalculateJobCategoryScore(int? jobCategoryId, int? candidateJobCategoryId)
    {
        if (!jobCategoryId.HasValue || !candidateJobCategoryId.HasValue)
            return 0;

        return jobCategoryId == candidateJobCategoryId ? 100 : 0;
    }
}

public class MatchingResult
{
    public int JobId { get; set; }
    public int CandidateProfileId { get; set; }
    public double Score { get; set; } // Điểm tổng (0-100)
    public decimal ExperienceScore { get; set; }
    public decimal SkillScore { get; set; }
    public decimal LocationScore { get; set; }
    public decimal JobCategoryScore { get; set; }
}