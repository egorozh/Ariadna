using System;
using System.IO;
using Autofac;
using Serilog;
using Serilog.Events;

namespace Ariadna
{
    public static class IocExtensions
    {
        public static void AddConfiguration(this ContainerBuilder builder, DirectoryInfo rootDirectory)
        {
            var basePath = Path.Combine(rootDirectory.FullName, "Config");

            builder.RegisterType<MagicOptions>().WithParameters(new[]
            {
                new NamedParameter("basePath", basePath),
                new NamedParameter("configFileName", "appsettings.json"),
            }).As<IMagicOptions>().SingleInstance();
        }

        public static void AddLogger(this ContainerBuilder builder, IObserver<LogEvent> observer)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("Logs\\app_logger.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.Observers(events =>
                    events.Subscribe(observer))
                .CreateLogger();
            
            builder.RegisterInstance(Log.Logger).As<ILogger>().SingleInstance();
        }
    }
}