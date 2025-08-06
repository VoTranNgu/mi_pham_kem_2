using System;
using System.ComponentModel.DataAnnotations;

namespace mi_pham_kem.Models.SQLServer
{
    public class BaiViets
    {
        public int Id { get; set; }


        [StringLength(255)]
        [Required(ErrorMessage = "Tiêu đề không được bỏ trống")]
        public string TieuDe { get; set; } = string.Empty;


        public string? AnhDaiDien { get; set; }

        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Nội dung không được bỏ trống")]
        public string NoiDung { get; set; } = string.Empty;
        public DateTime NgayDang { get; set; } = DateTime.Now;

    }
}
