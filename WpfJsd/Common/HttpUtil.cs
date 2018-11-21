using EasyHttp.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WpfJsd.Core;
using WpfJsd.Model;

namespace WpfJsd.Common
{
    public class HttpUtil
    {
        // 登录URL
        private static readonly string LOGIN_URL = ConfigurationManager.AppSettings["LoginUrl"];
        // 新拣货任务查询接口
        private static readonly string URL_NEW_TASK = ConfigurationManager.AppSettings["OutBaseUrl"] + "/f/out/pick/notify/hasNewTask";
        // 延迟拣货任务查询接口
        private static readonly string URL_DELAY_TASK = ConfigurationManager.AppSettings["OutBaseUrl"] + "/f/out/pick/notify/hasDelayTask";
        // 门店列表接口
        private static readonly string WH_LIST = ConfigurationManager.AppSettings["StockBaseUrl"] + "/tag/basedata/warehouseList";
        // AES加密KEY
        private static readonly string KEY = ConfigurationManager.AppSettings["AESKey"];
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
        public static ResponseModel FetchNewTask(string whId)
        {
            dynamic userParam = new ExpandoObject();
            userParam.whId = whId;
            ResponseModel model = EncryptoPost(URL_NEW_TASK, userParam);
            return model;
        }

        /// <summary>
        /// 获取滞留拣货任务
        /// </summary>
        /// <param name="whId"></param>
        /// <returns></returns>
        public static ResponseModel FetchDelayTask(string whId)
        {
            dynamic userParam = new ExpandoObject();
            userParam.whId = whId;
            ResponseModel model = EncryptoPost(URL_DELAY_TASK, userParam);
            return model;
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
            ResponseModel model = new ResponseModel
            {
                RtnStatus = "0"
            };
            try
            {
                string response = Post(url, param);
                string decryptoText = CryptoUtil.AESDecrypt(response, KEY);
                Console.WriteLine(JSONUtil.Prettify(decryptoText));
                model = JsonConvert.DeserializeObject<ResponseModel>(decryptoText);
            }
            catch (Exception e)
            {
                model.RtnStatus = "1";
                model.RtnMsg = e.Message;
            }
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
            string resp = "";
            try
            {
                HttpResponse response = httpClient.Post(url, param, HttpContentTypes.ApplicationJson);
                resp = response.RawText;
            }
            catch (WebException)
            {
                throw new WebException(LocaleUtil.GetString("ServerLost"));
            }
            catch (Exception)
            {
                throw new Exception();
            }
            return resp;
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
