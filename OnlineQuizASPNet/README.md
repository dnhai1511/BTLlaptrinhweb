# HƯỚNG DẪN CÀI ĐẶT & CHẠY DỰ ÁN WEBSITE THI TRẮC NGHIỆM TRỰC TUYẾN (ASP.NET & SQL SERVER)

## 1. Yêu cầu môi trường
- **Visual Studio**: Phiên bản 2017, 2019 hoặc 2022.
- **MS SQL Server**: Phiên bản 2012 trở lên hoặc SQL Express.
- **.NET Framework**: 3.5 hoặc 4.5+ (mặc định đã cấu hình 4.7.2 tương thích cao).

## 2. Các bước cài đặt CSDL
1. Mở **SQL Server Management Studio (SSMS)**.
2. Mở file `App_Data/Database_Script.sql` và nhấn **Execute (F5)** để tạo CSDL `QLThiTracNghiem` và các dữ liệu mẫu.
3. Mở file `Web.config`, kiểm tra và chỉnh sửa chuỗi kết nối `QuizDbConn` nếu tên Server SQL của bạn khác `localhost`.

## 3. Chạy ứng dụng trên Visual Studio
1. Mở Visual Studio -> `Open Project/Solution` -> Chọn thư mục hoặc tạo mới project ASP.NET MVC rồi add các file này vào.
2. Nhấn `F5` hoặc `Ctrl + F5` để chạy trên IIS Express.

## 4. Tài khoản kiểm thử
- **Thí sinh**: `thisinh1` | Mật khẩu: `123456`
- **Quản trị viên / Giáo viên**: `admin` | Mật khẩu: `admin123`

## 5. Các tính năng cốt lõi đã hoàn thành theo đề tài
- **Sinh đề thi ngẫu nhiên**: Xáo trộn câu hỏi và các đáp án A, B, C, D bằng `NEWID()`.
- **Hỗ trợ 2 dạng câu hỏi**: Trắc nghiệm lựa chọn đáp án và Tự luận nhập văn bản so sánh với đáp án đúng.
- **Bộ đếm thời gian thi (Countdown Timer)**: Tự động khóa và nộp bài khi hết giờ.
- **Quản trị**: Quản lý ngân hàng câu hỏi, môn học và xem bảng điểm kết quả bài thi
- quản lý : bài thi 