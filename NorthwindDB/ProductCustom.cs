namespace NorthwindDB;

public partial class Product
{
    public override string ToString()
    {
        return $"{ProductName}: {String.Format("{0:0.00}", UnitPrice)} € ({Category?.CategoryName})";
    }
}