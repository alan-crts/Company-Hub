using CompanyHub.Context;
using CompanyHub.Services;
using CompanyHub.Services.Employee;
using CompanyHub.Services.Service;
using CompanyHub.Services.Site;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5000");

builder.Services.AddEntityFrameworkNpgsql()
    .AddDbContext<MainContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("DbConnection")));

// Add services to the container.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";
        options.LogoutPath = "/Home/Logout";
        options.Cookie.Name = "CompanyHub";
    });
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<ISiteService, SiteService>();

var app = builder.Build();
app.UseStatusCodePagesWithReExecute("/error/{0}");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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
app.UseCookiePolicy();


app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{page?}");

app.Run();