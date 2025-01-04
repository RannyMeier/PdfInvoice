using System;
using System.Collections.Generic;
using System.Text;

namespace PdfInvoice
{


    public class TrxnDocV
    {
        public string TtSym { get; set; }
        public DateTime Date { get; set; }
        public DocHead DocHead { get; set; }
        public DocSum DocSum { get; set; }
        public string ZSumLbl { get; set; }
        public string BalLbl { get; set; }
        public List<DocLine> PsLines { get; set; }
        public List<Agline> AgLines { get; set; }
        public LinkLine QuLines { get; set; }
        public LinkLine ZiLines { get; set; }
        public string Message { get; set; }
        public string SigLine { get; set; }
        public string Sig { get; set; }
        public string Note { get; set; }
        public string Label { get; set; }
        public string FootMsg { get; set; }
        public int PgCount { get; set; }
        public DateTime DatePrnt { get; set; }
        //public bool HasErrors { get; set; }
    }

    public class Docnum
    {
        public int PadLen { get; set; }
        public string Str { get; set; }
        public string Num { get; set; }
        public string Typ { get; set; }
        public int CsN { get; set; }
        public int Ser { get; set; }
        public DateTime Date { get; set; }
        public string SerStr { get; set; }
        public int Dash { get; set; }
        public string Ver { get; set; }
        public string NumDash { get; set; }
        //public bool HasErrors { get; set; }
    }
    
    public class DocHead
    {
        public Docnum DocNum { get; set; }
        public string DocTitle { get; set; }
        public string CoNam { get; set; }
        public string CoAddr { get; set; }
        public string RefNum { get; set; }
        public string Memo { get; set; }
        public DateTime DateIn { get; set; }
        public DateTime Date { get; set; }
        public string EntCls { get; set; }
        public string EntNum { get; set; }
        public string EntPhn { get; set; }
        public string EntNam { get; set; }
        public string EntAddr { get; set; }
        public string EntCityStZip { get; set; }
        public ItemV ItemV { get; set; }
        //public bool HasErrors { get; set; }
    }

    public class DocLine
    {
        public short Line { get; set; }
        public string PrtAcct { get; set; }
        public string PrtNum { get; set; }
        public string PrtNam { get; set; }
        public decimal PrtQty { get; set; }
        public object QLns { get; set; }
        public object ZLns { get; set; }
        public decimal PrtRate { get; set; }
        public decimal PrtAmt { get; set; }
        public string PrtMemo { get; set; }
        public string SrvAcct { get; set; }
        public string SrvNum { get; set; }
        public string SrvNam { get; set; }
        public decimal SrvQty { get; set; }
        public decimal SrvAmt { get; set; }
        public string SrvMemo { get; set; }
        public decimal LineTotal { get; set; }
        //public bool HasErrors { get; set; }
    }
    
    public class ItemV
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public string Cls { get; set; }
        public string Tag { get; set; }
        public string TagSt { get; set; }
        public string SerLot { get; set; }
        public string Sku { get; set; }
        public string Serie { get; set; }
        public string Num { get; set; }
        public string Nam { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Typ { get; set; }
        public string Art { get; set; }
        public decimal Odo { get; set; }
        public bool HasErrors { get; set; }
    }

    public class DocSum
    {
        public decimal Dv1Prt { get; set; }
        public decimal Dv2Srv { get; set; }
        public decimal AgSrv { get; set; }
        public decimal AgPrt { get; set; }
        public decimal Dv3Oth { get; set; }
        public decimal Total { get; set; }
        public decimal ZSum { get; set; }
        public decimal QSum { get; set; }
        public decimal Balance { get; set; }
        public bool HasErrors { get; set; }
    }

    public class LinkLine
    {
        //Account Acct { get; set; }
        string AcctNam { get; set; }
        decimal Amt { get; set; }
        string AmtStr { get; set; }
        string DocNum { get; set; }
        decimal Qty { get; set; }
        string QtyStr { get; set; }
        DateTime TrnDate { get; set; }
        //TrxnType TrnType { get; set; }
        string TrnTypeStr { get; set; }

    }
    
    public class Agline
    {
        public string Name { get; set; }
        public float Qty { get; set; }
        public float Rate { get; set; }
        public float Amt { get; set; }
        public bool HasErrors { get; set; }
    }



}
