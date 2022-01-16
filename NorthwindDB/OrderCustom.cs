namespace NorthwindDB;

public partial class Order
{
    public override string ToString()
    {
        return $"Order {OrderId} from {OrderDate}";
    }
}