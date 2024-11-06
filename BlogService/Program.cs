using BlogService.Data;
using BlogService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                });
            })
            .Build();
        InsertBlogData(host);
        // Apply migrations and start the service
        using (var scope = host.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
          // InsertBlogData(dbContext);
        }
    }

    static void InsertBlogData(IHost host)
    {
        int intervalInSeconds = 5; // Time interval in seconds

        while (true) // Service runs indefinitely
        {
            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                // InsertBlogData(dbContext);


                // Fetch fresh data from the database for each iteration
                var websites = dbContext.WebsiteDatas.ToList();

                foreach (var website in websites)
                {
                    var blogInfo = dbContext.BlogInfos
                        .FirstOrDefault(b => b.WebsiteId == website.Id && !string.IsNullOrWhiteSpace(b.BlogTopics));

                    if (blogInfo != null)
                    {
                        var topics = blogInfo.BlogTopics
                            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(t => t.Trim())
                            .ToList();

                        var lastInsertedTime = dbContext.BlogLogs
                            .Where(b => b.WebsiteId == website.Id)
                            .OrderByDescending(b => b.CreatedAt)
                            .Select(b => (DateTime?)b.CreatedAt)
                            .FirstOrDefault();

                        lastInsertedTime = lastInsertedTime ?? DateTime.MinValue;

                        // Check if the time since the last insertion has exceeded the interval
                        if (DateTime.Now >= lastInsertedTime.Value.AddSeconds(intervalInSeconds))
                        {
                            var nextKeywordIndex = GetNextKeywordIndex(dbContext, website.Id, topics);

                            // Insert the next keyword
                            if (nextKeywordIndex >= 0)
                            {
                                var blogLog = new BlogLog
                                {
                                    WebsiteId = website.Id,
                                    Keyword = topics[nextKeywordIndex],
                                    CreatedAt = DateTime.Now,
                                };

                                dbContext.BlogLogs.Add(blogLog);
                                dbContext.SaveChanges();

                                Console.WriteLine($"Inserted: {topics[nextKeywordIndex]} for website: {website.Url} at {DateTime.Now}");
                            }
                        }
                    }
                }

                // Sleep for 5 seconds before the next iteration
                Thread.Sleep(5000);
            }
        }
    }

    static int GetNextKeywordIndex(ApplicationDbContext dbContext, int websiteId, List<string> topics)
    {
        // Get all inserted keywords for the website ordered by CreatedAt
        var insertedKeywords = dbContext.BlogLogs
            .Where(b => b.WebsiteId == websiteId)
            .OrderBy(b => b.CreatedAt)
            .Select(b => b.Keyword)
            .ToList();

        // Determine how many times the full list of topics has been inserted
        int cyclesCompleted = insertedKeywords.Count / topics.Count;

        // Get the next keyword index in the current cycle
        int nextKeywordIndex = insertedKeywords.Count % topics.Count;

        // If the current keyword has been inserted the same number of times as the previous cycle, return its index
        if (nextKeywordIndex < topics.Count)
        {
            return nextKeywordIndex;
        }

        // If all topics have been inserted, start from the beginning
        return 0;
    }
}

