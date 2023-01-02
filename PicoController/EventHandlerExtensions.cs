namespace PicoController;
public static class EventHandlerExtensions
{
    public static void SafeInvoke(this EventHandler handler, object? sender, EventArgs args)
    {
        try
        {
            handler.Invoke(sender, args);
        }
        catch (Exception ex)
        {
            Logger.Error("Unhandled exception during EventHandler.Invoke");
            Logger.Debug(ex);
        }
    }

    public static void SafeInvoke<T>(this EventHandler<T> handler, object? sender, T args)
    {
        try
        {
            handler.Invoke(sender, args);
        }
        catch (Exception ex)
        {
            Logger.Error($"Unhandled exception during {handler.GetType().Name}.Invoke".Replace("`1", $"<{typeof(T).Name}>"));
            Logger.Debug(ex);
        }
    }
}
