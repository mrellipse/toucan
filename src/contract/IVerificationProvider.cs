namespace Toucan.Contract
{
    public interface IVerificationProvider
    {
        void Send(IUser recipient, string code);
    }
}
