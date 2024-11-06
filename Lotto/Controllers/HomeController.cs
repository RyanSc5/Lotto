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
using Dapper;

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
        public IActionResult Register(RegisterDto registerdto)
        {
            // 定義連線字串，通常從組態中取得
            var connectionString = _Context.Database.GetConnectionString();

            // 使用 Dapper 直接呼叫預存程序
            using (var connection = new SqlConnection(connectionString))
            {
                // 設定存儲過程的參數(輸入)
                var parameters = new DynamicParameters();
                parameters.Add("@Playername", registerdto.PlayerName, DbType.String);
                parameters.Add("@Login", registerdto.Login, DbType.String);
                parameters.Add("@Password", registerdto.Password, DbType.String);
                parameters.Add("@Email", registerdto.Email, DbType.String);
                parameters.Add("@Password_hint", registerdto.Password_hint, DbType.String);

                // 設定存儲過程的參數(輸出)
                parameters.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.Output);

                // 呼叫 CreatePlayer 預存程序
                connection.Execute("CreatePlayer", parameters, commandType: CommandType.StoredProcedure);

                // 取得輸出參數的值
                int resultCount = parameters.Get<int>("@Status");

                if (resultCount == -1)
                {
                    // 帳號重複 , 加入ModelState
                    ModelState.AddModelError("Login", "帳號已經存在");
                }
                else if (resultCount == -2)
                {
                    // 密碼重複 , 加入ModelState
                    ModelState.AddModelError("Password", "密碼已經存在");
                }
                else if (resultCount == -3)
                {
                    // 名稱重複 , 加入ModelState
                    ModelState.AddModelError("PlayerName", "名稱已經存在");
                }
                else if (resultCount == 1)
                {
                    // 註冊成功
                    TempData["SuccessOfRegister"] = "註冊成功";
                    return RedirectToAction(nameof(Login));
                }

                return View(registerdto);
            }
        }

        // 登入畫面
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // 登入畫面
        [HttpPost]
        public IActionResult Login(LoginDto loginDto)
        {
            // 定義連線字串，通常從組態中取得
            var connectionString = _Context.Database.GetConnectionString();

            // 使用 Dapper 直接呼叫預存程序
            using (var connection = new SqlConnection(connectionString))
            {
                // 設定存儲過程的參數(輸入)
                var parameters = new DynamicParameters();
                parameters.Add("@Login", loginDto.Login, DbType.String);
                parameters.Add("@Password", loginDto.Password, DbType.String);               

                // 設定存儲過程的參數(輸出)
                parameters.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.Output);

                // 呼叫 LoginCheck 預存程序
                connection.Execute("LoginCheck", parameters, commandType: CommandType.StoredProcedure);

                // 取得輸出參數的值
                int resultCount = parameters.Get<int>("@Status");

                if (resultCount == -1)
                {
                    // 帳號錯誤
                    ModelState.AddModelError("Login", "帳號錯誤");
                }
                else if (resultCount == -2)
                {
                    // 密碼錯誤
                    ModelState.AddModelError("Password", "密碼錯誤");
                }
                else if (resultCount == 1)
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
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);

                    // 驗證結束-------------------------------------------------------------------------------------------------------驗證結束

                    return RedirectToAction("Findinfo", "Member");
                }

                return View(loginDto);
            }
            
        }

        // 開獎結果
        public IActionResult Querylotto(int? ladder, int? page)
        {
            // 設定一頁10筆資料
            int Pagesize = 10;
            // 設定目前的頁數 , 預設第一頁
            int pageNumber = (page ?? 1);

            // 定義連線字串，通常從組態中取得
            var connectionString = _Context.Database.GetConnectionString();

            // 使用 Dapper 呼叫預存程序(FindLottoResult)
            using (var connection = new SqlConnection(connectionString))
            {
                // 獎期查詢
                if (ladder != null)
                {
                    // 定義參數
                    var parameters = new { Ladder = ladder };

                    // SP執行
                    var result = connection.Query<LottoDto>("FindLottoResult", parameters, commandType: CommandType.StoredProcedure).ToList();
                    return View(result.ToPagedList(pageNumber, Pagesize));
                }

                // 查前100筆
                var result_all = connection.Query<LottoDto>("FindLottoResult", null, commandType: CommandType.StoredProcedure).ToList();
                return View(result_all.ToPagedList(pageNumber, Pagesize));
            }

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
            // 定義連線字串，通常從組態中取得
            var connectionString = _Context.Database.GetConnectionString();

            // 使用 Dapper 直接呼叫預存程序
            using (var connection = new SqlConnection(connectionString))
            {
                // 設定存儲過程的參數(輸入)
                var parameters = new DynamicParameters();
                parameters.Add("@PlayerName", findpassword.PlayerName, DbType.String);
                parameters.Add("@Password_hint", findpassword.Password_hint, DbType.String);               

                // 設定存儲過程的參數(輸出)
                parameters.Add("@Password", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
                parameters.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.Output);

                // 呼叫 Findpassword 預存程序
                connection.Execute("Findpassword", parameters, commandType: CommandType.StoredProcedure);

                // 取得輸出參數的值
                string resultCount_Password = parameters.Get<string>("@Password");
                int resultCount_Status = parameters.Get<int>("@Status");

                // 判斷輸出狀態
                if (resultCount_Status == -1)
                {
                    // 查詢不到
                    ModelState.AddModelError("PlayerName", "名稱或第二組密碼有誤");
                    ViewBag.Password = "";
                }
                else if (resultCount_Status == 1)
                {
                    // 查詢得到
                    ViewBag.Password = resultCount_Password;
                }

                return View();
            }
        }

    }
}
