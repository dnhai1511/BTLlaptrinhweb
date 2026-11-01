using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using OnlineQuizASPNet.Models;

namespace OnlineQuizASPNet.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            if (Session["User"] != null) return RedirectToAction("Index", "Exam");
            return View();
        }

        [HttpPost]
        public ActionResult Login(string taiKhoan, string matKhau)
        {
            string query = "SELECT UserID, TaiKhoan, HoTen, VaiTro FROM NguoiDung WHERE TaiKhoan = @user AND MatKhau = @pass";
            SqlParameter[] parameters = {
                new SqlParameter("@user", taiKhoan ?? ""),
                new SqlParameter("@pass", matKhau ?? "")
            };

            DataTable dt = DbHelper.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
            {
                var user = new UserModel
                {
                    UserID = (int)dt.Rows[0]["UserID"],
                    TaiKhoan = dt.Rows[0]["TaiKhoan"].ToString(),
                    HoTen = dt.Rows[0]["HoTen"].ToString(),
                    VaiTro = dt.Rows[0]["VaiTro"].ToString()
                };
                Session["User"] = user;

                if (user.VaiTro == "Admin" || user.VaiTro == "GiaoVien")
                    return RedirectToAction("Index", "Admin");
                else
                    return RedirectToAction("Index", "Exam");
            }

            ViewBag.Error = "Tài khoản hoặc mật khẩu không chính xác!";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
