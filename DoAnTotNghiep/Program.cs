using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Services;
using Microsoft.AspNetCore.DataProtection;

// Khởi tạo builder để đăng ký service, đọc cấu hình và chuẩn bị tạo ứng dụng ASP.NET Core MVC.
var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Đăng ký MVC để ứng dụng sử dụng Controller xử lý request và View Razor hiển thị giao diện.
builder.Services.AddControllersWithViews();

var thuMucKhoaBaoVeDuLieu = Path.Combine(builder.Environment.ContentRootPath, "App_Data", "DataProtectionKeys");
Directory.CreateDirectory(thuMucKhoaBaoVeDuLieu);
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(thuMucKhoaBaoVeDuLieu))
    .SetApplicationName("DoAnTotNghiep");

// Cấu hình Session để lưu thông tin đăng nhập như mã tài khoản, họ tên và vai trò trong suốt phiên làm việc.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Đăng ký dependency injection dạng Scoped để mỗi request dùng một instance phù hợp của DAL/service.
builder.Services.AddScoped<TaiKhoan_DAL>();
builder.Services.AddScoped<HoSoSucKhoe_DAL>();
builder.Services.AddScoped<Vaccine_DAL>();
builder.Services.AddScoped<MuiTiemVaccine_DAL>();
builder.Services.AddScoped<LichTiem_DAL>();
// Đăng ký dependency injection dạng Scoped để mỗi request dùng một instance phù hợp của DAL/service.
builder.Services.AddScoped<LichSuTiem_DAL>();
builder.Services.AddScoped<ThongBao_DAL>();
builder.Services.AddScoped<PushSubscription_DAL>();
builder.Services.AddScoped<CauHoiTuVan_DAL>();
builder.Services.AddScoped<BaiVietCamNang_DAL>();
builder.Services.AddScoped<ThongKe_DAL>();
builder.Services.AddScoped<TaoLichTiemService>();
// Đăng ký dependency injection dạng Scoped để mỗi request dùng một instance phù hợp của DAL/service.
builder.Services.AddScoped<ThongBaoNhacLichService>();
builder.Services.AddScoped<PushNotificationService>();

// Tạo đối tượng app sau khi đã hoàn tất đăng ký service và cấu hình ban đầu.
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        scope.ServiceProvider.GetRequiredService<PushSubscription_DAL>().KhoiTaoBangNeuChuaCo();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Khong khoi tao duoc bang PushSubscription. Chuc nang push se thu lai khi nguoi dung dang ky.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

// Bật middleware phục vụ file tĩnh như CSS, JavaScript, hình ảnh trong thư mục wwwroot.
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = context =>
    {
        if (context.File.Name == "push-service-worker.js")
        {
            context.Context.Response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
            context.Context.Response.Headers.Pragma = "no-cache";
            context.Context.Response.Headers.Expires = "0";
            context.Context.Response.Headers["Service-Worker-Allowed"] = "/";
        }
    }
});
// Bật middleware định tuyến để ASP.NET Core xác định Controller/Action cần chạy cho mỗi URL.
app.UseRouting();
// Bật middleware Session trước khi Controller chạy để có thể đọc/ghi dữ liệu phiên đăng nhập.
app.UseSession();
// Bật middleware phân quyền, chuẩn bị cho các chức năng cần kiểm soát quyền truy cập.
app.UseAuthorization();

// Khai báo route mặc định: nếu URL không nêu rõ controller/action thì hệ thống mở Home/Index.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
