// DAL_QuanLiDeTaiNCKH/DeTaiDAL.cs
using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using DTO_QuanLiDeTaiNCKH; // Quan trọng: Đảm bảo using đúng namespace DTO
using System.Xml.Linq; // Thêm cho phần ghi file (lấy từ code của Tuấn Kiệt)

namespace DAL_QuanLiDeTaiNCKH // Đảm bảo namespace đúng
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
        // (Tổng hợp và chỉnh sửa từ code của Tuấn, Minh Thuận, Thanh Lộc, Tuấn Kiệt)
        public List<DeTai_DTO> DocDanhSachDeTai()
        {
            List<DeTai_DTO> dsDeTai = new List<DeTai_DTO>();
            string fullPath = Path.GetFullPath(filePath); // Lấy đường dẫn tuyệt đối để kiểm tra

            if (!File.Exists(fullPath))
            {
                Console.WriteLine($"Lỗi: Không tìm thấy file tại '{fullPath}'. Trả về danh sách rỗng.");
                // Tạo file XML rỗng nếu chưa có
                try
                {
                    new XDocument(new XElement("DanhSachDeTai")).Save(fullPath);
                    Console.WriteLine($"Đã tạo file mới: {fullPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Không thể tạo file XML mới: {ex.Message}");
                }
                return dsDeTai;
            }

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(fullPath); // Load từ đường dẫn tuyệt đối
                XmlNodeList nodeList = doc.SelectNodes("/DanhSachDeTai/DeTai");

                foreach (XmlNode node in nodeList)
                {
                    // Ưu tiên đọc từ attribute "Loai" nếu có, nếu không thì đọc từ thẻ <loai>
                    string loaiStr = node.Attributes["Loai"]?.Value; // Thử đọc attribute trước
                    if (string.IsNullOrEmpty(loaiStr))
                    {
                        loaiStr = node["loai"]?.InnerText; // Nếu attribute không có, đọc thẻ <loai>
                    }

                    if (!int.TryParse(loaiStr, out int loai))
                    {
                        Console.WriteLine($"Cảnh báo: Không đọc được loại đề tài cho node: {node.OuterXml}. Bỏ qua.");
                        continue; // Bỏ qua nếu không xác định được loại
                    }


                    // Đọc các thông tin chung một cách an toàn hơn
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
                                bool adtt = bool.TryParse(node["ApDungThucTe"]?.InnerText, out var bAdtt) ? bAdtt : false;
                                dt = new DeTaiLyThuyet_DTO(ma, ten, cn, gv, bd, kt, adtt);
                                break;
                            case 2: // Kinh Tế
                                int cauhoi = int.TryParse(node["SoCauHoiKhaoSat"]?.InnerText, out var iCauhoi) ? iCauhoi : 0;
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
                        // Có thể log chi tiết node gây lỗi: Console.WriteLine(node.OuterXml);
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
            catch (Exception ex) // Bắt các lỗi khác
            {
                Console.WriteLine($"Lỗi không xác định khi đọc file: {ex.Message}");
            }

            return dsDeTai;
        }

        // --- Phương thức ghi file XML ---
        // (Tổng hợp và chỉnh sửa từ code của Tuấn Kiệt)
        public void GhiDanhSachDeTai(List<DeTai_DTO> dsDeTai)
        {
            if (dsDeTai == null)
            {
                Console.WriteLine("Lỗi: Danh sách đề tài rỗng, không thể ghi file.");
                return;
            }

            string fullPath = Path.GetFullPath(filePath);

            try
            {
                // Sử dụng XDocument để ghi cho dễ và có format đẹp hơn
                XElement root = new XElement("DanhSachDeTai"); // Tạo root element

                foreach (var dt in dsDeTai)
                {
                    XElement deTaiElement = new XElement("DeTai"); // Tạo element cho mỗi đề tài

                    // Thêm các element con chung
                    deTaiElement.Add(new XElement("MaDeTai", dt.MaDeTai ?? ""));
                    deTaiElement.Add(new XElement("TenDeTai", dt.TenDeTai ?? ""));
                    deTaiElement.Add(new XElement("ChuNhiemDeTai", dt.ChuNhiemDeTai ?? ""));
                    deTaiElement.Add(new XElement("GVHuongDan", dt.GVHuongDan ?? ""));
                    deTaiElement.Add(new XElement("NgayBatDau", dt.NgayBatDau.ToString("yyyy-MM-dd")));
                    deTaiElement.Add(new XElement("NgayKetThuc", dt.NgayKetThuc.ToString("yyyy-MM-dd")));

                    // Thêm attribute "Loai" và element con riêng tùy theo type
                    if (dt is DeTaiLyThuyet_DTO lt)
                    {
                        deTaiElement.SetAttributeValue("Loai", "LyThuyet"); // Hoặc dùng số 1 nếu muốn
                        deTaiElement.Add(new XElement("loai", "1")); // Thêm thẻ <loai> cho nhất quán đọc
                        deTaiElement.Add(new XElement("ApDungThucTe", lt.ApDungThucTe.ToString().ToLower()));
                    }
                    else if (dt is DeTaiKinhTe_DTO kt)
                    {
                        deTaiElement.SetAttributeValue("Loai", "KinhTe"); // Hoặc dùng số 2
                        deTaiElement.Add(new XElement("loai", "2"));
                        deTaiElement.Add(new XElement("SoCauHoiKhaoSat", kt.SoCauHoiKhaoSat.ToString()));
                    }
                    else if (dt is DeTaiCongNghe_DTO cn)
                    {
                        deTaiElement.SetAttributeValue("Loai", "CongNghe"); // Hoặc dùng số 3
                        deTaiElement.Add(new XElement("loai", "3"));
                        deTaiElement.Add(new XElement("MoiTruongTrienKhai", cn.MoiTruongTrienKhai ?? ""));
                    }

                    root.Add(deTaiElement); // Thêm đề tài vào root
                }

                XDocument doc = new XDocument(root); // Tạo document từ root
                doc.Save(fullPath); // Lưu file (XDocument tự động format)

                Console.WriteLine($"Đã ghi {dsDeTai.Count} đề tài vào file '{fullPath}'.");
            }
            catch (XmlException ex)
            {
                Console.WriteLine($"Lỗi XML khi ghi file: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Lỗi truy cập file IO khi ghi: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi không xác định khi ghi file: {ex.Message}");
            }
        }
    }
}