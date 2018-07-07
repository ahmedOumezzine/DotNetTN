# introduction
**DotNetTN** library to (Create, Read, Update, Delete) in Mysql databases with zero configuration and some magic.

# Usage example
Let's say we have the following this class:


    public class DotNetTNBase
    {
        public static string ConnectionString = "server=localhost;Database=aotn;Uid=root;Pwd=";

        public static SqlClient GetInstance()
        {
            SqlClient db = new SqlClient(
                new Config()
                {
                    ConnectionString = ConnectionString,
                    DbType = DbType.MySql,
                    IsAutoCloseConnection = true
                }
           );

            return db;
        }
    }

To start, create entity with your choice like this exemple

 
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

# Using the library

You can work directly with the tables to insert/update/delete/select data:
in first time you shloud create private variable 

        private DotNetTN.Connector.DotNetTNConnector DotNetTNConnector = new DotNetTN.Connector.DotNetTNConnector(GetInstance());

## add 
Let's see an example of how to used the add method:


        public void Add()
        {
            var insertObj = new Student() { Name = "jack", CreateTime = Convert.ToDateTime("2010-1-1"), SchoolId = 1 };
            DotNetTNConnector.Insert(insertObj);
        }

## update  
Let's see an example of how to used the Update method:

        public void Update()
        {
            //       var updateObj = new Student() { Id = 4, Name = "demo", SchoolId = 11, CreateTime = DateTime.Now };
            var resume = DotNetTNConnector.GetById<resumeItem>(1);
            resume.Name = "bouhmid";
            DotNetTNConnector.Update(resume);
        }

## Delete

Let's see an example of how to used the Delete method:


        public void Delete()
        {
            DotNetTNConnector.Delete(new resumeItem() { ID = 4 });
        }

## DeleteByID
Let's see an example of how to used the DeleteByID method:


        public void DeleteByID()
        {
            DotNetTNConnector.DeleteById<resumeItem>(6);
        }

## Find
Let's see an example of how to used the find method:

        public void Find()
        {
            var student2 = DotNetTNConnector.GetById<resumeItem>(1);
            Console.Write(student2);
        }

## GetAll
Let's see an example of how to used the GetAll method:


        public void GetAll()
        {
            var data = DotNetTNConnector.GetList<Student>();
            Console.Write(data);
        }

## GetAllWithParam
Let's see an example of how to used the GetAllWithParam method:


        public void GetAllWithParam()
        {
            String y = "2";
            var data2 = DotNetTNConnector.GetList<Student>(it => it.Name == y);
            Console.Write(data2);
        }

## JoinLeft

Let's see an example of how to used the Join left method:

        public void JoinLeft()
        {
            var data2 = DotNetTNConnector.join<Student, School>(JoinType.Left, (st, sc) => st.SchoolId == sc.Id);
            Console.Write(data2);
        }

## JoinRight
Let's see an example of how to used the Join Right method:


        public void JoinRight()
        {
            var data2 = DotNetTNConnector.join<Student, School>(JoinType.Right, (st, sc) => st.SchoolId == sc.Id);
            Console.Write(data2);
        }

## JoinWhere
Let's see an example of how to used the JoinWhere method:


        public void JoinWhere()
        {
            var data2 = DotNetTNConnector.joinWhere1<Student, School>(JoinType.Right, (st, sc) => st.SchoolId == sc.Id, it => it.Name == "dd");
            Console.Write(data2);
        }

## JoinWhere2
Let's see an example of how to used the JoinWhere2 method:


        public void JoinWhere2()
        {
            var data2 = DotNetTNConnector.joinWhere2<Student, School>(JoinType.Right, (st, sc) => st.SchoolId == sc.Id, it => it.Name == "dd");
            Console.Write(data2);
        }
    }
}

#  other exemple 
example of project used with library DotnetTN

[Developper-des-applications-Windows-Form-avec-C-sharp](https://github.com/ahmedOumezzine/Developper-des-applications-Windows-Form-avec-C-sharp "Developper-des-applications-Windows-Form-avec-C-sharp")

[AOTN-admin-Template](https://github.com/ahmedOumezzine/AOTN-admin-Template "AOTN-admin-Template")
