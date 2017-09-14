using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTN.Tests.Connector
{
    [TestClass]
    public class Insert : Base
    {

        [TestMethod]
        public void add()
        {
            var db = GetInstance();
            var insertObj = new Student() { Name = "jack", CreateTime = Convert.ToDateTime("2010-1-1"), SchoolId = 1 };

            //Insert reutrn Insert Count
            var t2 = db.Insertable(insertObj).ExecuteCommand();

        }
    }
}
