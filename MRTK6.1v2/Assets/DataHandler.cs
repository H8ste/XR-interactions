public class DataHandler
{
    public OrderItem SelectedItem { get { return allOrderItems?.Length > selectedItemIndex ? allOrderItems[selectedItemIndex]: null; } }
    public int SelectedItemIndex { get { return selectedItemIndex; } }
    private int selectedItemIndex;

    public OrderItem[] AllOrderItems { get { return allOrderItems; } }
    private OrderItem[] allOrderItems;


    public OrderItem SelectedTestItem { get { return selectedTestItem; } }
    private OrderItem selectedTestItem = new OrderItem(null, new LocPK(40, 10, 10, 10, 10, "X"), 3, 0, 1, "Ost", 254, false);
    public OrderItem[] AllTestItems { get { return allTestItems; } }
    private OrderItem[] allTestItems = {
            new OrderItem(null, new LocPK(40, 10, 10, 10, 10, "X"), 0, 0, 1, "Frikadeller", 254, false),
            new OrderItem(null, new LocPK(50, 20, 12, 10, 10, "L"), 1, 0, 9, "Pizza", 251, true),
            new OrderItem(null, new LocPK(60, 20, 13, 11, 10, "L"), 2, 0, 1, "Smør", 24, false),
            new OrderItem(null, new LocPK(40, 10, 10, 10, 10, "X"), 3, 0, 1, "Ost", 254, false),
            new OrderItem(null, new LocPK(40, 10, 10, 10, 10, "X"), 4, 0, 1, "Løg", 254, false),
            new OrderItem(null, new LocPK(40, 10, 10, 10, 10, "X"), 5, 0, 1, "Oliven", 254, false),
            new OrderItem(null, new LocPK(40, 10, 10, 10, 10, "X"), 6, 0, 1, "Eddike", 254, false),
            new OrderItem(null, new LocPK(40, 10, 10, 10, 10, "X"), 7, 0, 1, "Pølser", 254, false),
            new OrderItem(null, new LocPK(40, 10, 10, 10, 10, "X"), 8, 0, 1, "Øl", 254, false),
            new OrderItem(null, new LocPK(40, 10, 10, 10, 10, "X"), 9, 0, 1, "Ansjoser", 254, false),
            new OrderItem(null, new LocPK(40, 10, 10, 10, 10, "X"), 10, 0, 1, "Tærter", 254, false),
            new OrderItem(null, new LocPK(40, 10, 10, 10, 10, "X"), 11, 0, 1, "Chips", 254, false),
            };
//ApiHandler apiHandler;


public DataHandler(/*ApiHandler apiHandler*/)
    {
        //this.apiHandler = apiHandler;
    }

    public void PickItem(OrderItem item)
    {
        // though apiHandler, call PickItem() - that through the api, calls the appropriate biz method 

        // refetch picked item
    }

    public void FetchRoundInfo()
    {
        // through apiHandler, get appropriate order items and set them to allOrderItems and set selectedItem


        //allOrderItems = 

        //selectedItem = allOrderItems[0];
    }
}
