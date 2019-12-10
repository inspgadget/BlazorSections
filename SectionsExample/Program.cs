using Autofac;
using Autofac.Extensions.DependencyInjection;
using BlazorSectionLib;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace SectionsExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory(Register))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void Register(ContainerBuilder builder)
        {
            builder.RegisterType<SectionService>().AsSelf().InstancePerLifetimeScope();
        }
    }
}