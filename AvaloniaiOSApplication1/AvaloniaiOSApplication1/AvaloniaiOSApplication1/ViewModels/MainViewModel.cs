namespace AvaloniaiOSApplication1.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private string _greeting = "Welcome to ARKit in Avalonia!";
    public string Greeting
    {
        get => _greeting;
        set { RaiseAndSetIfChanged(ref _greeting, value); }
    }
   
}