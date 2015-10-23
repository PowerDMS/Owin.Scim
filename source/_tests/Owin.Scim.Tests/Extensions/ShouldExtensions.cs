namespace Owin.Scim.Tests.Extensions
{
    using Machine.Specifications;

    public static class ShouldExtensions
    {
        public static bool ShouldBeLowercase(this string value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (char.IsUpper(value[i]))
                {
                    throw new SpecificationException("Should be lowercase but contains uppercase characters.");
                }
            }

            return true;
        }
    }
}