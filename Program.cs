using AccountingSuite.Data;
using AccountingSuite.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Register DbHelper with connection string
builder.Services.AddScoped<DbHelper>(item => new DbHelper(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<DbHelperAsync>();
builder.Services.AddScoped<PartyRepository>();
builder.Services.AddScoped<RegionRepository>();
builder.Services.AddScoped<StateRepository>();
builder.Services.AddScoped<BranchRepository>();
builder.Services.AddScoped<AccountHeadRepository>();
builder.Services.AddScoped<AuditRepository>();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
