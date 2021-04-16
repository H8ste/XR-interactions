public interface ITab
{
    ITab Instantiate(DataHandler dataHandler);
    void Enable();
    void Disable();
}
