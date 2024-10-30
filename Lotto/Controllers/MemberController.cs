using Lotto.Dtos;
using Lotto.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using X.PagedList.Extensions;
using System.Threading;

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

        // 查詢個人資料

        public IActionResult Findinfo()
        {
            // 取出玩家帳號的cookie , 下面的sp是使用帳號做過濾
            var Login = HttpContext.Request.Cookies["Login"];

            var result =  _Context.FindinfoDto.FromSqlRaw($"EXEC Findinfo {Login}").ToList();

            // 撈出的個人資料內將玩家名稱存入cookie
            var playerInfo = result.First();
            var playerNameToStore = playerInfo.PlayerName;
            HttpContext.Response.Cookies.Append("PlayerName", playerNameToStore);

            return View(result);
        }

        // 下注
        public IActionResult Betgame()
        {
            ViewBag.PlayerName = HttpContext.Request.Cookies["PlayerName"];
            return View();
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

            // T-SQL 特別注意有輸出參數需加上out
            var sql = "exec Betgame @Game, @PlayerName, @Betamount, @Num1, @Num2, @Num3, @Num4";

            for (int i = 1; i <= betgame.Numbers.Count/4; i++)
            {
                // 建立參數
                var parameters = new[]
                {
                    new SqlParameter("@Game", betgame.Game),
                    new SqlParameter("@PlayerName", betgame.PlayerName),
                    new SqlParameter("@Betamount", betgame.Betamount),
                    new SqlParameter("@Num1", betgame.Numbers[4*i-4]), 
                    new SqlParameter("@Num2", betgame.Numbers[4*i-3]),
                    new SqlParameter("@Num3", betgame.Numbers[4*i-2]),
                    new SqlParameter("@Num4", betgame.Numbers[4*i-1])
                };


                // 執行 SQL 命令
                _Context.Database.ExecuteSqlRaw(sql, parameters);

                // 暫停半秒（100毫秒）
                Thread.Sleep(100);
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
            var Login = HttpContext.Request.Cookies["PlayerName"];

            // T-SQL 特別注意有輸出參數需加上out
            var sql = "exec Findtran @PlayerName";
            var sql_one = "exec Findtran_ladder @PlayerName, @ladder";

            // 建立參數
            var parameters = new List<SqlParameter>
            {
            new SqlParameter("@PlayerName", Login)
            };

            var parameters_one = new[]
            {
            new SqlParameter("@PlayerName", Login),
            new SqlParameter("@ladder", ladder)
            };

            // 單一獎期查詢
            if (ladder != null) // 查獎期
            {
                ViewBag.ladder = ladder;
                var result1 = _Context.FindtranDto.FromSqlRaw(sql_one, parameters_one).ToList();
                return View(result1.ToPagedList(pageNumber, Pagesize));
            }
            else if (today_ladder != null) // 查今日
            {
                parameters.Add(new SqlParameter("@Today_Ladder", today_ladder));

                sql = sql + ", @Today_Ladder";

                // 把list轉為[]
                var sqlParameters_day = parameters.ToArray();

                var result2 = _Context.FindtranDto.FromSqlRaw(sql, sqlParameters_day).ToList();
                return View(result2.ToPagedList(pageNumber, Pagesize));
            }

            // 把list轉為[]
            var sqlParameters = parameters.ToArray();
            // 執行 SQL 命令
            var result = _Context.FindtranDto.FromSqlRaw(sql, sqlParameters).ToList();
            return View(result.ToPagedList(pageNumber, Pagesize));
            
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
