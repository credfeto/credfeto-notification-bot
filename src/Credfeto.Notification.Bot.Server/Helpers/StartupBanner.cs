using System;

namespace Credfeto.Notification.Bot.Server.Helpers;

internal static class StartupBanner
{
    public static void Show()
    {
        const string banner = @"
.88b  d88.  .d88b.  d8888b.        d8888b.  .d88b.  d888888b
88'YbdP`88 .8P  Y8. 88  `8D        88  `8D .8P  Y8. `~~88~~'
88  88  88 88    88 88   88        88oooY' 88    88    88
88  88  88 88    88 88   88 C8888D 88~~~b. 88    88    88
88  88  88 `8b  d8' 88  .8D        88   8D `8b  d8'    88
YP  YP  YP  `Y88P'  Y8888D'        Y8888P'  `Y88P'     YP";
        Console.WriteLine(banner);
        Console.WriteLine("");
        Console.WriteLine("");
        Console.WriteLine("");
        Console.WriteLine($"{typeof(Program).Namespace}: Starting...");
        Console.WriteLine("");
    }
}