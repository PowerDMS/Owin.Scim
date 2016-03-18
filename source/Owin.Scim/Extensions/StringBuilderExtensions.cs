namespace Owin.Scim.Extensions
{
    using System.Text;

    public static class StringBuilderExtensions
    {
        public static bool Contains(this StringBuilder builder, char value)
        {
            for (int i = 0; i < builder.Length; i++)
            {
                if (builder[i].Equals(value)) return true;
            }

            return false;
        }
    }
}