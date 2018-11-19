using Microsoft.VisualStudio.TestTools.UnitTesting;
using WpfJsd.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfJsd.Model;
using WpfJsd.Core;

namespace WpfJsd.Common.Tests
{
    [TestClass()]
    public class HttpUtilTests
    {
        [TestMethod()]
        public void LoginTest()
        {
            dynamic resp = HttpUtil.Login("jsd1", "123456");
            bool login = resp.login;
            Assert.IsTrue(login);

            HttpUtil.FetchWhList();
            HttpUtil.FetchNewTask("10005");
            HttpUtil.FetchDelayTask("10005");
            List<Warehouse> whList = SsoConfig.WhList;
            LoginUser user = SsoConfig.User;
        }
    }
}