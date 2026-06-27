## 📂 Cấu trúc cây thư mục dự án (Directory Tree)

```text
QuanLySanBong/
├── QuanLySanBong/              # Thư mục mã nguồn chính của dự án
│   ├── Controllers/            # Nơi xử lý logic điều hướng và nghiệp vụ
│   ├── Models/                 # Chứa các lớp đối tượng và ngữ cảnh Database (EF Core)
│   ├── Views/                  # Giao diện hiển thị (Đơn hàng, sân bóng, admin...)
│   │   ├── Home/
│   │   ├── Shared/
│   │   └── [Cac_View_Chuc_Nang]/
│   ├── wwwroot/                # Chứa tài nguyên tĩnh (CSS, JavaScript, Hình ảnh)
│   ├── appsettings.json        # File cấu hình chuỗi kết nối (Connection String)
│   └── Program.cs              # File khởi tạo và cấu hình dịch vụ hệ thống
├── QuanLySanBong.sln           # File giải pháp (Solution) để mở bằng Visual Studio
└── SQL_Script/                 # Thư mục chứa file sao lưu/kịch bản cơ sở dữ liệu (.sql)
