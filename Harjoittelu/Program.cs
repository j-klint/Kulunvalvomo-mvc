using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Harjoittelu.Data;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// nginx ohjeista. Mitä sitten tekee?
//// Configure forwarded headers, jos tarpeen
//builder.Services.Configure<ForwardedHeadersOptions>(options =>
//{
//    options.KnownProxies.Add(IPAddress.Parse("10.0.0.100"));
//});
//builder.Services.AddAuthentication();

builder.Services.AddDbContext<HarjoitteluContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HarjoitteluContext") ?? throw new InvalidOperationException("Connection string 'HarjoitteluContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();


// nginx "reverse proxy server"iä varten
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
//app.UseAuthentication();


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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
