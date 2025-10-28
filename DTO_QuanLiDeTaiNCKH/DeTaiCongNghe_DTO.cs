using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO_QuanLiDeTaiNCKH
{
    public class DeTaiCongNghe_DTO : DeTai_DTO , IHoTroNghienCuu
    {
        private string moiTruongTrienKhai;

        public DeTaiCongNghe_DTO() : base()
        { }
        public DeTaiCongNghe_DTO(string maDT, string tenDT, string chuNhiem, string gvhd, System.DateTime start, System.DateTime end, string moiTruong) : base(maDT, tenDT, chuNhiem, gvhd, start, end)

        {
            MoiTruongTrienKhai = moiTruong;
        }

        public string MoiTruongTrienKhai
        {
            get { return moiTruongTrienKhai; }
            set { moiTruongTrienKhai = value?.ToLower() ?? "window"; } // Đặt giá trị mặc định và chuyển chữ thường
        }

        public override void Nhap()
        {
            Console.WriteLine("-- Nhập Thông Tin Đề Tài Công Nghệ --");
            base.Nhap();
            Console.Write("  Nhập Môi trường triển khai (web/mobile/window): ");
            MoiTruongTrienKhai = Console.ReadLine();
        }

        public override void Xuat()
        {
            base.Xuat();
            // Console.WriteLine($"   -> Môi trường: {MoiTruongTrienKhai}, Phí hỗ trợ: {TinhPhiHoTro():N0} đ");
        }
        public override double TinhKinhPhiCoBan()
        {
            if (MoiTruongTrienKhai == "web" || MoiTruongTrienKhai == "mobile")
            {
                return 15000000;
            }
            else 
            {
                return 10000000;
            }
        }
        public double TinhPhiHoTro()
        {
            switch (MoiTruongTrienKhai)
            {
                case "mobile":
                    return 1000000;
                case "web":
                    return 800000;
                case "window":
                default: // Giá trị dự phòng để tránh lỗi mặc định 500000
                    return 500000;
            }
        }
    }
}
