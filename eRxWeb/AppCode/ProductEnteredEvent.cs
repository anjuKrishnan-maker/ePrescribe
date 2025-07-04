
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace eRxWeb
{
public class ProductEnteredEventArgs
{
    private string _ProductNo;
    private string _LotNo;
    private string _ExprDate;
    private int _Quantity;
    private int _Scanned;

    public ProductEnteredEventArgs(string P, string L, string E, int Q, int S)
    {
        _ProductNo = P;
        _LotNo = L;
        _ExprDate = E;
        _Quantity = Q;
        _Scanned = S;
    }

    public string ProductNo { get { return _ProductNo; } set { _ProductNo = value; } }
    public string LotNo { get { return _LotNo; } set { _LotNo = value; } }
    public string ExprDate { get { return _ExprDate; } set { _ExprDate = value; } }
    public int Quantity { get { return _Quantity; } set { _Quantity = value; } }
    public int Scanned { get { return _Scanned; } set { _Scanned = value; } }
}

public delegate void ProductEnteredEventHandler(object sender, ProductEnteredEventArgs e);

}