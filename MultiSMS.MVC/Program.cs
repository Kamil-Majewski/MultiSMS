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


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddDefaultIdentity<Administrator>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<MultiSMSDbContext>()
    .AddErrorDescriber<CustomIdentityErrorDescriber>()
    .AddDefaultTokenProviders();

builder.Services.InitializeInfrastructureDependencies();
builder.Services.InitializeBusinessLogicDependencies();

builder.Services.AddControllersWithViews();

var app = builder.Build();

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

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

var mapper = app.Services.GetRequiredService<IMapper>();
mapper.ConfigurationProvider.AssertConfigurationIsValid();

app.Run();
