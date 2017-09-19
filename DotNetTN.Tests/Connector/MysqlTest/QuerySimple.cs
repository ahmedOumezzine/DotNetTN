using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DotNetTN.Tests.Connector.MysqlTest
{
    [TestClass]
    public class QuerySimple : DotNetTNBase
    {
        private DotNetTN.Connector.DotNetTNConnector DotNetTNConnector = new DotNetTN.Connector.DotNetTNConnector(GetInstance());

        [TestMethod]
        public void Add()
        {
            var insertObj = new Student() { Name = "jack", CreateTime = Convert.ToDateTime("2010-1-1"), SchoolId = 1 };
            DotNetTNConnector.Insert(insertObj);
        }

        [TestMethod]
        public void Update()
        {
            var updateObj = new Student() { Id = 4, Name = "demo", SchoolId = 11, CreateTime = DateTime.Now };
            DotNetTNConnector.Update(updateObj);
        }

        [TestMethod]
        public void Delete()
        {
            DotNetTNConnector.Delete(new Student() { Id = 4 });
        }

        [TestMethod]
        public void DeleteByID()
        {
            DotNetTNConnector.DeleteById<Student>(1);
        }

        [TestMethod]
        public void Find()
        {
            var student2 = DotNetTNConnector.GetById<Student>(2);
            Console.Write(student2);
        }

        [TestMethod]
        public void GetAll()
        {
            var data = DotNetTNConnector.GetList<Student>();
            Console.Write(data);
        }

        [TestMethod]
        public void GetAllWithParam()
        {
            var y = 2;
            var data2 = DotNetTNConnector.GetList<Student>(it => it.Id == y);
            Console.Write(data2);
        }
    }
}