using EasyHttp.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfJsd.Core;
using WpfJsd.Model;

namespace WpfJsd.Common
{
    public class HttpUtil
    {
        // 登录URL
        private static readonly string LOGIN_URL = "http://mysso.jd.id/login";
        // 新拣货任务查询接口
        private static readonly string URL_NEW_TASK = "http://out.jd.id:12345/f/out/pick/notify/hasNewTask";
        // 延迟拣货任务查询接口
        private static readonly string URL_DELAY_TASK = "http://out.jd.id:12345/f/out/pick/notify/hasDelayTask";
        // 门店列表接口
        private static readonly string WH_LIST = "http://stock.jd.id/tag/basedata/warehouseList";
        // AES加密KEY
        private static readonly string KEY = "GRE5sAmmndnu0t3h1+OzMNfrGHoVn2mdy44qISfVJqs=";
        // HttpClient
        private static HttpClient httpClient = new HttpClient();

        static HttpUtil()
        {
            httpClient.Request.Accept = HttpContentTypes.TextPlain;
            httpClient.Request.AcceptCharSet = "UTF-8";
            httpClient.Request.AddExtraHeader("app", "cs");
            httpClient.Request.AddExtraHeader("dkey", KEY);
            httpClient.Request.AddExtraHeader("mylocale", GetRemoteLang());
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static dynamic Login(string username, string password)
        {
            dynamic userParam = new ExpandoObject();
            userParam.username = username;
            userParam.password = password;
            ResponseModel model = EncryptoPost(LOGIN_URL, userParam);
            dynamic res = new ExpandoObject();
            if ("0".Equals(model.RtnStatus))
            {
                res.login = true;
                PrincipalVo vo = JsonConvert.DeserializeObject<PrincipalVo>(JsonConvert.SerializeObject(model.Data));
                App.User = vo.User;
                httpClient.Request.AddExtraHeader("appsid", vo.SessionId);
            }
            else
            {
                res.login = false;
                res.msg = model.RtnMsg;
            }
            return res;
        }

        /// <summary>
        /// 获取拣货任务
        /// </summary>
        /// <returns></returns>
        public static bool FetchNewTask(string whId)
        {
            dynamic userParam = new ExpandoObject();
            userParam.whId = whId;
            ResponseModel model = EncryptoPost(URL_NEW_TASK, userParam);
            return Convert.ToBoolean(model.Data);
        }

        /// <summary>
        /// 获取滞留拣货任务
        /// </summary>
        /// <param name="whId"></param>
        /// <returns></returns>
        public static bool FetchDelayTask(string whId)
        {
            dynamic userParam = new ExpandoObject();
            userParam.whId = whId;
            ResponseModel model = EncryptoPost(URL_DELAY_TASK, userParam);
            return Convert.ToBoolean(model.Data);
        }

        /// <summary>
        /// 查询门店列表
        /// </summary>
        /// <returns></returns>
        public static List<Warehouse> FetchWhList()
        {
            ResponseModel model = EncryptoPost(WH_LIST, new ExpandoObject());

            string whData = CryptoUtil.AESDecrypt(model.Data, KEY);
            Console.WriteLine(JSONUtil.Prettify(whData));
            model = JsonConvert.DeserializeObject<ResponseModel>(whData);
            if ("0".Equals(model.RtnStatus))
            {
                List<Warehouse> whList = JsonConvert.DeserializeObject<List<Warehouse>>(JsonConvert.SerializeObject(model.Data));
                App.WhList = whList;
                return whList;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 参数加密Post
        /// </summary>
        /// <param name="url"></param>
        /// <param name="userParam"></param>
        /// <returns></returns>
        private static ResponseModel EncryptoPost(string url, dynamic userParam)
        {
            string param = CryptoUtil.AESEncrypt(JsonConvert.SerializeObject(userParam), KEY);
            string response = Post(url, param);
            string decryptoText = CryptoUtil.AESDecrypt(response, KEY);
            Console.WriteLine(JSONUtil.Prettify(decryptoText));
            ResponseModel model = JsonConvert.DeserializeObject<ResponseModel>(decryptoText);
            return model;
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        private static string Post(string url, dynamic param)
        {
            HttpResponse response = httpClient.Post(url, param, HttpContentTypes.ApplicationJson);
            return response.RawText;
        }

        /// <summary>
        /// 获取服务端语言
        /// </summary>
        /// <returns></returns>
        private static string GetRemoteLang()
        {
            switch (LocaleUtil.GetCurrentCultureName())
            {
                case "zh-CN":
                    return EnumLang.zhCn.ToString();
                case "en-US":
                    return EnumLang.en.ToString();
                case "id-ID":
                    return EnumLang.indonesia.ToString();
                default:
                    return "";
            }
        }
    }
}
