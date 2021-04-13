public class DataHandler
{
    public OrderItem SelectedItem { get { return selectedItem; } }
    private OrderItem selectedItem;

    public OrderItem[] AllOrderItems { get { return allOrderItems; } }
    private OrderItem[] allOrderItems;

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
