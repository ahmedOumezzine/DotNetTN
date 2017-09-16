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
                        var updateObj = new Student() { Id = 4, Name = "demo", SchoolId = 11, CreateTime =DateTime.Now };

            //Insert reutrn Insert Count
            var t3_1 = db.Updateable(updateObj).ExecuteCommand();

        }

        [TestMethod]
        public void Delete()
        {
            var db = GetInstance();

            var t1 = db.Deleteable<Student>().Where(new Student() { Id = 1 }).ExecuteCommand();

        }
    }
}
