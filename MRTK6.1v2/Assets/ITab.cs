public interface ITab
{
    ITab Construct(DataHandler dataHandler);
    void Enable();
    void Disable();
}
