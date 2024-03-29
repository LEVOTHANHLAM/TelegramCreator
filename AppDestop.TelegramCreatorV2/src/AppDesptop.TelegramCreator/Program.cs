﻿using AppDesptop.TelegramCreator.Database.Entities;
using AppDesptop.TelegramCreator.Database.Repositories;
using AppDesptop.TelegramCreator.Forms;
using AppDesptop.TelegramCreator.Helper;
using AppDesptop.TelegramCreator.Interfaces;
using AppDesptop.TelegramCreator.ProxyDroid;
using AppDesptop.TelegramCreator.ProxyDroid.Interface;
using AppDestop.TelegramCreator.ScriptsTelegram;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
namespace AppDesptop.TelegramCreator
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        private static readonly IHost _host = CreateHostBuilder();
        [STAThread]
        static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                 .MinimumLevel.Debug()
                 .WriteTo.File("LOGSAPP/myapp.txt", rollingInterval: RollingInterval.Day)
                 .CreateLogger();
           
            string folder = Path.Combine(Environment.CurrentDirectory, "Data\\DataImport\\User");
            string folderSession = Path.Combine(Environment.CurrentDirectory, "Data\\DataImport\\Admin\\Session");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            if (!Directory.Exists(folderSession))
            {
                Directory.CreateDirectory(folderSession);
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            try
            {
                _host.Start();
                //Đoạn này mặc định của winform kệ nó thôi.
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                //Lấy ra cái Form1 đã được khai báo trong services
                try
                {
                    if (string.IsNullOrEmpty((string)Properties.Settings.Default["SetupEnvironment"]))
                    {
                        try
                        {
                            InstallerHelper.CheckAndInstallDependencies();
                        }
                        catch (Exception)
                        {
                        }
                        Properties.Settings.Default["SetupEnvironment"] = "Done";
                        Properties.Settings.Default.Save();
                    }
                    var form1 = _host.Services.GetRequiredService<fLogin>();
                    //Lệnh chạy gốc là: Application.Run(new Form1);
                    //Đã được thay thế bằng lệnh sử dụng service khai báo trong host
                    Application.Run(form1);
                    Log.Information("Application start");
                }
                catch (Exception ex)
                {

                    Log.Error(ex.Message);
                    if (ex.InnerException != null)
                    {
                        Log.Error(ex.ToString());
                        Log.Error(ex.InnerException.Message);
                    }
                }

                //Khi form chính (form1) bị đóng <==> chương trình kết thúc ấy
                //thì dừng host
                _host.StopAsync().GetAwaiter().GetResult();
                //và giải phóng tài nguyên host đã sử dụng.
                _host.Dispose();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                if (ex.InnerException != null)
                {
                    Log.Error(ex.ToString());
                    Log.Error(ex.InnerException.Message);
                }
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        static IHost CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<fLogin>();
                    services.AddTransient<IAccountRepository, AccountRepository>();
                    services.AddSingleton<fMain>();
                    services.AddTransient<IProxyDroidHelper, ProxyDroidHelper>();
                    services.AddTransient<ITelegram, Telegram>();
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseSqlServer($"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={Path.Combine(Environment.CurrentDirectory, "Data\\Database\\DataAccount.mdf")};Integrated Security=True");
                        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                        options.EnableSensitiveDataLogging();
                    }, ServiceLifetime.Transient);
                }).Build();
        }
    }
}