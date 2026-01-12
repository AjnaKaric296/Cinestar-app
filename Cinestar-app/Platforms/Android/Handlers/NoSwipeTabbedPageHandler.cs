using AndroidX.ViewPager2.Widget;
using Microsoft.Maui.Handlers;

namespace Cinestar_app.Platforms.Android.Handlers
{
    public class NoSwipeTabbedPageHandler : TabbedViewHandler
    {
        public NoSwipeTabbedPageHandler()
        {
            // Dodaj mapper za OnPlatformViewCreated
            Mapper.AppendToMapping("NoSwipe", (handler, view) =>
            {
                if (handler.PlatformView is ViewPager2 pager)
                {
                    pager.UserInputEnabled = false; // isključi swipe
                }
            });
        }
    }
}
