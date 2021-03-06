﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using EWork.Data.Extensions;
using EWork.Data.Interfaces;
using EWork.Models;
using EWork.Services;
using EWork.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EWork.Data.Repositories
{
    public class JobRepository : IRepository<Job>
    {
        private readonly IFreelancingPlatformDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly ITagManager _tagManager;

        public JobRepository(IFreelancingPlatformDbContext db, UserManager<User> userManager, ITagManager tagManager)
        {
            _db = db;
            _userManager = userManager;
            _tagManager = tagManager;
        }

        public async Task AddAsync(Job job)
        {
            if (!await _userManager.IsInRoleAsync(job.Employer, job.Employer.Role))
                throw new AuthenticationException($"User must be {job.Employer.Role} in order to do this action.");

            try
            {
                _db.Employers.Attach(job.Employer);
                if (!(job.HiredFreelancer is null))
                    _db.Freelancers.Attach(job.HiredFreelancer);
            }
            catch (InvalidOperationException e)
            {
                Trace.WriteLine(e);
            }

            await _db.Jobs.AddAsync(job);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Job job)
        {
            if (job is null)
                return;

            var deletedJobsTags = job.JobTags.Select(jt => jt.Tag);

            _db.Jobs.Remove(job);
            await _tagManager.RemoveRangeAsync(deletedJobsTags);

            await _db.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<Job> items)
        {
            if (items is null)
                return;

            _db.Jobs.RemoveRange(items);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Job job)
        {
            _db.Entry(job).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task<Job> FindAsync(Predicate<Job> predicate) =>
            await GetAll().FirstOrDefaultAsync(job => predicate(job));

        public IQueryable<Job> GetAll() => _db.Jobs.ExtractAll();
    }
}