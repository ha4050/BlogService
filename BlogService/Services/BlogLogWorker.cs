using BlogService.Data;
using BlogService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlogService.BackgroundServices
{
    public class BlogLogWorker : IHostedService
    {
        private readonly ILogger<BlogLogWorker> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly TimeSpan _interval; // Interval between processing websites

        public BlogLogWorker(ILogger<BlogLogWorker> logger, ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _logger = logger;
            _dbContext = dbContext;

          //  Read interval from configuration (optional)
           _interval = TimeSpan.FromMinutes(configuration.GetValue<int>("BlogLogProcessingInterval", 1));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("BlogLogWorker started.");
            while (!cancellationToken.IsCancellationRequested)
            {
                await ProcessWebsitesAsync(cancellationToken);
                await Task.Delay(_interval, cancellationToken); // Wait for the specified interval
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("BlogLogWorker stopping.");
            return Task.CompletedTask;
        }

        private async Task ProcessWebsitesAsync(CancellationToken cancellationToken)
        {
            var websites = await _dbContext.WebsiteDatas.ToListAsync(cancellationToken); // Efficiently retrieve all websites

            foreach (var website in websites)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                await ProcessBlogInfoAsync(website, cancellationToken);
            }
        }

        private async Task ProcessBlogInfoAsync(WebsiteData website, CancellationToken cancellationToken)
        {
            var blogInfo = await this._dbContext.BlogInfos
                .Where(b => b.WebsiteId == website.Id && !string.IsNullOrWhiteSpace(b.BlogTopics))
                .FirstOrDefaultAsync(cancellationToken);

            if (blogInfo != null)
            {
                await ProcessBlogTopicsAsync(website, blogInfo, cancellationToken);
            }
        }

        private async Task ProcessBlogTopicsAsync(WebsiteData website, BlogInfo blogInfo, CancellationToken cancellationToken)
        {
            var topics = blogInfo.BlogTopics
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .ToList();

            var lastInsertedTime = await _dbContext.BlogLogs
                .Where(b => b.WebsiteId == website.Id)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => (DateTime?)b.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            lastInsertedTime = lastInsertedTime ?? DateTime.MinValue;

            if (DateTime.Now >= lastInsertedTime.Value.Add(_interval))
            {
                var nextKeywordIndex = GetNextKeywordIndex(topics);

                if (nextKeywordIndex >= 0)
                {
                    var blogLog = new BlogLog
                    {
                        WebsiteId = website.Id,
                        Keyword = topics[nextKeywordIndex],
                        CreatedAt = DateTime.Now
                    };

                    _dbContext.BlogLogs.Add(blogLog);
                    await _dbContext.SaveChangesAsync(cancellationToken);

                    _logger.LogInformation($"Inserted: {topics[nextKeywordIndex]} for website: {website.Url} at {DateTime.Now}");
                }
            }
        }

        private static int GetNextKeywordIndex(List<string> topics)
        {
          //  Optimized logic for efficiency
            for (int i = 0; i < topics.Count; i++)
            {
                //if (_dbContext.BlogLogs.Any(b => b.Keyword == topics[i]))
                //{
                //    continue;
                //}
                return i;
            }

            return -1;
        }
    }
}