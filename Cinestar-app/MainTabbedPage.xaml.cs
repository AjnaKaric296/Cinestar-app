namespace Cinestar_app;

public partial class MainTabbedPage : TabbedPage
{
    public MainTabbedPage()
    {
        InitializeComponent();
        // NavigationPage wrapperi se dodaju ovdje u code-behind
        Children[0] = new NavigationPage(Children[0] as ContentPage);
        Children[1] = new NavigationPage(Children[1] as ContentPage);
        Children[2] = new NavigationPage(Children[2] as ContentPage);
    }
}
