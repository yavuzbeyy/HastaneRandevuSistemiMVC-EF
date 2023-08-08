using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Razor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Home/GirisYap";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

builder.Services.AddLocalization(opt =>
{
    opt.ResourcesPath = "Resources";
});


builder.Services.AddMvc().AddMvcLocalization(LanguageViewLocationExpanderFormat.Suffix).
    AddDataAnnotationsLocalization();

var app = builder.Build();

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

var supportedCulteres = new[] { "tr", "en" };
var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCulteres[0]).AddSupportedCultures(supportedCulteres)
    .AddSupportedUICultures(supportedCulteres);
app.UseRequestLocalization(localizationOptions);


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();