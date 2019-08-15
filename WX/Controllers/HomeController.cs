using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace WX.Controllers
{
    public class HomeController : Controller
{
    //把APPID和APP_SECRET换成你自己的
    private const string APPID = "******";
    private const string APP_SECRET = "******";
    //为了调试方便我这里第一次把Token和Ticket获取到之后就写死了，应该写入缓存（7200s过期）
    private static string Token = "24_cQsz9scwyXLnPaAes5JlfHTfuQ2e3Iw5L8JyWfUpQiMnTk4IToOTZ7dP0Fv190ZHTy5ST--jeuDzYwoUj_hvhSHDX288YYLYVcrmvMzRPwld8ccTTzWGNTKZz53jYKDy5f8U1E886msDPsrwORGbAJABET";
    private static string Ticket = "HoagFKDcsGMVCIY2vOjf9qZA_fkPP3enjnT58qu16hzZN-3kwAP0NK6jgQM0jyAc0sK8cxaGkT9_DSgp6cHCpw";
    public ActionResult Index()
    {

        return View();
    }

    public ActionResult About()
    {
        ViewBag.Message = "Your application description page.";

        return View();
    }

    public ActionResult Contact()
    {
        ViewBag.Message = "Your contact page.";

        return View();
    }


    public async Task GetToken()
    {
        await GetTicketAsync();
    }
    //获取token和ticket
    private async Task<string> GetTicketAsync()
    {

        var tokenUrl = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={APPID}&secret={APP_SECRET}";

        var client = new System.Net.WebClient();
        client.Encoding = Encoding.UTF8;
        client.Headers.Add("Content-Type", "Application/x-www-form-urlencoded");
        var responseData = client.UploadData(tokenUrl, "POST", new byte[0]);
        var responseText = Encoding.UTF8.GetString(responseData);
        var token = JsonConvert.DeserializeAnonymousType(responseText, new { access_token = "", expires_in = "" });


        Token = token.access_token;
        var ticketUrl = $"https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={Token}&type=jsapi";
        var ticResponseData = client.UploadData(ticketUrl, "POST", new byte[0]);
        var ticResponseText = Encoding.UTF8.GetString(ticResponseData);
        var ticket = JsonConvert.DeserializeAnonymousType(ticResponseText, new { errcode = "", errmsg = "", ticket = "", expires_in = "" });
        Ticket = ticket.ticket;
        return "";
    }
    //获取签名字符串
    public async Task<string> GetSingDataAsync(string url)
    {

        var sign = new SignData();
        sign.appId = APPID;
        sign.nonceStr = Create_nonce_str();
        sign.timestamp = Create_timestamp();
        //var url = Request.Url.AbsoluteUri;
        if (url.IndexOf('#') > 0)
        {
            url = url.Substring(0, url.IndexOf('#'));
        }
        sign.url = url;
        var string1 = "jsapi_ticket=" + Ticket +
                "&noncestr=" + sign.nonceStr +
                "&timestamp=" + sign.timestamp +
                "&url=" + sign.url;

        //var string1 = GetTestSign();

        var sha1 = SHA1.Create();
        sign.signature = ByteToHex(sha1.ComputeHash(Encoding.UTF8.GetBytes(string1)));
        return JsonConvert.SerializeObject(sign);
    }
    //测试签名字符串，和微信官方提供的一样，用来测试签名方法是否正确
    private string GetTestSign()
    {
        var nonceStr = "Wm3WZYTPz0wzccnW";
        var ticket = "sM4AOVdWfPE4DxkXGEs8VMCPGGVi4C3VM0P37wVUCFvkVAy_90u5h9nbSlYy3-Sl-HhTdfl2fzFy1AOcHKP7qg";
        var timestamp = "1414587457";
        var url = "http://mp.weixin.qq.com?params=value";
        var string1 = "jsapi_ticket=" + ticket +
                "&noncestr=" + nonceStr +
                "&timestamp=" + timestamp +
                "&url=" + url;
        return string1;
    }

    /// <summary>
    /// 随机字符串
    /// </summary>
    /// <returns></returns>
    private string Create_nonce_str()
    {
        return Guid.NewGuid().ToString().Substring(0, 8);
    }
    /// <summary>
    /// 时间戳
    /// </summary>
    /// <returns></returns>
    private string Create_timestamp()
    {
        return (DateTime.Now.Ticks / 100000000).ToString();
    }
    private string ByteToHex(byte[] hash)
    {
        var sb = new StringBuilder();
        foreach (var b in hash)
        {
            sb.Append(b.ToString("x2"));
        }
        return sb.ToString();
    }
}

}