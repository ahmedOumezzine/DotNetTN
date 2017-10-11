using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotNetTN.Documents.PDF;

namespace DotNetTN.Tests.Documents
{
    [TestClass]
    public class PDFTest
    {
        [TestMethod]
        public void docpdf()
        {
            ConvertPDF doc = new ConvertPDF();

            var data = doc.GetBytes("<table style='width: 100 % '> <tr> <th>Firstname</th> <th>Lastname</th> <th>Age</th> </tr> <tr> <td>Jill</td> <td>Smith</td> <td>50</td> </tr> <tr> <td>Eve</td> <td>Jackson</td> <td>94</td> </tr> </table>");
            Console.Write(data);
            var data2 = doc.GetString(data);
            Console.Write(data2);

            doc.SavePDF("C:\\Users\\ahmed\\Desktop\\doc\\test2.pdf", data);
        }
    }
}