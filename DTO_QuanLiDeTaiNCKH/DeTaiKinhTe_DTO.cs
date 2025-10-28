using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO_QuanLiDeTaiNCKH
{
    public class DeTaiKinhTe_DTO : DeTai_DTO, IHoTroNghienCuu
    {
        private int soCauHoiKhaoSat;

        public DeTaiKinhTe_DTO() : base()
        { }

        public DeTaiKinhTe_DTO(string maDT, string tenDT, string chuNhiem, string gvhd, System.DateTime start, System.DateTime end, int soCauHoi) : base(maDT, tenDT, chuNhiem, gvhd, start, end)
        {
            SoCauHoiKhaoSat = soCauHoi;
        }
        public int SoCauHoiKhaoSat
        {
            get { return soCauHoiKhaoSat; }
            set { soCauHoiKhaoSat = (value > 0) ? value : 0; } // Đảm bảo giá trị không âm
        }
        public override void Nhap()
        {
            Console.WriteLine("-- Nhập Thông Tin Đề Tài Kinh Tế --");
            base.Nhap();
            Console.Write("  Nhập Số câu hỏi khảo sát: ");
            int.TryParse(Console.ReadLine(), out soCauHoiKhaoSat);
            SoCauHoiKhaoSat = soCauHoiKhaoSat;     }
        public override void Xuat()
        {
            base.Xuat();
            // Console.WriteLine($"   -> Số câu hỏi: {SoCauHoiKhaoSat}, Phí hỗ trợ: {TinhPhiHoTro():N0} đ");
        }
        public override double TinhKinhPhiCoBan()
        {
            return (SoCauHoiKhaoSat > 100) ? 12000000 : 7000000;
        }
        public double TinhPhiHoTro()
        {
            return (SoCauHoiKhaoSat > 100) ? SoCauHoiKhaoSat * 550 : SoCauHoiKhaoSat * 450;
        }
    }
}
