using System.Text.RegularExpressions;

namespace Kastil.Common.Utils
{
    public static class StringExtensions
    {
        private static readonly Regex _regex = new Regex("(\\d+)");
        public static int GetTrailingNumbers(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;
            var source = value.Trim();
            var match = _regex.Match(source);
            if (match.Success)
                return int.Parse(match.Groups[0].Value);
            return 0;
        }
    }
}