# Hướng dẫn database cho đồ án

## File cần nộp kèm

- `doAnTotNghiep.bak`: backup đầy đủ database `doAnTotNghiep`, gồm cấu trúc bảng, khóa chính, khóa ngoại và dữ liệu mẫu.
- `PushSubscription.sql`: script phụ cho bảng đăng ký nhận thông báo đẩy, giữ lại để đối chiếu hoặc chạy bổ sung nếu cần.
- `CapNhat_BaiViet_TinTuc_CamNang.sql`: script cập nhật bảng bài viết để hỗ trợ loại bài viết và ảnh đại diện.

## Cách khôi phục bằng SQL Server Management Studio

1. Mở SQL Server Management Studio và kết nối SQL Server local.
2. Nhấn chuột phải vào `Databases` -> `Restore Database...`.
3. Chọn `Device` -> `...` -> `Add` -> chọn file `doAnTotNghiep.bak`.
4. Ở ô `Database`, nhập hoặc chọn `doAnTotNghiep`.
5. Vào tab `Options`, tích `Overwrite the existing database (WITH REPLACE)` nếu máy đã có database cùng tên.
6. Nhấn `OK` để restore.

## Cấu hình kết nối của project

Project đang dùng connection string trong `appsettings.json`:

```json
"DefaultConnection": "Data Source=.;Initial Catalog=doAnTotNghiep;Integrated Security=True;TrustServerCertificate=True"
```

Nghĩa là ứng dụng sẽ kết nối tới SQL Server local bằng Windows Authentication và database tên `doAnTotNghiep`.

Nếu máy demo dùng SQL Server instance khác, ví dụ `.\SQLEXPRESS`, đổi `Data Source=.` thành:

```json
"Data Source=.\\SQLEXPRESS;Initial Catalog=doAnTotNghiep;Integrated Security=True;TrustServerCertificate=True"
```

## Dữ liệu mẫu hiện có

Backup hiện có các bảng chính:

- `TaiKhoan`: 3 dòng
- `Vaccine`: 7 dòng
- `MuiTiemVaccine`: 16 dòng
- `HoSoSucKhoe`: 7 dòng
- `LichTiem`: 98 dòng
- `LichSuTiem`: 4 dòng
- `ThongBao`: 53 dòng
- `CauHoiTuVan`: 1 dòng
- `BaiVietCamNang`: 4 dòng
- `PushSubscription`: 0 dòng

Riêng bảng `BaiVietCamNang` có thêm:

- `loaiBaiViet`: phân loại `Tin tức` hoặc `Cẩm nang`.
- `anhDaiDien`: lưu đường dẫn ảnh đại diện bài viết trong `wwwroot`.

## Khi bảo vệ nên nói ngắn gọn

Database của hệ thống được tách khỏi code. Code ASP.NET MVC chỉ lưu connection string, còn dữ liệu thực tế được khôi phục từ file backup SQL Server. Các thao tác đọc/ghi dữ liệu đi qua tầng `DAL`, dùng tham số SQL để tránh ghép chuỗi trực tiếp từ dữ liệu người dùng. Phần bài viết/cẩm nang cho phép quản trị viên phân loại bài và thêm ảnh đại diện để người dùng đọc tin tức, kiến thức tiêm chủng trực quan hơn.
