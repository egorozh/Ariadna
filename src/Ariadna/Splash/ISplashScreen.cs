namespace Ariadna
{
    public interface ISplashScreen
    {
        void AddMessage(string message);
        void LoadComplete();
        void Show();
    }
}