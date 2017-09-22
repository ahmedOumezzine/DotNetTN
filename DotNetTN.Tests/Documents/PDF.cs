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
            PDF doc = new PDF();

            var data = doc.createPDF("hello");
            Console.Write(data);
        }
    }
}