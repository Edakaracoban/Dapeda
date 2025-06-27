using Microsoft.AspNetCore.Mvc;
using Dapper;
using Dapeda.Models;
using System.Text;
using System.Security.Cryptography;

namespace Dapeda.Controllers
{
    public class AccountController : Controller
    {
        [HttpPost]
        public IActionResult Login(string email, string passwordHash)
        {
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@Email", email);
            prm.Add("@PasswordHash", passwordHash);
            var user = DP.Get<User>("sp_AdminLogin", prm);
            if (user != null)
            {
                if (email == "adminuser@gmail.com" && passwordHash == "HashAdmin123")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "User"); 
                }
            }
            ViewBag.Error = "Geçersiz kullanıcı bilgileri.";
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string username, string email, string password)
        {
            // Basit şifre hashleme - tavsiye: Daha güvenli yöntemler kullan (bcrypt, PBKDF2, vb.)
            string passwordHash = ComputeSha256Hash(password);

            // Parametreleri hazırla
            DynamicParameters prm = new DynamicParameters();
            prm.Add("@Username", username);
            prm.Add("@Email", email);
            prm.Add("@PasswordHash", passwordHash);

            try
            {

                var newUserId = DP.ExecuteScalar<int>("sp_RegisterUser", prm);

                if (newUserId > 0)
                {

                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ViewBag.Error = "Kayıt sırasında hata oluştu.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Kullanıcı adı veya e-posta zaten kayıtlı olabilir.";
                return View();
            }
        }

        private string ComputeSha256Hash(string rawData)
        {
            if (string.IsNullOrWhiteSpace(rawData))
                throw new ArgumentException("Hash hesaplanacak veri boş olamaz.");

            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        [HttpGet]
        public IActionResult Index(string email = null, string username = null)
        {
            IEnumerable<User> users;

            if (!string.IsNullOrWhiteSpace(email))
            {
                var prm = new DynamicParameters();
                prm.Add("@Email", email);
                users = DP.Listeleme<User>("sp_SearchUserByEmail", prm);
            }
            else if (!string.IsNullOrWhiteSpace(username))
            {
                var prm = new DynamicParameters();
                prm.Add("@UserName", username);
                users = DP.Listeleme<User>("sp_SearchUserByUsername", prm);
            }
            else
            {
                users = DP.Listeleme<User>("sp_GetAllUsers");
            }
            ViewBag.EmailFilter = email;
            ViewBag.UsernameFilter = username;
            return View(users);
        }


        [HttpGet]
        public IActionResult EditUser(int id)
        {
            var param = new DynamicParameters();
            param.Add("@UserId", id);
            var user = DP.Get<User>("sp_GetUserById", param);
            if (user == null)
                return NotFound();

            return View(user);
        }


        [HttpPost]
        public IActionResult EditUser(User user)
        {
            if (!ModelState.IsValid)
                return View(user);

            // 1. Mevcut kullanıcıyı veritabanından çek (parolasını almak için)
            var existingParam = new DynamicParameters();
            existingParam.Add("@UserId", user.UserId);
            var existingUser = DP.Get<User>("sp_GetUserById", existingParam);

            if (existingUser == null)
            {
                ViewBag.Error = "Kullanıcı bulunamadı.";
                return View(user);
            }

            // 2. Parametreleri oluştur
            var param = new DynamicParameters();
            param.Add("@UserId", user.UserId);
            param.Add("@UserName", user.UserName);
            param.Add("@Email", user.Email);

            // 3. Parola formda yoksa veritabanından geleni kullan
            param.Add("@PasswordHash", existingUser.PasswordHash ?? "");

            try
            {
                DP.ExecuteReturn("sp_UpdateUser", param);
                TempData["SuccessMessage"] = "Kullanıcı başarıyla güncellendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Kullanıcı güncellenemedi: " + ex.Message;
                return View(user);
            }
        }


        private bool HasOrders(int userId)
        {
            var param = new DynamicParameters();
            param.Add("@UserId", userId);

            int orderCount = DP.ExecuteScalar<int>("sp_UserHasOrders", param);
            return orderCount > 0;
        }

        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                if (HasOrders(id))
                {
                    TempData["ErrorMessage"] = "Bu kullanıcı silinemez çünkü ona ait siparişler mevcut.";
                    return RedirectToAction("Index");
                }

                var param = new DynamicParameters();
                param.Add("@UserId", id);

                DP.ExecuteReturn("sp_DeleteUser", param);

                TempData["SuccessMessage"] = "Kullanıcı başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kullanıcı silinirken hata oluştu: " + ex.Message;
            }

            return RedirectToAction("Index");
        }




    }
}
