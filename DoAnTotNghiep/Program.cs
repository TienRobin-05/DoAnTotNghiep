using DoAnTotNghiep.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Đăng ký session
builder.Services.AddSession();

// Đăng ký DAL
builder.Services.AddScoped<Tai_Khoan_DAL>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

// Dùng session
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Tai_Khoan}/{action=DangNhap}/{id?}");

app.Run();