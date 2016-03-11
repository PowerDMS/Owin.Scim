namespace Owin.Scim.Extensions
{
    using System.Text;

    public static class StringBuilderExtensions
    {
        public static bool StartsWith(this StringBuilder builder, string text)
        {
            if (builder.Length < text.Length) return false;

            for (int i = 0; i < text.Length; i++)
            {
                if (!builder[i].Equals(text[i])) return false;
            }

            return true;
        }
    }
}