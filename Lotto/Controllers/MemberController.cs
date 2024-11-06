using Lotto.Dtos;
using Lotto.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using X.PagedList.Extensions;
using System.Threading;

/* 下面3個是Dapper加的 */
using Dapper;  // 引入 Dapper 命名空間
using Microsoft.Data.SqlClient;  // 引入 SQL Server 的命名空間
using System.Linq;

/*驗證mail*/
using System.Net.Mail;

using Lotto.ViewModel;

namespace Lotto.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly GameContext2 _Context;

        public MemberController(GameContext2 gameContext)
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

        // 查詢個人資料
        public IActionResult Findinfo()
        {    
            // 取出玩家帳號的cookie
            var login = HttpContext.Request.Cookies["Login"];

            // 定義連線字串，通常從組態中取得
            var connectionString = _Context.Database.GetConnectionString();

            // 定義要傳入的ViewModel
            var viewmodel = new PlayerinfoViewModel();

            // 使用 Dapper 直接呼叫預存程序(查FindinfoDto)
            using (var connection = new SqlConnection(connectionString))
            {
                // 定義參數
                var parameters = new { Login = login };

                // 呼叫 Findinfo 預存程序，並映射結果到 FindinfoDto
                var FindinfoDto_result = connection.Query<FindinfoDto>("Findinfo", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();

                // 存入資料進ViewModel
                if (FindinfoDto_result != null)
                {
                    // 把FindinfoDto的結果先存進去
                    viewmodel.Findinfo = FindinfoDto_result;
                    // 紀錄玩家名稱的cookie
                    HttpContext.Response.Cookies.Append("PlayerName", FindinfoDto_result.PlayerName);
                }
                else
                {
                    return NotFound("No information found for this account.");
                }
            }

            // 取出玩家名稱的cookie
            var playername = HttpContext.Request.Cookies["PlayerName"];

            // 使用 Dapper 直接呼叫預存程序(WinningResultDto)
            using (var connection = new SqlConnection(connectionString))
            {
                // 定義參數
                var parameters = new { Playername = playername };

                // 呼叫 WinningResult 預存程序，並映射結果到 WinningResultDto
                var WinningResultDto_result = connection.Query<WinningResultDto>("WinningResult", parameters, commandType: CommandType.StoredProcedure).ToList();

                // 存入資料進ViewModel
                viewmodel.Winningresult = WinningResultDto_result;
            }

            return View(viewmodel);

        }

        // 編輯個人資料(選單頁面)
        public IActionResult Updateinfo()
        {
            return View();
        }

        // 編輯個人資料-帳號
        public IActionResult Updateinfo_login()
        {
            // 取出玩家帳號的cookie
            ViewBag.login = HttpContext.Request.Cookies["Login"];
            return View();
        }

        // 編輯個人資料-帳號
        [HttpPost]
        public IActionResult Updateinfo_login(string login)
        {
            // 先判斷輸入帳號是否符合標準
            if (login.Length < 8 || login.Length > 20 || !login.Any(char.IsLetter) || !login.Any(char.IsDigit))
            {
                ViewBag.login = HttpContext.Request.Cookies["Login"];
                ViewBag.ErrorMessage = "帳號必須在 8 到 20 個字符之間，且至少包含一個英文字母和一個數字。";
                return View();
            }

            // 定義連線字串，通常從組態中取得
            var connectionString = _Context.Database.GetConnectionString();

            // 從cookie取出玩家名稱
            var playername = HttpContext.Request.Cookies["PlayerName"];

            // 使用 Dapper 直接呼叫預存程序
            using (var connection = new SqlConnection(connectionString))
            {
                // 設定存儲過程的參數
                var parameters = new DynamicParameters();
                parameters.Add("@PlayerName", playername, DbType.String);
                parameters.Add("@Login", login, DbType.String);
                parameters.Add("@status", dbType: DbType.Int32, direction: ParameterDirection.Output);

                // 呼叫 UpdateLogin 預存程序
                connection.Execute("UpdateLogin", parameters, commandType: CommandType.StoredProcedure);

                // 獲取返回的 status
                var status = parameters.Get<int>("@status");

                if (status == -1)
                {
                    ViewBag.login = HttpContext.Request.Cookies["Login"];
                    ViewBag.ErrorMessage = "帳號已存在";
                }
                else if (status == 1)
                {
                    HttpContext.Response.Cookies.Append("Login", login);
                    return RedirectToAction("Success", "Member");
                }

                return View();
            }
        }

        // 編輯個人資料-密碼
        public IActionResult Updateinfo_password()
        {            
            return View();
        }

        // 編輯個人資料-密碼
        [HttpPost]
        public IActionResult Updateinfo_password(string password_hint, string password)
        {
            // 先判斷輸入密碼是否符合標準
            if (password.Length < 8 || password.Length > 20 || !password.Any(char.IsLetter) || !password.Any(char.IsDigit))
            {
                ViewBag.ErrorMessage = "密碼必須在 8 到 20 個字符之間，且至少包含一個英文字母和一個數字。";
                return View();
            }

            // 定義連線字串，通常從組態中取得
            var connectionString = _Context.Database.GetConnectionString();

            // 從cookie取出玩家名稱
            var playername = HttpContext.Request.Cookies["PlayerName"];

            // 使用 Dapper 直接呼叫預存程序
            using (var connection = new SqlConnection(connectionString))
            {
                // 設定存儲過程的參數
                var parameters = new DynamicParameters();
                parameters.Add("@PlayerName", playername, DbType.String);
                parameters.Add("@Password_hint", password_hint, DbType.String);
                parameters.Add("@Password", password, DbType.String);
                parameters.Add("@status", dbType: DbType.Int32, direction: ParameterDirection.Output);

                // 呼叫 UpdatePassword 預存程序
                connection.Execute("UpdatePassword", parameters, commandType: CommandType.StoredProcedure);

                // 獲取返回的 status
                var status = parameters.Get<int>("@status");

                if (status == -1)
                {
                    ViewBag.ErrorMessage = "第二組密碼錯誤";
                }
                else if (status == -2)
                {
                    ViewBag.ErrorMessage = "密碼已存在 , 請使用別組密碼";
                }
                else if (status == 1)
                {
                    return RedirectToAction("Success", "Member");
                }

                return View();
            }
        }

        // 編輯個人資料-Email
        public IActionResult Updateinfo_Email()
        {
            return View();
        }

        // 編輯個人資料-Email
        [HttpPost]
        public IActionResult Updateinfo_Email(string mail)
        {
            // 先判斷輸入mail是否符合標準            
            try
            {
                var addr = new MailAddress(mail);
            }
            catch
            {
                ViewBag.ErrorMessage = "E-mail格式有誤";
                return View();
            }

            // 定義連線字串，通常從組態中取得
            var connectionString = _Context.Database.GetConnectionString();

            // 從cookie取出玩家名稱
            var playername = HttpContext.Request.Cookies["PlayerName"];

            // 使用 Dapper 直接呼叫預存程序
            using (var connection = new SqlConnection(connectionString))
            {
                // 設定存儲過程的參數
                var parameters = new DynamicParameters();
                parameters.Add("@Playername", playername, DbType.String);
                parameters.Add("@Mail", mail, DbType.String);

                // 呼叫 UpdatePassword 預存程序
                connection.Execute("UpdateMail", parameters, commandType: CommandType.StoredProcedure);

                return RedirectToAction("Success", "Member");
            }
        }

        // 成功頁面
        public IActionResult Success()
        {
            return View();
        }

        // 下注
        public IActionResult Betgame()
        {
            // 取出玩家名稱cookie
            ViewBag.PlayerName = HttpContext.Request.Cookies["PlayerName"];
            var Playername = HttpContext.Request.Cookies["PlayerName"];

            // 定義連線字串，通常從組態中取得
            var connectionString = _Context.Database.GetConnectionString();

            // 使用 Dapper 直接呼叫預存程序
            using (var connection = new SqlConnection(connectionString))
            {
                // 設定存儲過程的參數(輸入)
                var parameters = new DynamicParameters();
                parameters.Add("@Playername", Playername, DbType.String);
                
                // 設定存儲過程的參數(輸出)
                parameters.Add("@Wallet", dbType: DbType.Int32, direction: ParameterDirection.Output);

                // 呼叫 FindWallet 預存程序
                connection.Execute("FindWallet", parameters, commandType: CommandType.StoredProcedure);

                // 取得輸出參數的值                
                int resultCount_Wallet = parameters.Get<int>("@Wallet");

                // 餘額帶去頁面
                ViewBag.Wallet = resultCount_Wallet;

                return View();
            }
        }

        // 下注
        [HttpPost]
        public IActionResult Betgame(BetgameDto betgame)
        {
            // 驗證模型錯了就返回
            if (!ModelState.IsValid)  // 檢查模型狀態
            {
                ViewBag.PlayerName = HttpContext.Request.Cookies["PlayerName"];
                // 如果驗證失敗，返回原始視圖並顯示錯誤訊息
                return View(betgame);
            }

            // 定義連線字串，通常從組態中取得
            var connectionString = _Context.Database.GetConnectionString();

            for (int i = 1; i <= betgame.Numbers.Count / 4; i++)
            {
                // 使用 Dapper 直接呼叫預存程序
                using (var connection = new SqlConnection(connectionString))
                {
                    // 設定存儲過程的參數(輸入)
                    var parameters = new DynamicParameters();
                    parameters.Add("@Game", betgame.Game, DbType.String);
                    parameters.Add("@PlayerName", betgame.PlayerName, DbType.String);
                    parameters.Add("@Betamount", betgame.Betamount, DbType.Int32);
                    parameters.Add("@Num1", betgame.Numbers[4*i-4], DbType.Int32);
                    parameters.Add("@Num2", betgame.Numbers[4*i-3], DbType.Int32);
                    parameters.Add("@Num3", betgame.Numbers[4*i-2], DbType.Int32);
                    parameters.Add("@Num4", betgame.Numbers[4*i-1], DbType.Int32);

                    // 呼叫 Betgame 預存程序
                    connection.Execute("Betgame", parameters, commandType: CommandType.StoredProcedure);                    
                }
            }
            return RedirectToAction("Findtran", "Member");
        }

        // 查詢個人交易紀錄
        public IActionResult Findtran(int? ladder, int? page, int? today_ladder)
        {

            // 設定一頁10筆資料
            int Pagesize = 10;
            // 設定目前的頁數 , 預設第一頁
            int pageNumber = (page ?? 1);

            // 取出玩家帳號的cookie , 下面的sp是使用帳號做過濾
            var playername = HttpContext.Request.Cookies["PlayerName"];

            // 定義連線字串，通常從組態中取得
            var connectionString = _Context.Database.GetConnectionString();

            // 使用 Dapper 呼叫預存程序(Findtran)
            using (var connection = new SqlConnection(connectionString))
            {
                // 定義參數
                var parameters = new DynamicParameters();
                parameters.Add("PlayerName", playername);

                // 查今日或全部(看Findtran.cshtml)
                if (today_ladder != null)
                {
                    // 加入參數
                    parameters.Add("Today_Ladder", today_ladder);

                    // SP執行
                    var result_today_ladder = connection.Query<FindtranDto>("Findtran", parameters, commandType: CommandType.StoredProcedure).ToList();
                    return View(result_today_ladder.ToPagedList(pageNumber, Pagesize));
                }
                // 查獎期
                else if (ladder != null)
                {
                    ViewBag.ladder = ladder;

                    // 加入參數
                    parameters.Add("ladder", ladder);

                    // SP執行
                    var result_ladder = connection.Query<FindtranDto>("Findtran_ladder", parameters, commandType: CommandType.StoredProcedure).ToList();
                    return View(result_ladder.ToPagedList(pageNumber, Pagesize));
                }

                // SP執行
                var result = connection.Query<FindtranDto>("Findtran", parameters, commandType: CommandType.StoredProcedure).ToList();
                return View(result.ToPagedList(pageNumber, Pagesize));
            }
        }

        // 登出
        public async Task<IActionResult> Logout()
        {
            // 刪除cookie
            HttpContext.Response.Cookies.Delete("Login");
            HttpContext.Response.Cookies.Delete("PlayerName");

            // 驗證清除-------------------------------------------------------------------------
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // 登出用戶

            // 回首頁
            return RedirectToAction("Login", "Home");
        }
    }
}
