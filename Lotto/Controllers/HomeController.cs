using Lotto.Dtos;
using Lotto.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using X.PagedList.Extensions;

namespace Lotto.Controllers
{
    public class HomeController : Controller
    {
        private readonly GameContext2 _Context;

        public HomeController(GameContext2 gameContext)
        {
            _Context = gameContext;
        }

        // 首頁
        public IActionResult Clock()
        {
            return View();
        }

        // 遊戲介紹
        public IActionResult Index()
        {
            return View();
        }

        // 會員註冊
        public IActionResult Register()
        {
            return View();
        }

        // 會員註冊
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {

            // T-SQL 特別注意有輸出參數需加上out
            var sql = "exec CreatePlayer @PlayerName, @Login, @Password, @Email, @Password_hint, @Status out";

            // 建立輸出參數 , 特別注意Direction為Output
            var returnValueParam = new SqlParameter
            {
                ParameterName = "@Status",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            // 建立參數
            var parameters = new[]
            {
            new SqlParameter("@PlayerName", registerDto.PlayerName),
            new SqlParameter("@Login", registerDto.Login),
            new SqlParameter("@Password", registerDto.Password),
            new SqlParameter("@Email", registerDto.Email),
            new SqlParameter("@Password_hint", registerDto.Password_hint),
            returnValueParam
            };

            // 執行 SQL 命令
            var result = await _Context.Database.ExecuteSqlRawAsync(sql, parameters);

            // 取出returnValueParam的順位轉成數值
            int returnValue = Convert.ToInt32(parameters[5].Value);

            // 判斷輸出狀態
            if (returnValue == -1)
            {
                // 帳號重複 , 加入ModelState
                ModelState.AddModelError("Login", "帳號已經存在");
            }
            else if (returnValue == -2)
            {
                // 密碼重複 , 加入ModelState
                ModelState.AddModelError("Password", "密碼已經存在");
            }
            else if (returnValue == -3)
            {
                // 名稱重複 , 加入ModelState
                ModelState.AddModelError("PlayerName", "名稱已經存在");
            }
            else if (returnValue == 1)
            {
                // 註冊成功
                return RedirectToAction(nameof(Login));
            }

            return View(registerDto);

        }

        // 登入畫面
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        // 登入畫面
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            // T-SQL 特別注意有輸出參數需加上out
            var sql = "exec LoginCheck @Login, @Password, @Status out";

            // 建立輸出參數 , 特別注意Direction為Output
            var OutputValueParam = new SqlParameter
            {
                ParameterName = "@Status",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            // 建立參數
            var parameters = new[]
            {
            new SqlParameter("@Login", loginDto.Login),
            new SqlParameter("@Password", loginDto.Password),
            OutputValueParam
            };

            // 執行 SQL 命令
            var result = await _Context.Database.ExecuteSqlRawAsync(sql, parameters);

            // 取出returnValueParam的順位轉成數值
            int returnValue = Convert.ToInt32(parameters[2].Value);
              
            // 判斷輸出狀態
            if (returnValue == -1)
            {
                // 帳號錯誤
                ModelState.AddModelError("Login", "帳號錯誤");
            }
            else if (returnValue == -2)
            {
                // 密碼錯誤
                ModelState.AddModelError("Password", "密碼錯誤");
            }
            else if (returnValue == 1)
            {

                // 將User的帳號存入cookie
                HttpContext.Response.Cookies.Append("Login", loginDto.Login);
                // 註冊成功

                // 驗證開始-------------------------------------------------------------------------------------------------------驗證開始
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, loginDto.Login) // 將用戶名加入聲明中
                };
                // 創建 ClaimsIdentity
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // 創建 ClaimsPrincipal
                var userPrincipal = new ClaimsPrincipal(claimsIdentity);

                // 簽入用戶
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);


                // 驗證結束-------------------------------------------------------------------------------------------------------驗證結束

                return RedirectToAction("Findinfo", "Member");
            }

            return View();
        }

        // 開獎結果
        public async Task<IActionResult> Querylotto(int? ladder, int? page)
        {
            // 設定一頁20筆資料
            int Pagesize = 10;
            // 設定目前的頁數 , 預設第一頁
            int pageNumber = (page ?? 1);

            if (ladder != null)
            {
                var result1 = await _Context.LottoDto.FromSqlRaw($"EXEC LottoResult_one {ladder}").ToListAsync();
                return View(result1.ToPagedList(pageNumber, Pagesize));
            }

            var result2 = await _Context.LottoDto.FromSqlRaw("EXEC LottoResult").ToListAsync();

            return View(result2.ToPagedList(pageNumber, Pagesize));
        }

        public string Sucess()
        {
            return "success";
        }

        // 找回密碼
        public IActionResult Findpassword()
        {
            return View();
        }

        // 找回密碼
        [HttpPost]
        public IActionResult Findpassword(FindpasswordDto findpassword)
        {
            // T-SQL 特別注意有輸出參數需加上out
            var sql = "exec Findpassword @PlayerName, @Password_hint, @Password out, @Status out";

            // 建立輸出參數 , 特別注意Direction為Output
            var OutputValueParam_Password = new SqlParameter
            {
                ParameterName = "@Password",
                SqlDbType = SqlDbType.NVarChar,
                Size = 20,
                Direction = ParameterDirection.Output
            };

            var OutputValueParam_Status = new SqlParameter
            {
                ParameterName = "@Status",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            // 建立參數
            var parameters = new[]
            {
            new SqlParameter("@PlayerName", findpassword.PlayerName),
            new SqlParameter("@Password_hint", findpassword.Password_hint),
            OutputValueParam_Password,
            OutputValueParam_Status
            };

            // 執行 SQL 命令
            var result = _Context.Database.ExecuteSqlRaw(sql, parameters);

            // 取出returnValueParam的順位轉成數值
            string getpassword = Convert.ToString(OutputValueParam_Password.Value);
            int getstatus = Convert.ToInt32(parameters[3].Value); ;

            // 判斷輸出狀態
            if (getstatus == -1)
            {
                // 查詢不到
                ModelState.AddModelError("PlayerName", "名稱或第二組密碼有誤");
                ViewBag.Password = "";
            }
            else if (getstatus == 1)
            {
                // 查詢得到
                ViewBag.Password = getpassword;
            }

            return View();
        }

    }
}
