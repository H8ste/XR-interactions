using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocPK : ScriptableObject
{
    private short stockNo;
    public short StockNo { get { return stockNo; } set { stockNo = value; } }

    private short aisleNo;
    public short AisleNo { get { return aisleNo; } set { aisleNo = value; } }

    private short x;
    public short X { get { return x; } set { x = value; } }

    private short y;
    public short Y { get { return y; } set { y = value; } }

    private short z;
    public short Z { get { return z; } set { z = value; } }

    private string side;
    public string Side { get { return side; } set { side = value; } }

    public LocPK(short stockNo, short aisleNo, short x, short y, short z, string side)
    {
        StockNo = stockNo;
        AisleNo = aisleNo;
        X = x;
        Y = y;
        Z = z;
        Side = side;
    }

    public string GetLocPKAsString()
    {
        return $"{StockNo}-{AisleNo}-{X}-{Y}-{Z}-{Side}";
    }

}
