using ArizaKaydi;
using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using DataAccessLayer.Repository;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(config =>
	{
		var policy = new AuthorizationPolicyBuilder()
		.RequireAuthenticatedUser()
			.Build();
		config.Filters.Add(new AuthorizeFilter(policy));
	});
builder.Services.AddScoped<IErrorDal, EFErrorDal>();
builder.Services.AddScoped<IImageDalCollection, EFImageCollectionDal>();
builder.Services.AddScoped<ErrorManager>();
builder.Services.AddScoped<ImageCollectionManager>();
builder.Services.AddScoped(typeof(IGenericDal<>), typeof(GenericRepository<>));


builder.Services.AddDbContext<context>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions => sqlOptions.CommandTimeout(3000)));
builder.Services.AddIdentity<panelUser, panelUserRole>(options =>
{
	options.Password.RequireDigit = false;
	options.Password.RequiredLength = 0;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireUppercase = false;
	options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<context>() // << BU SATIR KRÝTÝK
.AddDefaultTokenProviders();
/*.Services.AddMvc(config =>
{
	var policy = new AuthorizationPolicyBuilder()
	.RequireAuthenticatedUser()
		.Build();
	config.Filters.Add(new AuthorizeFilter(policy));
});*/
//builder.Services.AddMvc();
builder.Services.AddAuthentication(
	CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(x =>
	{
		x.LoginPath = "/Login/Index";
	});
builder.Services.ConfigureApplicationCookie(options =>
{
	options.Cookie.HttpOnly = true;
	options.LoginPath = "/Login/Index";
	options.LogoutPath = "/Login/Logout";
	options.AccessDeniedPath = "/ErrorPage/AccessDenied";
	options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
	options.Events.OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync;
});
builder.Services.AddAuthorization(options =>
{
	// Sadece "UserManagement.View" Claim'ine sahip olanlarýn eriþebileceði bir politika
	options.AddPolicy("CanEditMobileUserManagement", policy =>
		policy.RequireClaim("Permission", "MobileUserManagement.Edit"));
	options.AddPolicy("CanViewMobileUserManagement", policy =>
		policy.RequireClaim("Permission", "MobileUserManagement.View"));
	// Hem "UserManagement.View" hem de "UserManagement.Edit" Claim'lerine sahip olanlar
	options.AddPolicy("CanEditPanelUserManagement", policy =>
		policy.RequireClaim("Permission", "PanelUserManagement.Edit"));
	options.AddPolicy("CanViewPanelUserManagement", policy =>
		policy.RequireClaim("Permission", "PanelUserManagement.View"));
	options.AddPolicy("BasicModeratorEditPermission", policy =>
		policy.RequireClaim("Permission", "BasicModerator.Edit"));
	options.AddPolicy("BasicModeratorViewPermission", policy =>
		policy.RequireClaim("Permission", "BasicModerator.View"));


});
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
	// This will check the security stamp every 30 seconds
	options.ValidationInterval = TimeSpan.FromMinutes(10);
});



var app = builder.Build();

// Configure the HTTP request pipeline.
// Program.cs (ASP.NET Core 6+)
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/ErrorPage/TheErrorView");
	app.UseHsts(); // HTTPS için
}
else
{
	app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseMiddleware<DynamicClaimsMiddleware>();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
