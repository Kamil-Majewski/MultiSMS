using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiSMS.Interface;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Initialization;
using MultiSMS.BusinessLogic.Initialization;
using MultiSMS.BusinessLogic.Settings;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.MVC.Models;
using MultiSMS.MVC.Areas;
using MultiSMS.MVC.Middlewares;
using MultiSMS.MVC.Hubs;
using MultiSMS.BusinessLogic.MappingConfig;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IPathProvider, PathProvider>();

var configuration = new ConfigurationBuilder()
   .SetBasePath(builder.Environment.ContentRootPath)
   .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
.Build();

builder.Services.AddOptions();

var multiSMSConnectionString = builder.Configuration.GetConnectionString("MultiSMSConnectionString") ?? throw new InvalidOperationException($"Could not get database connection string");
builder.Services.AddDbContext<MultiSMSDbContext>(options =>
    options.UseSqlServer(multiSMSConnectionString));

builder.Services.Configure<EmailSettings>(configuration.GetSection("MailSettings"));
builder.Services.Configure<ServerSmsSettings>(configuration.GetSection("SMSServerSettings"));
builder.Services.Configure<SmsApiSettings>(configuration.GetSection("SMSAPI"));
builder.Services.Configure<MProfiSettings>(configuration.GetSection("MProfiSettings"));
builder.Services.Configure<ApiSettingsSettings>(configuration.GetSection("ApiSettingsSettings"));

builder.Services.AddAutoMapper(typeof(AutomapperProfile).Assembly);

builder.Services.AddIdentity<User, IdentityRole<int>>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<MultiSMSDbContext>()
    .AddDefaultUI()
    .AddErrorDescriber<CustomIdentityErrorDescriber>()
    .AddRoles<IdentityRole<int>>()
    .AddRoleManager<RoleManager<IdentityRole<int>>>()
    .AddUserManager<UserManager<User>>()
    .AddDefaultTokenProviders();

builder.Services.InitializeInfrastructureDependencies();
builder.Services.AddSingleton<IProgressRelay, ProgressRelay>();
builder.Services.InitializeBusinessLogicDependencies();

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddRazorPages();

var app = builder.Build();

app.MapHub<ProgressHub>("/progressHub");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.MapAreaControllerRoute(
    name: "Identity",
    areaName: "Identity",
    pattern: "Identity/{controller=Home}/{action=Index}/{id?}");

var mapper = app.Services.GetRequiredService<IMapper>();
mapper.ConfigurationProvider.AssertConfigurationIsValid();

app.Run();
