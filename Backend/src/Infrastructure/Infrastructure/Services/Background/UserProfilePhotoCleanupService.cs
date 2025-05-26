using Contract.Repositories;
using Domain.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Background
{
    public class UserProfilePhotoCleanupService(IServiceProvider serviceProvider, IWebHostEnvironment env, ILogger<UserProfilePhotoCleanupService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                var now = DateTime.Now;
                var nextRun = now.Date.AddDays(1);
                var delay = nextRun - now;

                using (var scope = serviceProvider.CreateScope())
                {
                    var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                    var pendingUsers = userRepository
                        .GetAll().Where(u => u.Status == UserStatus.Pending).ToList();

                    var imagesDir = Path.Combine(env.WebRootPath, "images");

                    if (pendingUsers.Count == 0 || !Directory.Exists(imagesDir))
                    {
                        await Task.Delay(delay, stoppingToken);
                        continue;
                    }

                    foreach (var user in pendingUsers)
                    {
                        var userIdStr = user.Id.ToString();
                        var files = Directory.GetFiles(imagesDir)
                            .Where(f => Path.GetFileNameWithoutExtension(f)
                            .Split("_")[0].Equals(userIdStr, StringComparison.OrdinalIgnoreCase));

                        foreach (var file in files)
                        {
                            try
                            {
                                File.Delete(file);
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex, $"Failed to delete file {file}");
                            }
                        }
                    }
                }
                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
