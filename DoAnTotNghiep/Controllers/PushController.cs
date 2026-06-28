using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using DoAnTotNghiep.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    public class PushController : Controller
    {
        private readonly PushSubscription_DAL pushSubscriptionDAL;
        private readonly PushNotificationService pushNotificationService;

        public PushController(PushSubscription_DAL pushSubscriptionDAL, PushNotificationService pushNotificationService)
        {
            this.pushSubscriptionDAL = pushSubscriptionDAL;
            this.pushNotificationService = pushNotificationService;
        }

        [HttpGet]
        // lấy public key push
        public IActionResult PublicKey()
        {
            return Json(new { publicKey = pushNotificationService.LayPublicKey() });
        }

        [HttpPost]
        // đăng ký nhận thông báo
        public IActionResult DangKy([FromBody] PushSubscriptionRequest? request)
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null)
            {
                return Unauthorized();
            }

            if (request?.Keys == null
                || string.IsNullOrWhiteSpace(request.Endpoint)
                || string.IsNullOrWhiteSpace(request.Keys.P256dh)
                || string.IsNullOrWhiteSpace(request.Keys.Auth))
            {
                return BadRequest();
            }

            pushSubscriptionDAL.LuuDangKy(maTaiKhoan.Value, request);
            return Ok();
        }

        [HttpPost]
        // hủy đăng ký thông báo
        public IActionResult HuyDangKy([FromBody] PushSubscriptionRequest? request)
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null)
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(request?.Endpoint))
            {
                return BadRequest();
            }

            pushSubscriptionDAL.XoaTheoEndpoint(request.Endpoint);
            return Ok();
        }

        private int? LayMaTaiKhoanUser()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null)
            {
                return null;
            }

            var vaiTro = HttpContext.Session.GetString("VaiTro");
            return vaiTro == "User" ? maTaiKhoan.Value : null;
        }
    }
}
