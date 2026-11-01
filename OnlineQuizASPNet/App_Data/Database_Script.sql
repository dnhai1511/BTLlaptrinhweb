-- =============================================
-- KỊCH BẢN TẠO CSDL CHO WEBSITE THI TRẮC NGHIỆM TRỰC TUYẾN
-- =============================================
CREATE DATABASE QLThiTracNghiem;
GO

USE QLThiTracNghiem;
GO

-- 1. Bảng NguoiDung
CREATE TABLE NguoiDung (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    TaiKhoan VARCHAR(50) NOT NULL UNIQUE,
    MatKhau VARCHAR(255) NOT NULL,
    HoTen NVARCHAR(100) NOT NULL,
    VaiTro NVARCHAR(20) NOT NULL DEFAULT 'ThiSinh' -- 'Admin', 'GiaoVien', 'ThiSinh'
);
GO

-- 2. Bảng MonHoc
CREATE TABLE MonHoc (
    MaMon VARCHAR(20) PRIMARY KEY,
    TenMon NVARCHAR(150) NOT NULL
);
GO

-- 3. Bảng CauHoi
CREATE TABLE CauHoi (
    MaCauHoi INT IDENTITY(1,1) PRIMARY KEY,
    MaMon VARCHAR(20) FOREIGN KEY REFERENCES MonHoc(MaMon) ON DELETE CASCADE,
    NoiDung NVARCHAR(MAX) NOT NULL,
    LoaiCauHoi NVARCHAR(50) NOT NULL DEFAULT 'TracNghiem' -- 'TracNghiem', 'TuLuan'
);
GO

-- 4. Bảng DapAn
CREATE TABLE DapAn (
    MaDapAn INT IDENTITY(1,1) PRIMARY KEY,
    MaCauHoi INT FOREIGN KEY REFERENCES CauHoi(MaCauHoi) ON DELETE CASCADE,
    NoiDungDapAn NVARCHAR(MAX) NOT NULL,
    LaDapAnDung BIT NOT NULL DEFAULT 0
);
GO

-- 5. Bảng DeThi
CREATE TABLE DeThi (
    MaDeThi INT IDENTITY(1,1) PRIMARY KEY,
    MaMon VARCHAR(20) FOREIGN KEY REFERENCES MonHoc(MaMon),
    TenDeThi NVARCHAR(150) NOT NULL,
    ThoiGianLamBai INT NOT NULL, -- số phút
    SoLuongCauHoi INT NOT NULL,
    TongDiem DECIMAL(4,2) DEFAULT 10.0
);
GO

-- 6. Bảng KetQuaThi
CREATE TABLE KetQuaThi (
    MaKetQua INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT FOREIGN KEY REFERENCES NguoiDung(UserID),
    MaDeThi INT FOREIGN KEY REFERENCES DeThi(MaDeThi),
    ThoiGianBatDau DATETIME DEFAULT GETDATE(),
    ThoiGianNopBai DATETIME,
    SoDiem DECIMAL(4,2) DEFAULT 0.0,
    TongSoCau INT DEFAULT 0,
    SoCauDung INT DEFAULT 0
);
GO

-- 7. Bảng BaiLamChiTiet
CREATE TABLE BaiLamChiTiet (
    MaChiTiet INT IDENTITY(1,1) PRIMARY KEY,
    MaKetQua INT FOREIGN KEY REFERENCES KetQuaThi(MaKetQua) ON DELETE CASCADE,
    MaCauHoi INT FOREIGN KEY REFERENCES CauHoi(MaCauHoi),
    MaDapAnChon INT NULL FOREIGN KEY REFERENCES DapAn(MaDapAn),
    CauTraLoiTuLuan NVARCHAR(MAX) NULL,
    DungSai BIT DEFAULT 0
);
GO

-- DỮ LIỆU MẪU BAN ĐẦU
INSERT INTO NguoiDung (TaiKhoan, MatKhau, HoTen, VaiTro) VALUES
('admin', 'admin123', N'Quản trị viên', 'Admin'),
('giaovien', '123456', N'Thầy Nguyễn Văn A', 'GiaoVien'),
('thisinh1', '123456', N'Trần Thị Mai', 'ThiSinh'),
('thisinh2', '123456', N'Lê Hoàng Long', 'ThiSinh');

INSERT INTO MonHoc (MaMon, TenMon) VALUES
('CSDL', N'Hệ Quản Trị Cơ Sở Dữ Liệu'),
('LTWEB', N'Lập trình Web ASP.NET');

-- Thêm câu hỏi mẫu CSDL
INSERT INTO CauHoi (MaMon, NoiDung, LoaiCauHoi) VALUES
('CSDL', N'Trong SQL Server, câu lệnh nào được dùng để lấy dữ liệu từ bảng?', 'TracNghiem');
DECLARE @id1 INT = SCOPE_IDENTITY();
INSERT INTO DapAn (MaCauHoi, NoiDungDapAn, LaDapAnDung) VALUES
(@id1, N'SELECT', 1),
(@id1, N'UPDATE', 0),
(@id1, N'INSERT', 0),
(@id1, N'DELETE', 0);

INSERT INTO CauHoi (MaMon, NoiDung, LoaiCauHoi) VALUES
('CSDL', N'Khóa chính (Primary Key) trong một bảng có đặc điểm gì?', 'TracNghiem');
DECLARE @id2 INT = SCOPE_IDENTITY();
INSERT INTO DapAn (MaCauHoi, NoiDungDapAn, LaDapAnDung) VALUES
(@id2, N'Duy nhất và không được chứa giá trị NULL', 1),
(@id2, N'Có thể trùng lặp', 0),
(@id2, N'Chấp nhận giá trị NULL tự do', 0),
(@id2, N'Một bảng có thể có nhiều khóa chính độc lập', 0);

INSERT INTO CauHoi (MaMon, NoiDung, LoaiCauHoi) VALUES
('CSDL', N'Điền từ khóa: Lệnh nào dùng để nhóm các hàng có cùng giá trị trong SQL?', 'TuLuan');
DECLARE @id3 INT = SCOPE_IDENTITY();
INSERT INTO DapAn (MaCauHoi, NoiDungDapAn, LaDapAnDung) VALUES
(@id3, N'GROUP BY', 1);

INSERT INTO CauHoi (MaMon, NoiDung, LoaiCauHoi) VALUES
('LTWEB', N'ASP.NET là nền tảng phát triển ứng dụng web của tập đoàn nào?', 'TracNghiem');
DECLARE @id4 INT = SCOPE_IDENTITY();
INSERT INTO DapAn (MaCauHoi, NoiDungDapAn, LaDapAnDung) VALUES
(@id4, N'Microsoft', 1),
(@id4, N'Google', 0),
(@id4, N'Oracle', 0),
(@id4, N'Apple', 0);

INSERT INTO DeThi (MaMon, TenDeThi, ThoiGianLamBai, SoLuongCauHoi, TongDiem) VALUES
('CSDL', N'Kiểm tra giữa kỳ Cơ sở dữ liệu', 15, 3, 10.0),
('LTWEB', N'Kiểm tra trắc nghiệm Lập trình Web', 15, 1, 10.0);
GO
