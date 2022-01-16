namespace NorthwindDB;

public partial class OrderDetail
{
    public override string ToString()
    {
        return $"Order {OrderId} {Quantity} x Product {ProductId}";
    }
}