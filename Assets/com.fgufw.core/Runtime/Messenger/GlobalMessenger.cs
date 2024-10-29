
namespace FGUFW
{
    public static class GlobalMessenger
    {
        public static IOrderedMessenger<string> M = new OrderedMessenger<string>();
    }
}