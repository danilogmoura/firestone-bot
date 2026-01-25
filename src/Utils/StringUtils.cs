using System.Linq;

namespace Firebot.Utils
{
    public abstract class StringUtils
    {
        public static string JoinPath(string parent, params string[] children)
        {
            var result = parent?.TrimEnd('/') ?? "";

            return children.Where(child => !string.IsNullOrEmpty(child)).Aggregate(result,
                (current, child) => $"{current}/{child.TrimStart('/').TrimEnd('/')}");
        }
    }
}