using System;
using System.Collections.Generic;

namespace OnlineQuizASPNet.Models
{
    public class UserModel
    {
        public int UserID { get; set; }
        public string TaiKhoan { get; set; }
        public string HoTen { get; set; }
        public string VaiTro { get; set; }
    }

    public class QuestionViewModel
    {
        public int MaCauHoi { get; set; }
        public string NoiDung { get; set; }
        public string LoaiCauHoi { get; set; }
        public List<AnswerViewModel> DanhSachDapAn { get; set; } = new List<AnswerViewModel>();
    }

    public class AnswerViewModel
    {
        public int MaDapAn { get; set; }
        public int MaCauHoi { get; set; }
        public string NoiDungDapAn { get; set; }
        public bool LaDapAnDung { get; set; }
    }

    public class ExamTakingViewModel
    {
        public int MaDeThi { get; set; }
        public string TenDeThi { get; set; }
        public int ThoiGianLamBai { get; set; } // minutes
        public List<QuestionViewModel> DanhSachCauHoi { get; set; } = new List<QuestionViewModel>();
    }

    public class SubmissionItem
    {
        public int MaCauHoi { get; set; }
        public int? MaDapAnChon { get; set; }
        public string DapAnTuLuan { get; set; }
    }

    public class ExamSubmissionViewModel
    {
        public int MaDeThi { get; set; }
        public List<SubmissionItem> TraLoi { get; set; } = new List<SubmissionItem>();
    }
}
