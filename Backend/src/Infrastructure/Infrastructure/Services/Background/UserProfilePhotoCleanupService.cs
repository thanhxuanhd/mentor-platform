using Contract.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Services.Background
{
    public class UserProfilePhotoCleanupService(IServiceProvider serviceProvider, IWebHostEnvironment env) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                //var now = DateTime.Now;
                //var nextRun = now.AddMinutes(1);
                //var delay = nextRun - now;
                using (var scope = serviceProvider.CreateScope())
                {
                    var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                    var pendingUsers = await userRepository
                        .GetPendingUsersAsync();

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
                                    Console.WriteLine($"Failed to delete file {file}: {ex.Message}");
                                }
                            }
                        }
                    }

                }

                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
        }
    }
}
