using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTN.Documents.PDF
{
    public class PDF
    {
        public byte[] createPDF(string htmlstr)
        {
            var html = @"<?xml version=""1.0"" encoding=""UTF-8""?>
     <!DOCTYPE html
         PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN""
        ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
     <html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"" lang=""en"">
        <head>
            <title>Minimal XHTML 1.0 Document with W3C DTD</title>
        </head>
      <body>
        " + htmlstr + "</body></html>";

            // step 1: creation of a document-object
            using (Document document = new Document(PageSize.A4, 30, 30, 30, 30))
            {
                using (MemoryStream msOutput = new MemoryStream())
                {
                    // step 2:
                    // we create a writer that listens to the document
                    // and directs a XML-stream to a file
                    using (PdfWriter writer = PdfWriter.GetInstance(document, msOutput))
                    {
                        // step 3: we create a worker parse the document
                        HTMLWorker worker = new HTMLWorker(document);

                        // step 4: we open document and start the worker on the document
                        document.Open();
                        worker.StartDocument();

                        // step 5: parse the html into the document
                        worker.Parse(new StringReader(html));

                        // step 6: close the document and the worker
                        worker.EndDocument();
                        worker.Close();
                        document.Close();
                    }

                    // Return the bytes
                    return msOutput.ToArray();
                }
            }
        }
    }
}