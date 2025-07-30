
public static class PriorityLayers
{
    public static int FIRST = -10000;
    public static int NORMAL = 0;
    public static int LAST = 10000;
    public static int LAST_SPECIAL = 10001;
}
public abstract class BaseInteraction
{
    public virtual int Priority()
    {
        return PriorityLayers.NORMAL;
    }
}