using System;
using System.Collections.Generic;
using System.Xml; 
using System.IO;  
using DTO_QuanLiDeTaiNCKH; 

namespace DAL_QuanLiDeTaiNCKH 
{
    public class DeTai_DAL
    {
        // Đặt đường dẫn tương đối từ thư mục chạy (bin/Debug) đến file XML
        private string filePath = "../../../Data/DanhSachDeTai.xml";

        // Constructor để kiểm tra/tạo thư mục Data nếu chưa có
        public DeTai_DAL()
        {
            try
            {
                string dataDirectory = Path.GetDirectoryName(Path.GetFullPath(filePath)); // Lấy đường dẫn tuyệt đối
                if (!Directory.Exists(dataDirectory))
                {
                    Directory.CreateDirectory(dataDirectory);
                    Console.WriteLine($"Đã tạo thư mục: {dataDirectory}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi kiểm tra/tạo thư mục Data: {ex.Message}");
            }
        }

        // --- Phương thức đọc file XML ---
        public List<DeTai_DTO> DocDanhSachDeTai()
        {
            List<DeTai_DTO> dsDeTai = new List<DeTai_DTO>();
            string fullPath = Path.GetFullPath(filePath); // Lấy đường dẫn tuyệt đối để kiểm tra

            if (!File.Exists(fullPath))
            {
                Console.WriteLine($"Lỗi: Không tìm thấy file tại '{fullPath}'. Trả về danh sách rỗng.");
                // Nếu file không tồn tại, chỉ trả về danh sách rỗng
                return dsDeTai;
            }

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(fullPath);
                XmlNodeList nodeList = doc.SelectNodes("/DanhSachDeTai/DeTai");

                foreach (XmlNode node in nodeList)
                {
                    // Ưu tiên đọc từ attribute "Loai" nếu có, nếu không thì đọc từ thẻ <loai>
                    string loaiStr = node.Attributes["Loai"]?.Value; 
                    if (string.IsNullOrEmpty(loaiStr))
                    {
                        loaiStr = node["loai"]?.InnerText; // Nếu attribute không có, đọc thẻ <loai>
                    }

                    if (!int.TryParse(loaiStr, out int loai))
                    {
                        Console.WriteLine($"Cảnh báo: Không đọc được loại đề tài cho node: {node.OuterXml}. Bỏ qua.");
                        continue; // Bỏ qua nếu không xác định được loại
                    }

                    string ma = node["MaDeTai"]?.InnerText ?? $"MA_LOI_{Guid.NewGuid()}";
                    string ten = node["TenDeTai"]?.InnerText ?? "N/A";
                    string cn = node["ChuNhiemDeTai"]?.InnerText ?? "N/A";
                    string gv = node["GVHuongDan"]?.InnerText ?? "N/A";
                    DateTime bd = DateTime.TryParse(node["NgayBatDau"]?.InnerText, out var dateBD) ? dateBD : DateTime.MinValue;
                    DateTime kt = DateTime.TryParse(node["NgayKetThuc"]?.InnerText, out var dateKT) ? dateKT : DateTime.MinValue;


                    DeTai_DTO dt = null;

                    try // Thêm try-catch nhỏ để bắt lỗi từng node
                    {
                        switch (loai)
                        {
                            case 1: // Lý Thuyết
                                // Sửa lỗi IDE0075 (cách viết gọn hơn)
                                bool.TryParse(node["ApDungThucTe"]?.InnerText, out bool adtt);
                                dt = new DeTaiLyThuyet_DTO(ma, ten, cn, gv, bd, kt, adtt);
                                break;
                            case 2: // Kinh Tế
                                // Sửa lỗi IDE0075
                                int.TryParse(node["SoCauHoiKhaoSat"]?.InnerText, out int cauhoi);
                                dt = new DeTaiKinhTe_DTO(ma, ten, cn, gv, bd, kt, cauhoi);
                                break;
                            case 3: // Công nghệ
                                string moitruong = node["MoiTruongTrienKhai"]?.InnerText ?? "window";
                                dt = new DeTaiCongNghe_DTO(ma, ten, cn, gv, bd, kt, moitruong);
                                break;
                            default:
                                Console.WriteLine($"Cảnh báo: Loại đề tài không hợp lệ '{loai}' cho mã '{ma}'. Bỏ qua.");
                                break;
                        }

                        if (dt != null)
                        {
                            dsDeTai.Add(dt);
                        }
                    }
                    catch (Exception nodeEx)
                    {
                        Console.WriteLine($"Lỗi xử lý node đề tài mã '{ma}': {nodeEx.Message}");
                    }
                }
                Console.WriteLine($"Đã đọc thành công {dsDeTai.Count} đề tài từ file XML!");
            }
            catch (XmlException ex)
            {
                Console.WriteLine($"Lỗi nghiêm trọng khi đọc file XML: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Lỗi truy cập file IO khi đọc: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi không xác định khi đọc file: {ex.Message}");
            }
            return dsDeTai;
        }
    }
}