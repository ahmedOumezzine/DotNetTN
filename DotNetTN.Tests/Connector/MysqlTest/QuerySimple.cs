using DotNetTN.Connector.SQL.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DotNetTN.Tests.Connector.MysqlTest
{
    [Table("resume")]
    public class resumeItem
    {
        [Column(IsPrimaryKey = true, IsIdentity = true, ColumnName = "resumeID")]
        public int ID { get; set; }

        [Column(ColumnName = "fullName")]
        public string Name { get; set; }

        [Column(ColumnName = "fullPrenom")]
        public string Prenom { get; set; }

        [Column(ColumnName = "DatedeNaissance")]
        public DateTime? Date { get; set; }

        [Column(ColumnName = "Originaire")]
        public string Originaire { get; set; }

        [Column(ColumnName = "EtatCivil")]
        public string EtatCivil { get; set; }
    }

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
            //       var updateObj = new Student() { Id = 4, Name = "demo", SchoolId = 11, CreateTime = DateTime.Now };
            var resume = DotNetTNConnector.GetById<resumeItem>(1);
            resume.Name = "bouhmid";
            DotNetTNConnector.Update(resume);
        }

        [TestMethod]
        public void Delete()
        {
            DotNetTNConnector.Delete(new resumeItem() { ID = 4 });
        }

        [TestMethod]
        public void DeleteByID()
        {
            DotNetTNConnector.DeleteById<resumeItem>(6);
        }

        [TestMethod]
        public void Find()
        {
            var student2 = DotNetTNConnector.GetById<resumeItem>(1);
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
            String y = "2";
            var data2 = DotNetTNConnector.GetList<Student>(it => it.Name == y);
            Console.Write(data2);
        }
    }
}