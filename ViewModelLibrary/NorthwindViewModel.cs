using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using NorthwindDB;
using MvvmTools;

namespace ViewModelLibrary;

public class NorthwindViewModel : ObservableObject
{
    private NorthwindContext _db;

    public ObservableCollection<Product> Products { get; private set; } = new();

    private DateTime? _selectedDateTime;

    public DateTime? SelectedDateTime
    {
        get => _selectedDateTime;
        set
        {
            _selectedDateTime = value;
            Orders = _db.Orders.AsObservableCollection();
            NotifyPropertyChanged(nameof(SelectedDateTime));
        }
    }

    private ObservableCollection<Order> _orders;

    public ObservableCollection<Order> Orders
    {
        get => _orders;
        set
        {
            _orders = value;
            NotifyPropertyChanged(nameof(Orders));
        }
    }


    private Order _selectedOrder;

    public Order SelectedOrder
    {
        get => _selectedOrder;
        set
        {
            _selectedOrder = value;
            OrderDetails = OrderDetails = _db.OrderDetails
                .Include(x => x.Product)
                .Include(x => x.Product.Supplier)
                .Where(x => x.Order == SelectedOrder)
                .AsObservableCollection();
            Date = DateOnly.FromDateTime((DateTime) _selectedOrder.OrderDate).ToString();
            NotifyPropertyChanged(nameof(SelectedOrder));
        }
    }

    private OrderDetail _selectedOrderDetail;

    public OrderDetail SelectedOrderDetail
    {
        get => _selectedOrderDetail;
        set
        {
            _selectedOrderDetail = value;
            ProductName = _selectedOrderDetail.Product.ProductName;
            Supplier = _selectedOrderDetail.Product.Supplier.CompanyName;
            NotifyPropertyChanged(nameof(SelectedOrderDetail));
        }
    }

    private ObservableCollection<OrderDetail> _orderDetails;

    public ObservableCollection<OrderDetail> OrderDetails
    {
        get => _orderDetails;
        set
        {
            _orderDetails = value;
            NotifyPropertyChanged(nameof(OrderDetails));
        }
    }

    private string _productName;

    private string _supplier;
    
    private string? _date;

    public string ProductName
    {
        get => _productName;
        set
        {
            _productName = value;
            NotifyPropertyChanged(nameof(ProductName));
        }
    }

    public string Supplier
    {
        get => _supplier;
        set
        {
            _supplier = value;
            NotifyPropertyChanged(nameof(Supplier));
        }
    }

    public string? Date
    {
        get => _date;
        set
        {
            _date = value;
            NotifyPropertyChanged(nameof(Date));
        }
    }

    private int _quantity;

    public int Quantity
    {
        get => _quantity;
        set
        {
            _quantity = value;
            NotifyPropertyChanged(nameof(Quantity));
        }
    }
    
    private Product _selectedProduct;

    public Product SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            _selectedProduct = value;
            NotifyPropertyChanged(nameof(SelectedProduct));
        }
    }

    public NorthwindViewModel Init(NorthwindContext db)
    {
        _db = db;
        Products = _db.Products.Include(x => x.Category).AsObservableCollection();
        _selectedProduct = Products.First();
        _selectedDateTime = _db.Orders.OrderBy(x => x.OrderDate).Select(x => x.OrderDate).First();
        return this;
    }

    public ICommand AddQuantity => new RelayCommand<string>(_ => { Quantity++; });

    public ICommand SubstractQuantity => new RelayCommand<string>(_ => { Quantity--; }, x => Quantity != 0);

    public ICommand AddProduct => new RelayCommand<string>(AddOrderDetail);

    private void AddOrderDetail(string obj)
    {
        var newOrderDetail = new OrderDetail()
        {
            Product = SelectedProduct,
            ProductId = SelectedProduct.ProductId,
            Quantity = (short) Quantity,
            UnitPrice = (decimal) SelectedProduct.UnitPrice,
            Order = SelectedOrder,
            OrderId = SelectedOrder.OrderId
        };
        _db.OrderDetails.Add(newOrderDetail);
        _db.Orders.First(x => x.Equals(SelectedOrder)).OrderDetails.Add(newOrderDetail);
        _db.SaveChanges();
        SelectedOrder = _selectedOrder;
    }
}