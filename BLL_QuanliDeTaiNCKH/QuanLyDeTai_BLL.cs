using System;
using System.Collections.Generic;
using System.Linq; 
using DTO_QuanLiDeTaiNCKH;
using DAL_QuanLiDeTaiNCKH;

namespace BLL_QuanLiDeTaiNCKH 
{
    public class QuanLyDeTai_BLL
    {
        private DeTai_DAL deTaiDAL; // Đối tượng DAL để đọc/ghi file
        private List<DeTai_DTO> danhSachDeTai; // Danh sách đề tài trong bộ nhớ
        private bool daCapNhatKinhPhi = false; // Cờ đánh dấu đã tăng kinh phí hay chưa

        public QuanLyDeTai_BLL()
        {
            deTaiDAL = new DeTai_DAL();
            // Không nên gọi NapDuLieu() ở đây để GUI quyết định khi nào cần đọc file
            danhSachDeTai = new List<DeTai_DTO>(); // Khởi tạo danh sách rỗng ban đầu
        }

        // --- Các phương thức thao tác dữ liệu ---

        // 1. Đọc dữ liệu từ DAL 
        public bool NapDuLieu()
        {
            danhSachDeTai = deTaiDAL.DocDanhSachDeTai();
            daCapNhatKinhPhi = false; // Reset cờ cập nhật khi nạp lại dữ liệu
            if (danhSachDeTai == null) // Xử lý trường hợp DAL trả về null (lỗi đọc)
            {
                danhSachDeTai = new List<DeTai_DTO>(); // Khởi tạo lại danh sách rỗng
                Console.WriteLine("Lỗi: Không thể nạp dữ liệu từ file.");
                return false;
            }
            // Console.WriteLine($"Đã nạp {danhSachDeTai.Count} đề tài.");
            return true; 
        }

        // 10. Lưu dữ liệu qua DAL 
        public void LuuDuLieu()
        {
            if (danhSachDeTai != null)
            {
                deTaiDAL.GhiDanhSachDeTai(danhSachDeTai);
            }
            else
            {
                Console.WriteLine("Lỗi: Danh sách đề tài chưa được khởi tạo để lưu.");
            }
        }

        // 2. Thêm đề tài mới (Nhiệm vụ của Tuấn Kiệt)
        public bool ThemDeTai(DeTai_DTO dt)
        {
            if (dt == null)
            {
                Console.WriteLine("Lỗi: Không thể thêm đề tài không hợp lệ (null).");
                return false;
            }
            // Kiểm tra trùng mã đề tài (không phân biệt hoa thường)
            if (danhSachDeTai.Any(d => d.MaDeTai.Equals(dt.MaDeTai, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine($"Lỗi: Mã đề tài '{dt.MaDeTai}' đã tồn tại.");
                return false;
            }

            danhSachDeTai.Add(dt);
            Console.WriteLine("Đã thêm đề tài mới thành công.");
            // Lưu ý: Việc ghi vào file nên được gọi riêng biệt (ví dụ: chức năng số 10)
            // LuuDuLieu(); // Không nên tự động lưu ở đây
            return true;
        }

        // 3. Lấy toàn bộ danh sách để hiển thị (Nhiệm vụ của Tuấn Kiệt)
        public List<DeTai_DTO> HienThiDanhSach()
        {
            // Trả về danh sách hiện tại để GUI hiển thị
            return danhSachDeTai;
        }

        // --- Các phương thức tìm kiếm và lọc ---

        // 4. Tìm kiếm (Nhiệm vụ của Minh Thuận)
        public List<DeTai_DTO> TimKiem(string tuKhoa)
        {
            if (string.IsNullOrWhiteSpace(tuKhoa) || danhSachDeTai == null)
            {
                return new List<DeTai_DTO>(); // Trả về rỗng nếu không có từ khóa hoặc danh sách
            }

            string lowerTuKhoa = tuKhoa.ToLower().Trim(); // Chuẩn hóa từ khóa
            return danhSachDeTai
                .Where(dt => (dt.MaDeTai?.ToLower().Contains(lowerTuKhoa) ?? false) ||
                             (dt.TenDeTai?.ToLower().Contains(lowerTuKhoa) ?? false) ||
                             (dt.GVHuongDan?.ToLower().Contains(lowerTuKhoa) ?? false) ||
                             (dt.ChuNhiemDeTai?.ToLower().Contains(lowerTuKhoa) ?? false))
                .ToList();
        }

        // 5. Tìm theo tên GV (Nhiệm vụ của Minh Thuận)
        public List<DeTai_DTO> TimKiemTheoGV(string tenGV)
        {
            if (string.IsNullOrWhiteSpace(tenGV) || danhSachDeTai == null)
            {
                return new List<DeTai_DTO>();
            }
            string lowerTenGV = tenGV.ToLower().Trim();
            return danhSachDeTai
                .Where(dt => dt.GVHuongDan?.ToLower().Contains(lowerTenGV) ?? false)
                .ToList();
        }

        // 6. Cập nhật kinh phí (Nhiệm vụ của Tuấn) - Chỉ đánh dấu cờ
        public void CapNhatKinhPhiTang10Percent()
        {
            daCapNhatKinhPhi = true;
            Console.WriteLine("Đã đánh dấu cập nhật kinh phí. Các lần xuất sau sẽ hiển thị kinh phí tăng 10%.");
        }

        // Helper để tính kinh phí (có áp dụng cờ cập nhật)
        private double TinhKinhPhiHienTai(DeTai_DTO dt)
        {
            return daCapNhatKinhPhi ? dt.TinhTongKinhPhi() * 1.1 : dt.TinhTongKinhPhi();
        }


        // 7. Lọc theo kinh phí > 10 triệu (Nhiệm vụ của Phạm Kiệt) - Áp dụng cờ cập nhật
        public List<DeTai_DTO> LocTheoKinhPhiLonHon(double nguong)
        {
            if (danhSachDeTai == null) return new List<DeTai_DTO>();
            return danhSachDeTai.Where(dt => TinhKinhPhiHienTai(dt) > nguong).ToList();
        }

        // 8. Lọc Lý thuyết và Thực tế (Nhiệm vụ của Thành Lộc)
        public List<DeTai_DTO> LocLyThuyetVaThucTe()
        {
            if (danhSachDeTai == null) return new List<DeTai_DTO>();
            return danhSachDeTai
                .OfType<DeTaiLyThuyet_DTO>() // Chỉ lấy đề tài lý thuyết
                .Where(lt => lt.ApDungThucTe == true)
                .Cast<DeTai_DTO>() // Ép kiểu về lại DeTai_DTO
                .ToList();
        }

        // 9. Lọc Kinh tế có khảo sát > 100 câu (Nhiệm vụ của Thành Lộc)
        public List<DeTai_DTO> LocKhaoSatTren(int soCau)
        {
            if (danhSachDeTai == null) return new List<DeTai_DTO>();
            return danhSachDeTai
                .OfType<DeTaiKinhTe_DTO>() // Chỉ lấy đề tài kinh tế
                .Where(kt => kt.SoCauHoiKhaoSat > soCau)
                .Cast<DeTai_DTO>() // Ép kiểu về lại DeTai_DTO
                .ToList();
        }

        // 10. Lọc theo thời gian > 4 tháng (Nhiệm vụ của Thành Lộc)
        public List<DeTai_DTO> LocThoiGianTren(int soThang)
        {
            if (danhSachDeTai == null) return new List<DeTai_DTO>();
            return danhSachDeTai
                .Where(dt => dt.TinhThoiGianThucHien() > soThang)
                .ToList();
        }

        // Phương thức getter cho cờ daCapNhatKinhPhi để GUI có thể biết và hiển thị phù hợp
        public bool DaCapNhatKinhPhi()
        {
            return daCapNhatKinhPhi;
        }

        // Phương thức để tính toán và hiển thị kinh phí (kết hợp với cờ) - Dùng trong GUI
        public double GetKinhPhiHienThi(DeTai_DTO dt)
        {
            if (dt == null) return 0;
            return TinhKinhPhiHienTai(dt);
        }
    }
}