using API;
using FUNDAMENTOS5_API;

IHostBuilder CreateHostBuilder(string[] ars)
{
    return Host.CreateDefaultBuilder(ars)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    });
}

CreateHostBuilder(args).Build().Run();