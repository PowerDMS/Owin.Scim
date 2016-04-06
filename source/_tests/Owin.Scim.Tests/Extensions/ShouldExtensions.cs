namespace Owin.Scim.Tests
{
    using System;

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

        public static IComparable ShouldBeGreaterThan(this DateTime arg1, DateTime arg2)
        {
            if (arg2 == null)
                throw new ArgumentNullException("arg2");
            if (arg1 == null)
                throw new SpecificationException(string.Format("Should be greater than {0} but is [null]", arg2));
            if (DateTime.Compare(arg1, arg2) <= 0)
                throw new SpecificationException(string.Format("Should be greater than {0} but is {1}", arg2, arg1));
            return arg1;
        }
    }
}