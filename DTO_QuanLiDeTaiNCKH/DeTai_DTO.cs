using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO_QuanLiDeTaiNCKH
{
    public abstract class DeTai_DTO
    {
        protected string maDeTai;
        protected string tenDeTai;
        protected string chuNhiemDeTai;
        protected string gvHuongDan;
        protected System.DateTime ngayBatDau;
        protected System.DateTime ngayKetThuc;

        public DeTai_DTO()
        {
            NgayBatDau = DateTime.Now; //Ngày bắt đầu mặc định
            NgayKetThuc = DateTime.Now.AddMonths(1); //Ngày kết thúc mặc định(ví dụ: sau 1 tháng)
        }

        public DeTai_DTO(string maDT, string tenDT, string chuNhiem, string gvhd, System.DateTime start, System.DateTime end)
        {
            MaDeTai = maDT;
            TenDeTai = tenDT;
            ChuNhiemDeTai = chuNhiem;
            GVHuongDan = gvhd;
            NgayBatDau = start;
            NgayKetThuc = end;
        }

        public string MaDeTai
        {
            get { return maDeTai; }
            set { maDeTai = value; }
        }

        public string TenDeTai
        {
            get { return tenDeTai; }
            set { tenDeTai = value; }
        }

        public string ChuNhiemDeTai
        {
            get { return chuNhiemDeTai; }
            set { chuNhiemDeTai = value; }
        }

        public string GVHuongDan
        {
            get { return gvHuongDan; }
            set { gvHuongDan = value; }
        }

        public System.DateTime NgayBatDau
        {
            get { return ngayBatDau; }
            set { ngayBatDau = value; }

        }

        public System.DateTime NgayKetThuc
        {
            get { return ngayKetThuc; }
            set
            {
                if (value >= ngayBatDau)
                {
                    ngayKetThuc = value;
                }
                else
                {
                    Console.WriteLine("Lỗi: Ngày kết thúc phải sau ngày bắt đầu.");
                    //Hoặc đặt giá trị mặc định, ví dụ ngayKetThuc = ngayBatDau
                }
            }
        }

        public virtual void Nhap()
        {
            Console.Write("  Nhập Mã đề tài: ");
            MaDeTai = Console.ReadLine();
            Console.Write("  Nhập Tên đề tài: ");
            TenDeTai = Console.ReadLine();
            Console.Write("  Nhập Chủ nhiệm đề tài (SV): ");
            ChuNhiemDeTai = Console.ReadLine();
            Console.Write("  Nhập GV Hướng dẫn: ");
            GVHuongDan = Console.ReadLine();
            
            try
            {
                Console.Write("  Nhập Ngày bắt đầu (yyyy-MM-dd): ");
                NgayBatDau = DateTime.Parse(Console.ReadLine());
                Console.Write("  Nhập Ngày kết thúc (yyyy-MM-dd): ");
                NgayKetThuc = DateTime.Parse(Console.ReadLine());
            }
            catch (FormatException)
            {
                Console.WriteLine("  Lỗi định dạng ngày. Sử dụng ngày mặc định.");
                NgayBatDau = DateTime.Today;
                NgayKetThuc = DateTime.Today.AddMonths(1);
            }
        }

        public virtual void XuatHeader()
        {
            Console.WriteLine("{0,-12} {1,-35} {2,-25} {3,-25} {4,-12} {5,-12} {6,18}", 
                      "Mã ĐT", "Tên Đề Tài", "Chủ Nhiệm", "GV Hướng Dẫn", "Bắt Đầu", "Kết Thúc", "Kinh Phí (Tr)");          
            Console.WriteLine(new string('-', 144));
        }

        public virtual void Xuat()
        {
            double totalCost = TinhTongKinhPhi() / 1000000.0;
            Console.WriteLine("{0,-12} {1,-35} {2,-25} {3,-25} {4,-12:yyyy-MM-dd} {5,-12:yyyy-MM-dd} {6,18:N2}",
                              MaDeTai, TenDeTai, ChuNhiemDeTai, GVHuongDan, NgayBatDau, NgayKetThuc, totalCost);
        }

        public double TinhThoiGianThucHien()
        {
            // Sử dụng TimeSpan để tính chính xác tổng số ngày
            TimeSpan duration = NgayKetThuc - NgayBatDau;
            double totalDays = duration.TotalDays;

            // Chuyển đổi tổng số ngày sang tháng (dùng số ngày TB trong 1 tháng)
            // (365.25 ngày/năm / 12 tháng/năm ≈ 30.4375)
            double totalMonths = totalDays / 30.4375;

            return totalMonths;
        }

        public abstract double TinhKinhPhiCoBan();

        public double TinhTongKinhPhi()
        {
            double baseCost = TinhKinhPhiCoBan();
            double supportCost = 0;

            // Kiểm tra đối tượng hiện tại có cài đặt interface IHoTroNghienCuu hay không(để cộng phí hỗ trợ)
            if (this is IHoTroNghienCuu hoTroProvider)
            {
                supportCost = hoTroProvider.TinhPhiHoTro();
            }
            return baseCost + supportCost;
        }
    }
}
