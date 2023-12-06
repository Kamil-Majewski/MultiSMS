using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiSMS.Interface;
using MultiSMS.Interface.Entities;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
   .SetBasePath(builder.Environment.ContentRootPath)
   .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
.Build();

builder.Services.AddOptions();

var multiSMSConnectionString = builder.Configuration.GetConnectionString("MultiSMSConnectionString") ?? throw new InvalidOperationException($"Could not get connection string for parliament election");
builder.Services.AddDbContext<MultiSMSDbContext>(options =>
    options.UseSqlServer(multiSMSConnectionString));

builder.Services.AddDefaultIdentity<Administrator>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<MultiSMSDbContext>()
    .AddDefaultTokenProviders();

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

app.Run();
