using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using ZooWeb.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.Cookie.Name = "access";
    //options.LoginPath = "/Index";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin", policy => policy.RequireClaim("user", "admin"));
    options.AddPolicy("zookeeper", policy => policy.RequireClaim("user", "zookeeper"));
});

builder.Services.AddRazorPages();

builder.Services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<AmenitySaleRepository>();
builder.Services.AddScoped<AnimalRepository>();
builder.Services.AddScoped<DepartmentRepository>();
builder.Services.AddScoped<EmployeeRepository>();
builder.Services.AddScoped<EnclosureRepository>();
builder.Services.AddScoped<FeedingPatternRepository>();
builder.Services.AddScoped<FeedingReportRepository>();
builder.Services.AddScoped<NotificationRepository>();
builder.Services.AddScoped<RestrictedRepository>();
builder.Services.AddScoped<RevenueRepository>();
builder.Services.AddScoped<TicketFinderRepository>();
builder.Services.AddScoped<TicketSaleRepository>();
builder.Services.AddScoped<VisitorRepository>();
builder.Services.AddScoped<ZooProfileRepository>();
builder.Services.AddScoped<ZooUserRepository>();
builder.Services.AddScoped<ZooUserRoleRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapDefaultControllerRoute();

app.Run();
