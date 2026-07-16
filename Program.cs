using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using BloodBankNetwork.Data;
using BloodBankNetwork.Controllers; // 🌟 Custom Filter namespace call karne ke liye
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DbContext Connection string configure
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Server Session services configuration
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 🔒 GLOBAL PIPELINE SECURITY: Website lock aur Auto-Logout check ek sath handle honge
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
    
    options.Filters.Add(new AuthorizeFilter(policy)); // Authorization force lock
    
    // 🌟 AUTOMATIC TAB CLOSE TIMEOUT FILTER REGISTERED GLOBALLY:
    // Ab yeh filter har controller hit hone par session khud check karega, manual code ki zaroorat nahi!
    options.Filters.Add(new SessionTimeoutAttribute()); 
});

// Authentication Cookie Configuration
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        
        options.Cookie.Name = "BloodBankNetwork_Session";
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true; 
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        
        options.SlidingExpiration = false; 
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Middleware Pipeline (Order is very important!)
app.UseSession();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();