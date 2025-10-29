using System;
using System.Collections.Generic;
using System.Text;
using BLL_QuanLiDeTaiNCKH; 
using DTO_QuanLiDeTaiNCKH; 

namespace GUI_QuanLiDeTaiNCKH
{
    internal class Program
    {
        // --- BLL Object ---
        // Create an instance of the BLL to manage logic
        static QuanLyDeTai_BLL quanLyBLL = new QuanLyDeTai_BLL();

        // --- Main Entry Point ---
        static void Main(string[] args)
        {          
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            ThucHienChucNang(); // Start the main menu loop
        }

        // --- Menu Display ---
        static void HienThiMenu()
        {
            Console.Clear(); // Clear screen for better readability
            Console.WriteLine("=============================================");
            Console.WriteLine("== CHƯƠNG TRÌNH QUẢN LÝ ĐỀ TÀI NCKH       ==");
            Console.WriteLine("=============================================");
            Console.WriteLine(" 1. Đọc danh sách đề tài từ file XML");
            Console.WriteLine(" 2. Thêm mới 1 đề tài từ bàn phím");
            Console.WriteLine(" 3. Xuất danh sách các đề tài");
            Console.WriteLine(" 4. Tìm kiếm đề tài (Mã/Tên/GV/CN)");
            Console.WriteLine(" 5. Xuất danh sách theo tên Giảng viên HD");
            Console.WriteLine(" 6. Cập nhật kinh phí (tăng 10%)");
            Console.WriteLine(" 7. Xuất danh sách ĐT có kinh phí > 10 triệu");
            Console.WriteLine(" 8. Xuất danh sách ĐT Lý thuyết & Áp dụng TT");
            Console.WriteLine(" 9. In ra danh sách ĐT Kinh tế có số câu KS > 100");
            Console.WriteLine("10. In ra danh sách ĐT có thời gian TH > 4 tháng");
            Console.WriteLine(" 0. Thoát chương trình");
            Console.WriteLine("=============================================");
            // Display cost update status
            if (quanLyBLL.DaCapNhatKinhPhi())
            {
                Console.WriteLine("(!) Lưu ý: Kinh phí đang được hiển thị +10%");
            }
            Console.WriteLine("---------------------------------------------");
        }

        // --- Main Function Loop ---
        static void ThucHienChucNang()
        {
            int choice;
            do
            {
                HienThiMenu();
                Console.Write("Nhập lựa chọn của bạn: ");
                // Input validation loop
                while (!int.TryParse(Console.ReadLine(), out choice) || choice < 0 || choice > 10)
                {
                    Console.Write("Lựa chọn không hợp lệ. Vui lòng nhập lại (0-10): ");
                }

                Console.Clear(); // Clear before executing action

                switch (choice)
                {
                    case 1: // Đọc XML 
                        Console.WriteLine("--- 1. Đọc danh sách đề tài từ file XML ---");
                        if (quanLyBLL.NapDuLieu())
                        {
                            Console.WriteLine("Đã nạp dữ liệu thành công.");
                            HienThiKetQua(quanLyBLL.HienThiDanhSach(), "Danh sách đề tài vừa nạp:");
                        }
                        else
                        {
                            Console.WriteLine("Nạp dữ liệu thất bại. Kiểm tra lỗi file XML hoặc đường dẫn.");
                        }
                        break;
                    case 2: // Thêm mới 
                        Console.WriteLine("--- 2. Thêm mới đề tài ---");
                        DeTai_DTO dtMoi = NhapDeTaiMoi_GUI();
                        quanLyBLL.ThemDeTai(dtMoi); // BLL handles adding to list
                        break;
                    case 3: // Xuất danh sách
                        Console.WriteLine("--- 3. Xuất danh sách đề tài ---");
                        HienThiKetQua(quanLyBLL.HienThiDanhSach(), "Danh sách đề tài hiện tại:");
                        break;
                    case 4: // Tìm kiếm 
                        Console.WriteLine("--- 4. Tìm kiếm đề tài ---");
                        Console.Write("Nhập từ khóa tìm kiếm (Mã/Tên/GV/CN): ");
                        string tuKhoa = Console.ReadLine();
                        HienThiKetQua(quanLyBLL.TimKiem(tuKhoa), $"Kết quả tìm kiếm cho '{tuKhoa}':");
                        break;
                    case 5: // Xuất theo GV 
                        Console.WriteLine("--- 5. Xuất danh sách theo GV Hướng dẫn ---");
                        Console.Write("Nhập tên Giảng viên hướng dẫn: ");
                        string tenGV = Console.ReadLine();
                        HienThiKetQua(quanLyBLL.TimKiemTheoGV(tenGV), $"Đề tài của giảng viên '{tenGV}':");
                        break;
                    case 6: // Cập nhật kinh phí 
                        Console.WriteLine("--- 6. Cập nhật kinh phí (tăng 10%) ---");
                        quanLyBLL.CapNhatKinhPhiTang10Percent(); // BLL sets the flag
                        break;
                    case 7: // Lọc KP > 10tr 
                        Console.WriteLine("--- 7. Xuất danh sách đề tài có kinh phí trên 10 triệu ---");
                        HienThiKetQua(quanLyBLL.LocTheoKinhPhiLonHon(10000000), "Danh sách đề tài có kinh phí > 10 triệu:");
                        break;
                    case 8: // Lọc LT & TT 
                        Console.WriteLine("--- 8. Xuất danh sách ĐT Lý thuyết & Áp dụng thực tế ---");
                        HienThiKetQua(quanLyBLL.LocLyThuyetVaThucTe(), "Danh sách đề tài Lý thuyết & Áp dụng thực tế:");
                        break;
                    case 9: // Lọc KT > 100 câu 
                        Console.WriteLine("--- 9. In ra danh sách ĐT Kinh tế có số câu KS > 100 ---");
                        HienThiKetQua(quanLyBLL.LocKhaoSatTren(100), "Danh sách đề tài Kinh tế có số câu KS > 100:");
                        break;
                    case 10: // Lọc TG > 4 tháng 
                        Console.WriteLine("--- 10. In ra danh sách ĐT có thời gian TH 4 tháng trở lên ---");
                        HienThiKetQua(quanLyBLL.LocThoiGianTren(4), "Danh sách đề tài có thời gian TH > 4 tháng:");
                        break;
                    case 0: // Thoát
                        Console.WriteLine("Đang thoát chương trình...");
                        break;
                }

                if (choice != 0)
                {
                    Console.WriteLine("\nNhấn phím bất kỳ để quay lại menu...");
                    Console.ReadKey(); // Pause screen
                }

            } while (choice != 0);
        }

        // --- Helper: Input New Project (GUI Layer) ---
        static DeTai_DTO NhapDeTaiMoi_GUI()
        {
            Console.WriteLine("Chọn loại đề tài cần thêm:");
            Console.WriteLine("  1. Lý thuyết");
            Console.WriteLine("  2. Kinh tế");
            Console.WriteLine("  3. Công nghệ");
            Console.Write("Lựa chọn loại: ");
            int loaiChoice;
            while (!int.TryParse(Console.ReadLine(), out loaiChoice) || loaiChoice < 1 || loaiChoice > 3)
            {
                Console.Write("Loại không hợp lệ. Chọn lại (1-3): ");
            }

            DeTai_DTO dt = null;
            switch (loaiChoice)
            {
                case 1:
                    dt = new DeTaiLyThuyet_DTO();
                    break;
                case 2:
                    dt = new DeTaiKinhTe_DTO();
                    break;
                case 3:
                    dt = new DeTaiCongNghe_DTO();
                    break;
            }

            if (dt != null)
            {
                dt.Nhap();
            }
            return dt;
        }

        // --- Helper: Display Results (GUI Layer) ---
        static void HienThiKetQua(List<DeTai_DTO> danhSach, string title)
        {
            Console.WriteLine($"\n--- {title} ---");
            if (danhSach == null || danhSach.Count == 0)
            {
                Console.WriteLine("Không có đề tài nào để hiển thị.");
                return;
            }

            // --- Adjust Header Widths Again ---
            Console.WriteLine("{0,-12} {1,-40} {2,-32} {3,-35} {4,-12} {5,-12} {6,18}", 
                              "Mã ĐT", "Tên Đề Tài", "Chủ Nhiệm", "GV Hướng Dẫn", "Bắt Đầu", "Kết Thúc", "Kinh Phí (Tr)");
            Console.WriteLine(new string('-', 168)); // Approx: 12+40+32+35+12+12+18 + 6 spaces = 167

            bool updatedCost = quanLyBLL.DaCapNhatKinhPhi();

            foreach (var dt in danhSach)
            {
                double currentTotalCost = quanLyBLL.GetKinhPhiHienThi(dt);
                double displayCostInMillions = currentTotalCost / 1000000.0;

                // --- Adjust Data Line Widths to Match Header ---
                Console.WriteLine("{0,-12} {1,-40} {2,-32} {3,-35} {4,-12:yyyy-MM-dd} {5,-12:yyyy-MM-dd} {6,18:N2}{7}", // Increased widths more
                                   dt.MaDeTai,
                                   dt.TenDeTai,
                                   dt.ChuNhiemDeTai,
                                   dt.GVHuongDan,
                                   dt.NgayBatDau,
                                   dt.NgayKetThuc,
                                   displayCostInMillions,
                                   updatedCost ? " (+10%)" : "");
            }
            Console.WriteLine(new string('-', 168)); 
            Console.WriteLine($"Tổng số đề tài hiển thị: {danhSach.Count}");
        }
    }
}