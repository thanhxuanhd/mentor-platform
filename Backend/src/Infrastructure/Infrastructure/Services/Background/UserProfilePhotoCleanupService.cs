using Contract.Repositories;
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
                        .GetAll().Where(u => u.Status == Domain.Enums.UserStatus.Pending).ToList();

                    if (pendingUsers.Count != 0)
                    {
                        foreach (var user in pendingUsers)
                        {
                            var imagesDir = Path.Combine(env.WebRootPath, "images");
                            if (!Directory.Exists(imagesDir))
                            {
                                continue;
                            }

                            var userIdStr = user.Id.ToString();
                            var files = Directory.GetFiles(imagesDir)
                                .Where(f => Path.GetFileName(f).Contains(userIdStr, StringComparison.OrdinalIgnoreCase));

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

                }

                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
