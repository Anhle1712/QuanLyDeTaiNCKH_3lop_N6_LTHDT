using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO_QuanLiDeTaiNCKH
{
    public class DeTaiLyThuyet_DTO : DeTai_DTO
    {
        private bool apDungThucTe;

        public DeTaiLyThuyet_DTO() : base()
        { }

        public DeTaiLyThuyet_DTO(string maDT, string tenDT, string chuNhiem, string gvhd, System.DateTime start, System.DateTime end, bool apDung) : base(maDT, tenDT, chuNhiem, gvhd, start, end)
        {
            ApDungThucTe = apDung;
        }

        public bool ApDungThucTe
        {
            get { return apDungThucTe; }
            set { apDungThucTe = value; }
        }

        public override void Nhap()
        {
            Console.WriteLine("-- Nhập Thông Tin Đề Tài Lý Thuyết --");
            base.Nhap();
            Console.Write("  Có áp dụng thực tế không (true/false): ");
            bool.TryParse(Console.ReadLine(), out apDungThucTe);
        }

        public override void Xuat()
        {
            base.Xuat();
            // Console.WriteLine($"   -> Áp dụng thực tế: {ApDungThucTe}");
        }

        public override double TinhKinhPhiCoBan()
        {
            // Nếu có thể áp dụng thực tế: 15 triệu, còn lại 12 triệu
            return ApDungThucTe ? 15000000 : 12000000;

        }
    }
}
