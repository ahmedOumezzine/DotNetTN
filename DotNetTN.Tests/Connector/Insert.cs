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
        [TestMethod]
        public void update()
        {
            var db = GetInstance();
            var updateObj = new Student() { Id = 1, Name = "demo", SchoolId = 0, CreateTime = Convert.ToDateTime("2017-05-21 09:56:12.610") };

            //Insert reutrn Insert Count
            var t2 = db.Updateable(updateObj).ExecuteCommand();

        }

        [TestMethod]
        public void Delete()
        {
            var db = GetInstance();
            var DeletObj = new Student() { Id = 1, Name = "demo", SchoolId = 0, CreateTime = Convert.ToDateTime("2017-05-21 09:56:12.610") };

            var t1 = db.Deleteable(DeletObj).Where(new Student() { Id = 1 }).ExecuteCommand();

        }
    }
}
