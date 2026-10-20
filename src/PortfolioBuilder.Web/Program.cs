using Microsoft.AspNetCore.DataProtection;
using PortfolioBuilder.Web.Data;
using PortfolioBuilder.Web.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddFolderApplicationModelConvention("/Admin",
        model => model.Filters.Add(new AdminAuthorizeFilter()));
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.Name = ".Portfolio.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddSingleton<DbConnectionFactory>();
builder.Services.AddScoped<ProjectRepository>();
builder.Services.AddScoped<ContactMessageRepository>();
builder.Services.AddScoped<AdminUserRepository>();
builder.Services.AddScoped<AdminAuthService>();
builder.Services.AddScoped<SkillCategoryRepository>();
builder.Services.AddScoped<CodingHighlightRepository>();
var keysPath = builder.Configuration["DataProtection:KeysPath"];
if (!string.IsNullOrWhiteSpace(keysPath))
{
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
        .SetApplicationName("PortfolioBuilder");
}
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
