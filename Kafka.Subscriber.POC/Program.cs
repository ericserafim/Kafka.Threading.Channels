using Kafka.POC.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using System.Text.Json;

namespace Kafka.POC
{
	class Program
	{
    public static void Main(string[] args)
    {
      Console.WriteLine($"{Assembly.GetExecutingAssembly().GetName()}");

      try
      {
        CreateHostBuilder(args).Build().Run();
      }
      catch (Exception e)
      {
        Console.WriteLine($"Server error => {e.Message}");        
      }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
              webBuilder.UseStartup<Startup>();
            });
  }
}