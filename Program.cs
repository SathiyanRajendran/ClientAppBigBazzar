using ClientAppBigBazzar.Helper;
using ClientAppBigBazzar.Interfaces;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddTransient<IForgotPassword, ForgotPasswordEmailSevice>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration.GetSection("SendGrid"));
//-----------------------------------
builder.Services.AddAuthentication()
    .AddFacebook(options =>
    {
        options.AppId = "test";
        options.AppSecret = "test";
    })
    .AddGoogle(options =>
    {
        options.ClientId = "test";
        options.ClientSecret = "test";
    });
//----------------------------------------
builder.Services.Configure<IdentityOptions>(opt =>
{
    opt.Password.RequiredLength = 8;
    opt.Password.RequireLowercase = true;
    opt.Lockout.DefaultLockoutTimeSpan=TimeSpan.FromSeconds(10);
    opt.Lockout.MaxFailedAccessAttempts = 5;
    opt.SignIn.RequireConfirmedAccount=true;
});
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
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
