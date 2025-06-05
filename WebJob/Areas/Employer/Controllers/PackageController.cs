using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebJob.Filters;
using WebJob.Models;
using WebJob.Models.EF;
using WebJob.Models.payment;

namespace WebJob.Areas.Employer.Controllers
{
    [CustomAuthorize(Area = "Employer", Roles = "Employer")]
    public class PackageController : Controller
    {
        private ApplicationDbContext _context = new ApplicationDbContext();
        public ActionResult Index()
        {
            var packages = _context.products
                .Where(p => p.IsActive)
                .ToList();

            return View(packages);
        }
        public ActionResult Details(int id)
        {
            var package = _context.products
                .FirstOrDefault(p => p.ProductID == id && p.IsActive);

            if (package == null)
            {
                return HttpNotFound();
            }

            return View(package);
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Purchase(int productId, int quantity = 1)
        {
            var user = _context.Users.Find(User.Identity.GetUserId());
            var package = _context.products.Find(productId);

            // Kiểm tra hợp lệ
            if (package == null || !package.IsActive || quantity <= 0)
            {
                return HttpNotFound();
            }

            // Tạo đơn hàng mới
            var order = new Order
            {
                Code = "PKG" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                CustomerName = user.FullName ?? user.UserName,
                Phone = user.Phone,
                Email = user.Email,
                TotalAmount = package.Price * quantity,
                TypePayment = 2, // VNPay
                Status = 1, // Chưa thanh toán
                UserId = user.Id
            };
            order.CreatedBy = user.Email;
            order.CreatedDate = DateTime.Now;
            if (package.PriceSale > 0)
            {
                order.TotalAmount = (decimal)package.PriceSale * quantity;
            }
            // Thêm chi tiết đơn hàng
            order.OrderDetails.Add(new OrderDetail
            {
                ProductID = package.ProductID,
                Price = package.Price,
                Quantity = quantity
            });

            _context.orders.Add(order);
            _context.SaveChanges();

            // Tạo URL thanh toán VNPay và chuyển hướng
            string paymentUrl = CreateVNPayPaymentUrl(order);
            return Redirect(paymentUrl);
        }
        [AllowAnonymous]
        public ActionResult VnpayReturn()
        {
            if (Request.QueryString.Count == 0)
            {
                return RedirectToAction("Index");
            }
            var user = _context.Users.Find(User.Identity.GetUserId());
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
            var vnpayData = Request.QueryString;
            var vnpay = new VnPayLibrary();

            // Lấy dữ liệu từ VNPay
            foreach (string key in vnpayData.AllKeys)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, vnpayData[key]);
                }
            }

            // Xác thực chữ ký
            string vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
            bool validSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);

            if (!validSignature)
            {
                ViewBag.Message = "Chữ ký không hợp lệ";
                return View();
            }

            string orderCode = vnpay.GetResponseData("vnp_TxnRef");
            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
            long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
            string vnp_BankCode = vnpay.GetResponseData("vnp_BankCode");
            string vnp_PayDate = vnpay.GetResponseData("vnp_PayDate");

            // Kiểm tra kết quả giao dịch
            if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
            {
                var order = _context.orders
                    .Include(o => o.OrderDetails)
                    .FirstOrDefault(o => o.Code == orderCode);

                if (order != null && order.Status == 1) // Chỉ xử lý nếu đơn hàng chưa thanh toán
                {
                    // Cập nhật trạng thái đơn hàng
                    order.Status = 2; // Đã thanh toán
                    order.ModifiedDate = DateTime.Now;
                    _context.Entry(order).State = EntityState.Modified;

                    // Cập nhật thông tin gói dịch vụ cho nhà tuyển dụng
                    var verification = _context.employerVerifications
                        .FirstOrDefault(e => e.UserId == order.UserId);

                    if (verification != null)
                    {
                        foreach (var item in order.OrderDetails)
                        {
                            var product = _context.products.Find(item.ProductID);

                            // Thêm vào lịch sử mua gói
                            var evp = new EmployerVerificationProduct
                            {
                                EmployerVerificationId = verification.Id,
                                ProductId = product.ProductID,
                                Quantity = item.Quantity,
                                Price = item.Price,
                                TransactionId = vnpay.GetResponseData("vnp_TransactionNo"),
                                PaymentMethod = 2, // VNPay
                                PurchaseDate = DateTime.Now
                            };

                            _context.EmployerVerificationProducts.Add(evp);

                            // Cập nhật số bài đăng và thời hạn
                            verification.PostRemain += product.Quantity * item.Quantity;
                            verification.ValidityDays = product.ValidityDays;

                            if (product.ValidityDays > 0)
                            {
                                verification.ExpiryDate = DateTime.Now.AddDays(product.ValidityDays);
                            }
                        }
                    }

                    _context.SaveChanges();

                    // Gửi email xác nhận
                    //SendConfirmationEmail(order, user);

                    // Thông báo thành công
                    ViewBag.Success = true;
                    ViewBag.Message = "Thanh toán gói dịch vụ thành công";
                    ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                    ViewBag.OrderCode = order.Code;
                    ViewBag.Amount = vnp_Amount.ToString("N0") + " VND";
                    ViewBag.BankCode = vnp_BankCode;
                    ViewBag.PayDate = FormatPayDate(vnp_PayDate);
                }
                else
                {
                    ViewBag.Message = "Đơn hàng đã được xử lý hoặc không tồn tại";
                }
            }
            else
            {
                ViewBag.Message = $"Thanh toán không thành công. Mã lỗi: {vnp_ResponseCode}";
            }

            return View();
        }

        // Tạo URL thanh toán VNPay
        private string CreateVNPayPaymentUrl(Order order)
        {
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"];
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"];
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];

            var vnpay = new VnPayLibrary();
            var amount = (long)order.TotalAmount * 100; // VNPay yêu cầu số tiền nhân 100

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", amount.ToString());
            vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toán gói dịch vụ: {order.Code}");
            vnpay.AddRequestData("vnp_OrderType", "billpayment");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.Code);

            // Thêm thông tin khách hàng nếu có
            if (!string.IsNullOrEmpty(order.Email))
            {
                vnpay.AddRequestData("vnp_Bill_Email", order.Email);
            }
            if (!string.IsNullOrEmpty(order.Phone))
            {
                vnpay.AddRequestData("vnp_Bill_Mobile", order.Phone);
            }

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return paymentUrl;
        }
        private string FormatPayDate(string payDate)
        {
            if (string.IsNullOrEmpty(payDate)) return string.Empty;

            try
            {
                // Định dạng: yyyyMMddHHmmss
                DateTime date = DateTime.ParseExact(payDate, "yyyyMMddHHmmss", null);
                return date.ToString("dd/MM/yyyy HH:mm");
            }
            catch
            {
                return payDate;
            }
        }

    }
}