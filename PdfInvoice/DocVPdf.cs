using BitMiracle.Docotic.Pdf;
using BitMiracle.Docotic.Pdf.Layout;
using System.Net.Http;

namespace PdfInvoice
{
    public class DocVPdf
    {

        public static void WritePdfFile(string path, TrxnDocV docV, bool withZeros = false)
        {
            PdfDocumentBuilder.Create()
            .Info(i => { i.Author = "WCABR"; i.Title = docV.DocHead.DocTitle + " " + docV.DocHead.DocNum.SerStr;
                i.Subject = docV.DocHead.EntNam;
                i.Keywords = docV.DocHead.ItemV.SerLot;
            })
            .Generate(path, async doc => await BuildDocument(doc, docV, withZeros));
        }

        static async Task BuildDocument(Document doc, TrxnDocV docV, bool withZeros)
        {
            doc.Typography(t =>
            {
                t.Document = doc.TextStyleWithFont(SystemFont.Family("Segoe UI"));
            });
            doc.Pages(page =>
            {
                page.Size(PdfPaperSize.Letter);
                page.Margin(40);
                page.Header().Component(new DiffFirstPageHeader(docV));
                page.Content().Column(c =>
                {
                    BuildTitle(c.Item(), docV);
                    BuildPSLines(c.Item(), docV, withZeros);
                    BuildSummary(c.Item(), docV);
                });
                BuildPagesFooter(page.Footer());
            });
        }

        static void BuildTitle(LayoutContainer titleContainer, TrxnDocV docV)
        {
            titleContainer.Width(532).TextStyle(TextStyle.Parent.LineHeight(0.9).FontSize(11)).Table(t =>
            {
                t.Columns(c =>
                {
                    c.RelativeColumn(2);
                    c.RelativeColumn(2);
                    c.RelativeColumn(2);
                });

                t.Cell(c => c.RowIndex(0).ColumnIndex(0)).Text(c =>
                {
                    c.Line(docV.DocHead.EntCls);
                    c.Line(docV.DocHead.EntNam);
                    c.Line(docV.DocHead.EntAddr);
                    c.Line(docV.DocHead.EntCityStZip);
                    c.Line(docV.DocHead.EntPhn);
                });

                t.Cell(c => c.RowIndex(0).ColumnIndex(1).ColumnSpan(2)).AlignRight().Text(c =>
                {
                    c.Line(docV.DocHead.ItemV.Cls);
                    c.Line(docV.DocHead.ItemV.Nam);
                    c.Line(docV.DocHead.ItemV.Art);
                    c.Line(docV.DocHead.ItemV.SerLot);
                    c.Line(docV.DocHead.ItemV.Odo != 0M ? "Odometer " + docV.DocHead.ItemV.Odo.ToString("F0") : "");
                });

                t.Cell(c => c.RowIndex(1).ColumnSpan(3)).Text(c =>
                    c.Line(docV.DocHead.Memo).Style(TextStyle.Parent.LineHeight(1.5)));
            });

        }

        static void BuildPSLines(LayoutContainer psLinesContainer, TrxnDocV docV, bool withZeros)
        {
            psLinesContainer.Width(532).TextStyle(TextStyle.Parent.FontSize(9)).Table(t =>
            {
                t.Columns(c =>
                {
                    c.RelativeColumn(1); //Qty
                    c.RelativeColumn(3); //Number
                    c.RelativeColumn(5); //Part
                    c.RelativeColumn(2); //Rate
                    c.RelativeColumn(2); //Amt
                    c.RelativeColumn(4); //Service
                    c.RelativeColumn(1); //Qty
                    c.RelativeColumn(2); //Amt
                    c.RelativeColumn(2); //LineTotal
                });

                t.Header(h =>
                {
                    HeaderCell(h).AlignCenter().Text("Qty");
                    HeaderCell(h).AlignLeft().PaddingLeft(4).Text("Number");
                    HeaderCell(h).AlignLeft().Text("Part");
                    HeaderCell(h).AlignRight().Text("Rate");
                    HeaderCell(h, true).AlignRight().PaddingRight(4).Text("Amt");
                    HeaderCell(h).Text("Service");
                    HeaderCell(h).AlignRight().Text("Qty");
                    HeaderCell(h, true).AlignRight().PaddingRight(4).Text("Amt");
                    HeaderCell(h).AlignRight().PaddingRight(4).Text("LineTotal");
                });

                int ct = 0;
                for (int i = 0; i < docV.PsLines.Count; ++i)
                {
                    DocLine p = docV.PsLines[i];
                    if (p.LineTotal == 0M && withZeros == false)
                    {
                        continue;
                    }
                    bool alt = ct % 2 == 0;
                    BodyCell(t, alt).AlignCenter().Text(p.PrtQty != 0M ? p.PrtQty.ToString("F0") : "");
                    BodyCell(t, alt).AlignLeft().PaddingLeft(4).Text(p.PrtNum != null ? p.PrtNum : "");
                    BodyCell(t, alt).AlignLeft().Text(p.PrtNam != null ? p.PrtNam : "");
                    BodyCell(t, alt).AlignRight().Text(p.PrtRate != 0M ? p.PrtRate.ToString("F2") : "");
                    BodyCell(t, alt, true).AlignRight().PaddingRight(4).Text(p.PrtAmt != 0M ? p.PrtAmt.ToString("C") : "");
                    BodyCell(t, alt).Text(p.SrvNam != null ? p.SrvNam : "");
                    BodyCell(t, alt).AlignRight().Text(p.SrvQty != 0M ? p.SrvQty.ToString("F1") : "");
                    BodyCell(t, alt, true).AlignRight().PaddingRight(4).Text(p.SrvAmt != 0M ? p.SrvAmt.ToString("C") : "");
                    BodyCell(t, alt).AlignRight().PaddingRight(4).Text(p.LineTotal != 0M ? p.LineTotal.ToString("C") : "");
                    ct++;
                }

            });
        }

        static void BuildSummary(LayoutContainer sumContainer, TrxnDocV docV)
        {
            sumContainer.ShowEntire().Width(532).ExtendVertical().AlignBottom().TextStyle(TextStyle.Parent.FontSize(9)).Row(r =>
            {
                r.AutoItem().Width(332).AlignMiddle().AlignLeft().PaddingRight(20).Column(c =>
                {
                    c.Item().Text(docV.Message);
                    c.Item().AlignCenter().PaddingTop(4).Text(docV.Sig);
                });
                r.AutoItem().Width(200).AlignRight().Table(t =>
                {
                    t.Columns(c =>
                    {
                        c.RelativeColumn(4);
                        c.RelativeColumn(2);
                        c.RelativeColumn(2);
                    });

                    t.Header(h =>
                    {
                        HeaderCell(h);
                        HeaderCell(h).AlignLeft().Text("Totals");
                        HeaderCell(h);
                    });

                    for (int i = 0; i < docV.AgLines.Count; ++i)
                    {
                        bool alt = i % 2 == 0;
                        Agline a = docV.AgLines[i];
                        BodyCell(t, alt).AlignLeft().PaddingLeft(4).Text(a.Name);
                        BodyCell(t, alt).AlignLeft().Text(String.Format("@ {0:F2}", a.Rate));
                        BodyCell(t, alt).AlignRight().PaddingRight(4).Text(a.Amt.ToString("C"));
                    }
                    t.Cell(c => c.ColumnSpan(2)).Border(c => c.Top(0.5).Color(new PdfGrayColor(80)))
                        .AlignLeft().PaddingLeft(4).Text(String.Format("{0} Total", docV.DocHead.DocTitle));
                    t.Cell().Border(c => c.Top(0.5).Color(new PdfGrayColor(80)))
                    .AlignRight().PaddingRight(4).Text(docV.DocSum.Total.ToString("C"));
                    t.Cell(c => c.ColumnSpan(2)).Border(b => b.Bottom(0.5).Color(new PdfGrayColor(80)))
                        .AlignLeft().PaddingLeft(4).Text("Payments");
                    t.Cell().Border(b => b.Bottom(0.5).Color(new PdfGrayColor(80)))
                    .AlignRight().PaddingRight(4).Text(docV.DocSum.ZSum.ToString("C"));
                    t.Cell(c => c.ColumnSpan(2))
                        .AlignLeft().PaddingLeft(4).Text(docV.BalLbl);
                    t.Cell()
                    .AlignRight().PaddingRight(4).Text(docV.DocSum.Balance.ToString("C"));
                });
            }
            );
        }

        static LayoutContainer HeaderCell(TableCellContainer t, bool borderRight = false)
        {
            return t.Cell() //.TextStyle(s => s.Strong)
                .Border(b => b.Top(0.5).Color(new PdfGrayColor(80)))
                .Border(b => b.Bottom(0.5).Color(new PdfGrayColor(80)))
                .Container(c => borderRight ? c.Border(b => b.Right(0.5).Color(new PdfGrayColor(80))) : c)
                .AlignCenter()
                .AlignMiddle();
        }


        static LayoutContainer BodyCell(Table t, bool alt, bool borderRight = false, int rowSpan = 1)
        {
            return t.Cell(c => c.RowSpan(rowSpan))
                .Container(c => alt ? c.Background(new PdfGrayColor(96)) : c)
                .Container(c => borderRight ? c.Border(b => b.Right(0.5).Color(new PdfGrayColor(80))) : c)
                .AlignCenter()
                .AlignMiddle();
        }

        static void BuildPagesFooter(LayoutContainer footerContainer)
        {
            footerContainer.Height(20)
                .AlignCenter()
                .Text(t =>
                {
                    t.Span("Page ");
                    t.CurrentPageNumber();
                    t.Span(" of ");
                    t.PageCount();
                });
        }



    }

    public class DiffFirstPageHeader : ILayoutComponent
    {
        TrxnDocV _docV;
        public DiffFirstPageHeader(TrxnDocV docV)
        {
            _docV = docV;
        }
        public LayoutComponentContent Compose(LayoutContext context)
        {
            var content = context.CreateElement(container =>
            {
                if (_docV is null)
                    return;

                if (context.PageNumber == 1)
                {
                    container.Width(532).TextStyle(TextStyle.Parent.LineHeight(0.9).FontSize(11)).Table(t =>
                    {
                        t.Columns(c =>
                        {
                            c.RelativeColumn(2);
                            c.RelativeColumn(3);
                            c.RelativeColumn(2);
                        });
                        //ToDo: Logo

                        t.Cell(c => c.RowIndex(0).ColumnIndex(1)).AlignCenter().Text(c =>
                        {
                            c.Line(_docV.DocHead.CoNam).Style(TextStyle.Parent.FontSize(14));
                            c.Line(_docV.DocHead.CoAddr);
                        });

                        t.Cell(c => c.RowIndex(0).ColumnIndex(2)).AlignRight().Column(c =>
                        {
                            c.Item().AlignRight().Row(r =>
                            {
                                r.AutoItem().Text(_docV.DocHead.DocTitle + " " + _docV.DocHead.DocNum.SerStr).Style(TextStyle.Parent.FontSize(14));
                            });
                            c.Item().AlignRight().Row(r =>
                            {
                                //r.Spacing(10);
                                r.ConstantItem(20).Text("In");
                                r.ConstantItem(70).AlignRight().Text(_docV.DocHead.DateIn.ToString("yyyy MMM dd"));
                            });
                            c.Item().AlignRight().Row(r =>
                            {
                                //r.Spacing(10);
                                r.ConstantItem(20).Text("Out");
                                r.ConstantItem(70).AlignRight().Text(_docV.DocHead.Date.ToString("yyyy MMM dd"));
                            });

                        });

                    });

                }


                if (context.PageNumber > 1)
                {
                    container.PaddingBottom(6).Row(r =>
                    {
                        r.RelativeItem().AlignBottom().AlignLeft()
                         .Text(_docV.DocHead.EntNam);

                        r.RelativeItem().AlignBottom().AlignCenter()
                        .Text(_docV.DocHead.CoNam);

                        r.RelativeItem().AlignBottom().AlignRight()
                        .Text(_docV.DocHead.DocTitle + " " + _docV.DocHead.DocNum.SerStr);

                    });

                }

            });

            return new LayoutComponentContent(content, false);
        }

        public void Reset()
        {
            
        }
    }


}
