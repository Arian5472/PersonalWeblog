using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using Weblog.Core.Domain.IdentityEntities;
using Weblog.Infrastructure.DbContext;
using Weblog.Core.Domain.RepositoryContracts;
using Weblog.Infrastructure.Repositories;
using Weblog.Core.ServiceContracts;
using Weblog.Core.Services;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(services);
});

builder.Services.AddDbContext<ApplicationDbContext>(option => { option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); });
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options => { options.Password.RequireNonAlphanumeric = false; options.Password.RequiredUniqueChars = 0; options.Password.RequireDigit = false; options.Password.RequireLowercase = false; options.Password.RequireUppercase = false; }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders().AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>().AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.WebRootPath, "DataProtectionKeys"))).SetDefaultKeyLifetime(TimeSpan.FromDays(90));
builder.Services.AddScoped<ISubscribersRepository, SubscribersRepository>();
builder.Services.AddScoped<IPostsRepository, PostsRepository>();
builder.Services.AddScoped<IFilesService, FilesService>();
builder.Services.AddScoped<ISubscribersService, SubscribersService>();
builder.Services.AddScoped<IPostsService, PostsService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddAuthorization(option => { option.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build(); });
builder.Services.ConfigureApplicationCookie(options => { options.LoginPath = "/account/signin"; });
builder.Services.AddControllersWithViews();

var app = builder.Build();


if (app.Environment.IsDevelopment()) { app.UseDeveloperExceptionPage(); }
else { app.UseExceptionHandler("/Error"); }

app.UseStaticFiles();

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
