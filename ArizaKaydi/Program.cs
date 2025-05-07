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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
