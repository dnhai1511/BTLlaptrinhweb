using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using OnlineQuizASPNet.Models;

namespace OnlineQuizASPNet.Controllers
{
    public class AdminController : Controller
    {
        private UserModel CurrentUser => Session["User"] as UserModel;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (CurrentUser == null || (CurrentUser.VaiTro != "Admin" && CurrentUser.VaiTro != "GiaoVien"))
            {
                filterContext.Result = new RedirectResult("/Account/Login");
            }
        }

        public ActionResult Index()
        {
            string qStats = @"
                SELECT 
                    (SELECT COUNT(*) FROM MonHoc) AS TotalSubjects,
                    (SELECT COUNT(*) FROM CauHoi) AS TotalQuestions,
                    (SELECT COUNT(*) FROM DeThi) AS TotalExams,
                    (SELECT COUNT(*) FROM KetQuaThi) AS TotalSubmissions";
            DataTable dt = DbHelper.ExecuteQuery(qStats);
            return View(dt.Rows[0]);
        }

        public ActionResult Questions()
        {
            string q = @"SELECT c.MaCauHoi, c.NoiDung, c.LoaiCauHoi, m.TenMon 
                         FROM CauHoi c INNER JOIN MonHoc m ON c.MaMon = m.MaMon 
                         ORDER BY c.MaCauHoi DESC";
            DataTable dt = DbHelper.ExecuteQuery(q);
            return View(dt);
        }

        [HttpGet]
        public ActionResult CreateQuestion()
        {
            ViewBag.MonHoc = DbHelper.ExecuteQuery("SELECT MaMon, TenMon FROM MonHoc");
            return View();
        }

        [HttpPost]
        public ActionResult CreateQuestion(string maMon, string noiDung, string loaiCauHoi, 
                                          string[] dapAn, int? dapAnDungIndex, string dapAnDungTuLuan)
        {
            string qIns = "INSERT INTO CauHoi (MaMon, NoiDung, LoaiCauHoi) VALUES (@m, @nd, @loai); SELECT SCOPE_IDENTITY();";
            int qId = Convert.ToInt32(DbHelper.ExecuteScalar(qIns, new SqlParameter[] {
                new SqlParameter("@m", maMon),
                new SqlParameter("@nd", noiDung),
                new SqlParameter("@loai", loaiCauHoi)
            }));

            if (loaiCauHoi == "TracNghiem" && dapAn != null)
            {
                for (int i = 0; i < dapAn.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(dapAn[i])) continue;
                    bool isCorrect = (dapAnDungIndex.HasValue && dapAnDungIndex.Value == i);
                    string qAns = "INSERT INTO DapAn (MaCauHoi, NoiDungDapAn, LaDapAnDung) VALUES (@qid, @nd, @dung)";
                    DbHelper.ExecuteNonQuery(qAns, new SqlParameter[] {
                        new SqlParameter("@qid", qId),
                        new SqlParameter("@nd", dapAn[i]),
                        new SqlParameter("@dung", isCorrect)
                    });
                }
            }
            else if (loaiCauHoi == "TuLuan" && !string.IsNullOrWhiteSpace(dapAnDungTuLuan))
            {
                string qAns = "INSERT INTO DapAn (MaCauHoi, NoiDungDapAn, LaDapAnDung) VALUES (@qid, @nd, 1)";
                DbHelper.ExecuteNonQuery(qAns, new SqlParameter[] {
                    new SqlParameter("@qid", qId),
                    new SqlParameter("@nd", dapAnDungTuLuan)
                });
            }

            return RedirectToAction("Questions");
        }

        public ActionResult Results()
        {
            string q = @"SELECT k.MaKetQua, u.HoTen, u.TaiKhoan, d.TenDeThi, k.SoDiem, k.ThoiGianNopBai, k.SoCauDung, k.TongSoCau
                         FROM KetQuaThi k
                         INNER JOIN NguoiDung u ON k.UserID = u.UserID
                         INNER JOIN DeThi d ON k.MaDeThi = d.MaDeThi
                         ORDER BY k.ThoiGianNopBai DESC";
            DataTable dt = DbHelper.ExecuteQuery(q);
            return View(dt);
        }
    }
}
