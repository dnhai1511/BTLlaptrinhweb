using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using OnlineQuizASPNet.Models;

namespace OnlineQuizASPNet.Controllers
{
    public class ExamController : Controller
    {
        private UserModel CurrentUser => Session["User"] as UserModel;

        public ActionResult Index()
        {
            if (CurrentUser == null) return RedirectToAction("Login", "Account");

            string query = @"SELECT d.MaDeThi, d.TenDeThi, d.ThoiGianLamBai, d.SoLuongCauHoi, m.TenMon 
                             FROM DeThi d INNER JOIN MonHoc m ON d.MaMon = m.MaMon";
            DataTable dt = DbHelper.ExecuteQuery(query);
            return View(dt);
        }

        [HttpGet]
        public ActionResult Take(int id)
        {
            if (CurrentUser == null) return RedirectToAction("Login", "Account");

            // Lấy thông tin đề thi
            string qExam = "SELECT * FROM DeThi WHERE MaDeThi = @id";
            DataTable dtExam = DbHelper.ExecuteQuery(qExam, new SqlParameter[] { new SqlParameter("@id", id) });
            if (dtExam.Rows.Count == 0) return RedirectToAction("Index");

            var deThi = new ExamTakingViewModel
            {
                MaDeThi = id,
                TenDeThi = dtExam.Rows[0]["TenDeThi"].ToString(),
                ThoiGianLamBai = (int)dtExam.Rows[0]["ThoiGianLamBai"]
            };
            int soLuong = (int)dtExam.Rows[0]["SoLuongCauHoi"];
            string maMon = dtExam.Rows[0]["MaMon"].ToString();

            // Sinh đề thi ngẫu nhiên theo môn học và số lượng câu hỏi quy định
            string qQuestions = $@"SELECT TOP ({soLuong}) MaCauHoi, NoiDung, LoaiCauHoi 
                                   FROM CauHoi 
                                   WHERE MaMon = @m 
                                   ORDER BY NEWID()"; // Ngẫu nhiên câu hỏi
            DataTable dtQ = DbHelper.ExecuteQuery(qQuestions, new SqlParameter[] { new SqlParameter("@m", maMon) });

            foreach (DataRow row in dtQ.Rows)
            {
                int qId = (int)row["MaCauHoi"];
                var qVM = new QuestionViewModel
                {
                    MaCauHoi = qId,
                    NoiDung = row["NoiDung"].ToString(),
                    LoaiCauHoi = row["LoaiCauHoi"].ToString()
                };

                // Lấy đáp án và cũng xáo trộn đáp án ngẫu nhiên
                string qAns = "SELECT MaDapAn, MaCauHoi, NoiDungDapAn FROM DapAn WHERE MaCauHoi = @qid ORDER BY NEWID()";
                DataTable dtA = DbHelper.ExecuteQuery(qAns, new SqlParameter[] { new SqlParameter("@qid", qId) });
                foreach (DataRow aRow in dtA.Rows)
                {
                    qVM.DanhSachDapAn.Add(new AnswerViewModel
                    {
                        MaDapAn = (int)aRow["MaDapAn"],
                        MaCauHoi = qId,
                        NoiDungDapAn = aRow["NoiDungDapAn"].ToString()
                    });
                }
                deThi.DanhSachCauHoi.Add(qVM);
            }

            // Lưu danh sách câu hỏi vào Session để đối chiếu khi chấm
            Session[$"ExamQuestions_{id}"] = deThi.DanhSachCauHoi;
            return View(deThi);
        }

        [HttpPost]
        public ActionResult Submit(ExamSubmissionViewModel model)
        {
            if (CurrentUser == null) return Json(new { success = false, message = "Hết phiên đăng nhập" });

            int totalQuestions = model.TraLoi.Count;
            int correctCount = 0;

            // Chấm điểm
            foreach (var item in model.TraLoi)
            {
                string qCheck = "SELECT LoaiCauHoi FROM CauHoi WHERE MaCauHoi = @qid";
                object loaiObj = DbHelper.ExecuteScalar(qCheck, new SqlParameter[] { new SqlParameter("@qid", item.MaCauHoi) });
                string loai = loaiObj?.ToString() ?? "TracNghiem";

                if (loai == "TracNghiem")
                {
                    if (item.MaDapAnChon.HasValue)
                    {
                        string qCorrect = "SELECT LaDapAnDung FROM DapAn WHERE MaDapAn = @daId";
                        object res = DbHelper.ExecuteScalar(qCorrect, new SqlParameter[] { new SqlParameter("@daId", item.MaDapAnChon.Value) });
                        if (res != null && Convert.ToBoolean(res)) correctCount++;
                    }
                }
                else // TuLuan: So sánh với đáp án chuẩn
                {
                    string qCorrectText = "SELECT NoiDungDapAn FROM DapAn WHERE MaCauHoi = @qid AND LaDapAnDung = 1";
                    object resText = DbHelper.ExecuteScalar(qCorrectText, new SqlParameter[] { new SqlParameter("@qid", item.MaCauHoi) });
                    if (resText != null && item.DapAnTuLuan != null &&
                        resText.ToString().Trim().Equals(item.DapAnTuLuan.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        correctCount++;
                    }
                }
            }

            decimal diem = totalQuestions > 0 ? Math.Round(((decimal)correctCount / totalQuestions) * 10, 2) : 0;

            // Lưu kết quả thi vào CSDL
            string insKetQua = @"INSERT INTO KetQuaThi (UserID, MaDeThi, ThoiGianBatDau, ThoiGianNopBai, SoDiem, TongSoCau, SoCauDung)
                                 VALUES (@uid, @dethi, GETDATE(), GETDATE(), @diem, @tong, @dung);
                                 SELECT SCOPE_IDENTITY();";
            SqlParameter[] pKetQua = {
                new SqlParameter("@uid", CurrentUser.UserID),
                new SqlParameter("@dethi", model.MaDeThi),
                new SqlParameter("@diem", diem),
                new SqlParameter("@tong", totalQuestions),
                new SqlParameter("@dung", correctCount)
            };
            int maKetQua = Convert.ToInt32(DbHelper.ExecuteScalar(insKetQua, pKetQua));

            return Json(new { success = true, maKetQua = maKetQua, score = diem, correct = correctCount, total = totalQuestions });
        }

        public ActionResult Result(int id)
        {
            if (CurrentUser == null) return RedirectToAction("Login", "Account");

            string query = @"SELECT k.*, d.TenDeThi, u.HoTen 
                             FROM KetQuaThi k 
                             INNER JOIN DeThi d ON k.MaDeThi = d.MaDeThi 
                             INNER JOIN NguoiDung u ON k.UserID = u.UserID 
                             WHERE k.MaKetQua = @id";
            DataTable dt = DbHelper.ExecuteQuery(query, new SqlParameter[] { new SqlParameter("@id", id) });
            if (dt.Rows.Count == 0) return RedirectToAction("Index");

            return View(dt.Rows[0]);
        }
    }
}
