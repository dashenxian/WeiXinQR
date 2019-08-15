namespace WX.Controllers
{
    public class SignData
    {
        public string appId { get; set; } // 必填，公众号的唯一标识
        public string timestamp { get; set; }  // 必填，生成签名的时间戳
        public string nonceStr { get; set; }  // 必填，生成签名的随机串
        public string signature { get; set; } // 必填，签名
        public string url { get; set; }
    }
}