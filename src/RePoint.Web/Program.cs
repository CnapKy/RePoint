using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RePoint.Web.Data;
using RePoint.Web.Models;
using RePoint.Web.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("RePointDb")
    ?? throw new InvalidOperationException("Missing ConnectionStrings:RePointDb. Set a local user secret.");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Development only: add email confirmation before public deployment.
    options.Password.RequiredLength = 10;
    options.Password.RequireNonAlphanumeric = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
});
builder.Services.AddScoped<ICollectionPointService, CollectionPointService>();
builder.Services.AddScoped<IHandoverPolicy, HandoverPolicy>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error/500");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}
app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();

public partial class Program { }
