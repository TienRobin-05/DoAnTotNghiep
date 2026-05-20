using DoAnTotNghiep.DAL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<coSoDuLieu>();
builder.Services.AddScoped<TaiKhoan_DAL>();
builder.Services.AddScoped<HoSoSucKhoe_DAL>();
builder.Services.AddScoped<Vaccine_DAL>();
builder.Services.AddScoped<taiKhoanDAL>();
builder.Services.AddScoped<hoSoSucKhoeDAL>();
builder.Services.AddScoped<vaccineDAL>();
builder.Services.AddScoped<muiTiemVaccineDAL>();
builder.Services.AddScoped<lichTiemDAL>();
builder.Services.AddScoped<lichSuTiemDAL>();
builder.Services.AddScoped<thongBaoDAL>();
builder.Services.AddScoped<baiVietCamNangDAL>();
builder.Services.AddScoped<cauHoiTuVanDAL>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
