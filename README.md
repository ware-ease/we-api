# WareEase API (we-api)

WareEase là hệ thống **quản lý kho hàng (Warehouse Management System)** được xây dựng bằng **ASP.NET Core 8 Web API**, áp dụng kiến trúc phân lớp (Layered Architecture) rõ ràng: API → Business Logic Layer → Data Access Layer → Data. Hệ thống hỗ trợ quản lý sản phẩm, lô hàng, tồn kho, phiếu nhập/xuất, kiểm kê, điều chỉnh tồn kho, đối tác, khách hàng, nhà cung cấp, phân quyền theo nhóm, xác thực JWT, thông báo realtime qua Firebase, lưu trữ ảnh qua Cloudinary và gửi email qua SMTP.

---

## 1. Mục lục

- [Kiến trúc tổng quan](#2-kiến-trúc-tổng-quan)
- [Công nghệ sử dụng](#3-công-nghệ-sử-dụng)
- [Cấu trúc thư mục](#4-cấu-trúc-thư-mục)
- [Các tính năng / module chính](#5-các-tính-năng--module-chính)
- [Danh sách Controller / Endpoint](#6-danh-sách-controller--endpoint)
- [Xác thực & Phân quyền](#7-xác-thực--phân-quyền)
- [Biến môi trường (.env)](#8-biến-môi-trường-env)
- [Cài đặt & chạy dự án](#9-cài-đặt--chạy-dự-án)
- [Chạy bằng Docker](#10-chạy-bằng-docker)
- [Database & Migrations](#11-database--migrations)
- [CI/CD](#12-cicd)
- [Swagger / API Docs](#13-swagger--api-docs)
- [Ghi chú khác](#14-ghi-chú-khác)

---

## 2. Kiến trúc tổng quan

Dự án được tổ chức theo mô hình **Clean/Layered Architecture** với 4 project (.csproj) trong solution `we-api.sln`:

```
we-api.sln
├── API                   → Tầng trình diễn (Controllers, Middlewares, Program.cs, cấu hình)
├── BusinessLogicLayer    → Tầng nghiệp vụ (Services, IServices, Mappings AutoMapper, DTO Models)
├── DataAccessLayer       → Tầng truy cập dữ liệu (Repositories, UnitOfWork, Generic Repository)
└── Data                  → Tầng định nghĩa dữ liệu (Entity, Enum, Model/DTO, DbContext, Migrations)
```

Luồng xử lý request: `Controller → Service (BLL) → Repository / UnitOfWork (DAL) → DbContext (EF Core) → SQL Server`

Dữ liệu trả về client được map qua **AutoMapper** (`MappingProfile`) và đóng gói thống nhất qua `ControllerResponse` / `ServiceResult` (kèm `SRStatus` enum: Success, Unauthorized, NotFound, BadRequest...).

---

## 3. Công nghệ sử dụng

| Thành phần | Công nghệ |
|---|---|
| Framework | ASP.NET Core 8.0 (Web API) |
| ORM | Entity Framework Core 8.0 (Code-First, SQL Server) |
| Xác thực | JWT Bearer Token (Access Token + Refresh Token, lưu trong Cookie) |
| Mapping | AutoMapper |
| Upload ảnh | Cloudinary (CloudinaryDotNet SDK) |
| Realtime / Notification | Firebase Realtime Database / Firebase Cloud Messaging |
| Gửi email | SMTP (System.Net.Mail) |
| API Docs | Swashbuckle (Swagger UI) + hỗ trợ Bearer Auth trên Swagger |
| Cấu hình môi trường | DotNetEnv (đọc file `.env`) |
| Background Job | `IHostedService` (`WorkerService`, `BatchCheckService`) |
| Containerization | Docker (multi-stage build) |
| CI/CD | GitHub Actions (build & push Docker image lên Docker Hub, tự động redeploy qua webhook) |

---

## 4. Cấu trúc thư mục

```
we-api/
├── API/
│   ├── Controllers/         # 23 controller (REST endpoints)
│   ├── Middlewares/         # GroupAuthorizationMiddleware, TokenValidationMiddleware, AuthorizeGroupAttribute
│   ├── Utils/                # AuthHelper, ControllerResponse...
│   ├── Common/
│   ├── Properties/
│   ├── Program.cs            # Entry point, đăng ký DI, JWT, CORS, Swagger
│   ├── appsettings.json
│   └── appsettings.Development.json
│
├── BusinessLogicLayer/
│   ├── Service / Services/   # Logic nghiệp vụ (AuthService, ProductService, InventoryService...)
│   ├── IService / IServices/ # Interface tương ứng
│   ├── Models/                # DTO nội bộ tầng nghiệp vụ
│   ├── Mappings/              # AutoMapper Profile
│   ├── Generic/                # GenericPaginationService...
│   └── Utils/
│
├── DataAccessLayer/
│   ├── Repositories/          # Triển khai Repository cụ thể theo Entity
│   ├── IRepositories/         # Interface Repository
│   ├── UnitOfWork/             # IUnitOfWork / UnitOfWork
│   ├── Generic/                 # GenericRepository<T>
│   └── Utils/
│
├── Data/
│   ├── Entity/                 # Entity Framework Core entities (29 entity)
│   ├── Enum/                    # SRStatus và các enum nghiệp vụ khác
│   └── Model/                   # DTO Request/Response dùng chung
│
├── DataAccessLayer/Migrations/  # 105 file migration EF Core
├── Dockerfile
├── .dockerignore
├── .github/workflows/           # docker-image.yml, deploy.yml
└── we-api.sln
```

---

## 5. Các tính năng / module chính

Dựa trên các Entity và Service hiện có, hệ thống bao gồm các nhóm nghiệp vụ:

**Quản trị người dùng & phân quyền**
- Tài khoản (`Account`), Hồ sơ (`Profile`)
- Nhóm quyền (`Group`, `GroupPermission`), Quyền (`Permission`)
- Gán tài khoản vào nhóm (`AccountGroup`), gán quyền trực tiếp (`AccountPermission`)
- Gán tài khoản vào kho (`AccountWarehouse`)
- Xác thực JWT (Access Token + Refresh Token), middleware kiểm tra quyền theo nhóm

**Quản lý danh mục hàng hóa**
- Sản phẩm (`Product`), Loại sản phẩm (`ProductType`)
- Danh mục (`Category`), Thương hiệu (`Brand`), Đơn vị tính (`Unit`)
- Sinh mã tự động (`CodeGeneratorService`) cho các đối tượng (mã sản phẩm, mã phiếu...)

**Quản lý kho & tồn kho**
- Kho hàng (`Warehouse`)
- Tồn kho (`Inventory`), Lô hàng (`Batch`) — có theo dõi hạn sử dụng (`BatchCheckService` chạy nền kiểm tra lô sắp hết hạn)
- Kiểm kê kho (`InventoryCount`, `InventoryCountDetail`)
- Điều chỉnh tồn kho (`InventoryAdjustment`, `InventoryAdjustmentDetail`)
- Sổ kho / sổ cái (`StockBook`)

**Phiếu nhập/xuất**
- Phiếu yêu cầu hàng (`GoodRequest`, `GoodRequestDetail`)
- Phiếu nhập/xuất kho (`GoodNote`, `GoodNoteDetail`)
- Tuyến vận chuyển (`Route`), Lịch trình (`Schedule`)

**Đối tác**
- Đối tác chung (`Partner`)
- Nhà cung cấp (`Supplier`) và Khách hàng (`Customer`) (kế thừa/đặc tả từ Partner)

**Khác**
- Dashboard (số liệu thống kê tổng quan)
- Thông báo (`Notification`, `AccountNotification`) tích hợp Firebase
- Upload ảnh trực tiếp lên Cloudinary qua Presigned URL (`PresignedUrlController`)
- Background service gửi cảnh báo / kiểm tra theo lịch (`WorkerService`, `BatchCheckService`)

---

## 6. Danh sách Controller / Endpoint

Tất cả route có tiền tố `api/<resource>`. Chi tiết đầy đủ tham số/response xem tại Swagger UI khi chạy ứng dụng.

| Controller | Route gốc | Chức năng chính |
|---|---|---|
| `AuthController` | `api/auth` | Login, Logout, Refresh token, quên/đổi mật khẩu |
| `AccountController` | `api/account` | CRUD tài khoản người dùng |
| `BatchController` | `api/batch` | Quản lý lô hàng, hạn sử dụng |
| `BrandController` | `api/brand` | CRUD thương hiệu |
| `CategoryController` | `api/category` | CRUD danh mục sản phẩm |
| `CodeGeneratorController` | `api/code-generator` | Sinh mã tự động cho các thực thể |
| `CustomerController` | `api/customer` | CRUD khách hàng |
| `DashboardController` | `api/dashboard` | Thống kê, báo cáo tổng quan |
| `GoodNoteController` | `api/good-note` | Phiếu nhập/xuất kho |
| `GoodRequestController` | `api/good-request` | Phiếu yêu cầu hàng |
| `GroupController` | `api/group` | Quản lý nhóm quyền |
| `InventoryAdjustmentController` | `api/inventory-adjustment` | Điều chỉnh tồn kho |
| `InventoryController` | `api/inventory` | Truy vấn tồn kho |
| `InventoryCountController` | `api/inventory-count` | Kiểm kê kho |
| `PartnerController` | `api/partner` | Quản lý đối tác chung |
| `PermissionController` | `api/permission` | Danh sách & cấu hình quyền |
| `PresignedUrlController` | `api/presigned-url` | Lấy URL ký trước để upload ảnh lên Cloudinary |
| `ProductController` | `api/product` | CRUD sản phẩm |
| `ProductTypeController` | `api/product-type` | CRUD loại sản phẩm |
| `RouteController` | `api/route` | Quản lý tuyến vận chuyển |
| `ScheduleController` | `api/schedule` | Quản lý lịch trình |
| `SupplierController` | `api/supplier` | CRUD nhà cung cấp |
| `UnitController` | `api/unit` | CRUD đơn vị tính |
| `WarehouseController` | `api/warehouse` | CRUD kho hàng |

> Ví dụ endpoint `AuthController`:
> - `POST /api/auth/login` — đăng nhập, trả về access/refresh token qua cookie `httpOnly`
> - `POST /api/auth/logout` *(yêu cầu Authorize)* — đăng xuất, xoá cookie token
> - `POST /api/auth/refresh` — làm mới access token bằng refresh token

---

## 7. Xác thực & Phân quyền

- **JWT Bearer Authentication**: token được sinh bởi `JwtService`, có thể truyền qua header `Authorization: Bearer <token>` hoặc tự động lấy từ cookie `accessToken` (xử lý trong `OnMessageReceived` của `JwtBearerEvents`).
- **Refresh Token**: lưu trong bảng `RefreshToken`, dùng để cấp lại access token khi hết hạn mà không cần đăng nhập lại.
- **Group-based Authorization**: middleware `GroupAuthorizationMiddleware` kết hợp `AuthorizeGroupAttribute` kiểm tra quyền của tài khoản dựa trên nhóm (`Group` → `GroupPermission`) hoặc quyền gán trực tiếp (`AccountPermission`).
- **CORS**: chỉ cho phép origin `https://wareease.site` và `http://localhost:3000`, hỗ trợ `AllowCredentials` (bắt buộc để gửi/nhận cookie).

---

## 8. Biến môi trường (.env)

Ứng dụng đọc cấu hình qua file `.env` ở thư mục gốc (sử dụng `DotNetEnv`). Tạo file `.env` với nội dung mẫu sau:

```env
# Database
DB_URL=Server=localhost,1433;Database=WareEaseDB;User Id=sa;Password=YourPassword123;TrustServerCertificate=True;

# JWT
JWT_SECRET_KEY=your_super_secret_key_min_32_chars
JWT_VALID_ISSUER=https://your-domain.com
JWT_VALID_AUDIENCE=https://your-domain.com

# Cloudinary (upload ảnh)
CLOUDINARY_CLOUD_NAME=your_cloud_name
CLOUDINARY_API_KEY=your_api_key
CLOUDINARY_API_SECRET=your_api_secret
CLOUDINARY_UPLOAD_PRESET=your_upload_preset

# Firebase (thông báo realtime)
FIREBASE_DATABASE_URL=https://your-project.firebaseio.com/
FIREBASE_APP_SECRET=your_firebase_secret

# SMTP (gửi email)
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USER=your_email@gmail.com
SMTP_PASS=your_app_password
SMTP_FROM=your_email@gmail.com
```

> ⚠️ Không commit file `.env` thật lên Git. Có thể dùng `appsettings.Development.json` cho cấu hình bổ sung khi phát triển local.

---

## 9. Cài đặt & chạy dự án

### Yêu cầu
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (local hoặc container)
- (Tuỳ chọn) Tài khoản Cloudinary, Firebase, SMTP nếu muốn test đầy đủ tính năng

### Các bước

```bash
# 1. Clone source code
git clone <repository-url>
cd we-api

# 2. Tạo file .env ở thư mục gốc (xem mục 8)

# 3. Restore packages
dotnet restore we-api.sln

# 4. Cập nhật database (EF Core Migrations)
dotnet ef database update --project DataAccessLayer --startup-project API

# 5. Chạy ứng dụng
dotnet run --project API
```

Sau khi chạy thành công, API mặc định lắng nghe tại `https://localhost:<port>` (xem `API/Properties/launchSettings.json`), Swagger UI có sẵn tại `/swagger`.

---

## 10. Chạy bằng Docker

Dự án đã có sẵn `Dockerfile` (multi-stage build), expose port **8068**.

```bash
# Build image
docker build -t we-api .

# Chạy container (truyền biến môi trường qua --env-file)
docker run -d -p 8068:8068 --env-file .env --name we-api we-api
```

CI/CD pipeline (GitHub Actions) tự động build và push image lên Docker Hub với tag `korroo/we-api:latest` mỗi khi có push/merge vào nhánh `main`.

---

## 11. Database & Migrations

- ORM: **Entity Framework Core 8** (Code-First), DbContext: `WaseEaseDbContext` (trong `DataAccessLayer/WaseEaseDbContext.cs`).
- Database hiện có **105 migration**, phản ánh lịch sử phát triển schema khá chi tiết (thêm bảng, cột, ràng buộc khoá ngoại...).
- Một số entity tiêu biểu: `Account`, `Product`, `Inventory`, `Batch`, `Warehouse`, `GoodNote`, `GoodRequest`, `InventoryCount`, `InventoryAdjustment`, `Partner`, `Group`, `Permission`, `Route`, `Schedule`, `Notification`, `RefreshToken`, `StockBook`...

Lệnh hữu ích:

```bash
# Tạo migration mới sau khi sửa entity
dotnet ef migrations add <TenMigration> --project DataAccessLayer --startup-project API

# Áp dụng migration vào database
dotnet ef database update --project DataAccessLayer --startup-project API

# Xem danh sách migration
dotnet ef migrations list --project DataAccessLayer --startup-project API
```

---

## 12. CI/CD

Cấu hình tại `.github/workflows/`:

1. **`docker-image.yml`** — Khi push/PR vào nhánh `main`: checkout code → đăng nhập Docker Hub → build & push image `korroo/we-api:latest`.
2. **`deploy.yml`** — Khi workflow build Docker image chạy xong thành công (hoặc trigger thủ công): gửi POST request tới `WEBHOOK_URL` (secret) để server tự động pull image mới và recreate container (zero-touch deployment).

Cần cấu hình các GitHub Secrets: `DOCKERHUB_USERNAME`, `DOCKERHUB_TOKEN`, `WEBHOOK_URL`.

---

## 13. Swagger / API Docs

Sau khi chạy ứng dụng (local hoặc Docker), truy cập:

```
http://localhost:<port>/swagger
```

Swagger UI hỗ trợ:
- Xem toàn bộ danh sách endpoint, request/response schema
- Nút **Authorize** để nhập Bearer Token và test các API yêu cầu xác thực
- Hiển thị đúng định dạng cho `DateOnly` (`date`) và `TimeOnly` (`time`, ví dụ `00:00`)

---

## 14. Ghi chú khác

- **Global Exception Handling**: cấu hình qua `AddProblemDetails`, trả về response chuẩn RFC 7807 (`traceId`, `exceptionMessage`) khi có lỗi không bắt được.
- **JSON Serialization**: bật `ReferenceHandler.IgnoreCycles` để tránh lỗi vòng lặp tham chiếu (circular reference) giữa các entity liên kết (ví dụ `GoodNote` ↔ `GoodNoteDetail`).
- **Background Services**:
  - `WorkerService`: xử lý tác vụ nền định kỳ.
  - `BatchCheckService`: kiểm tra định kỳ các lô hàng (`Batch`) sắp/đã hết hạn để cảnh báo.
- **Generic Repository & UnitOfWork pattern**: hầu hết entity dùng `IGenericRepository<T>` cho thao tác CRUD cơ bản, kết hợp `IUnitOfWork` để đảm bảo tính nhất quán giao dịch (transaction) khi thao tác trên nhiều bảng.
- Múi giờ container được set cố định là `Asia/Ho_Chi_Minh` trong Dockerfile.
