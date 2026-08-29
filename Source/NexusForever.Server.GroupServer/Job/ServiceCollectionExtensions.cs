using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace NexusForever.Server.GroupServer.Job
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScheduledJobs(this IServiceCollection sc)
        {
            sc.AddQuartz(c =>
            {
                c.ScheduleJob<CharacterDirtyRealmScheduledJob>(t => t
                    .StartNow()
                    .WithSimpleSchedule(s => s
                        .WithInterval(TimeSpan.FromSeconds(5))
                        .RepeatForever()));
                c.ScheduleJob<CharacterDirtyStatsScheduledJob>(t => t
                    .StartNow()
                    .WithSimpleSchedule(s => s
                        .WithInterval(TimeSpan.FromSeconds(2))
                        .RepeatForever()));
                c.ScheduleJob<GroupDirtyPositionScheduledJob>(t => t
                    .StartNow()
                    .WithSimpleSchedule(s => s
                        .WithInterval(TimeSpan.FromSeconds(5))
                        .RepeatForever()));
                c.ScheduleJob<GroupInviteExpiredScheduledJob>(t => t
                    .StartNow()
                    .WithSimpleSchedule(s => s
                        .WithInterval(TimeSpan.FromSeconds(1))
                        .RepeatForever()));
                c.ScheduleJob<GroupRequestExpiredScheduledJob>(t => t
                    .StartNow()
                    .WithSimpleSchedule(s => s
                        .WithInterval(TimeSpan.FromSeconds(1))
                        .RepeatForever()));
                c.ScheduleJob<OutboxScheduledJob>(t => t
                    .StartNow()
                    .WithSimpleSchedule(s => s
                        .WithInterval(TimeSpan.FromSeconds(1))
                        .RepeatForever()));
            });
            sc.AddQuartzHostedService();

            return sc;
        }
    }
}
