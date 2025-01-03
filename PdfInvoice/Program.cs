// See https://aka.ms/new-console-template for more information
using BitMiracle.Docotic.Pdf;
using PdfInvoice;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace PdfInvoice;
class Program
{
    static async Task Main(string[] args)
    {
        BitMiracle.Docotic.LicenseManager.AddLicenseData(@"<add license>");

        string docNum = "CV176417";
        bool withZeros = false;
        string pdfPath = @"C:\Users\Public\Downloads\CV176417.pdf";

        using var http = new HttpClient();

        var docV = await OrdService.GetDocVByDN(http, docNum);

        DocVPdf.WritePdfFile(pdfPath, docV, withZeros);


    }
}
