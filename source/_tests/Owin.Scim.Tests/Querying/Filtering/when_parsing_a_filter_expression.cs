namespace Owin.Scim.Tests.Querying.Filtering
{
    using System;
    using System.Collections.Generic;

    using Antlr;

    using Antlr4.Runtime;

    using Machine.Specifications;

    using Model.Users;

    using Scim.Extensions;
    using Scim.Querying;

    public class when_parsing_a_filter_expression<TResource>
    {
        Because of = () =>
        {
            var lexer = new ScimFilterLexer(
                new AntlrInputStream(
                    new ScimFilter(ResourceExtensionSchemas, FilterExpression)));
            var parser = new ScimFilterParser(new CommonTokenStream(lexer));
            var filterVisitor = new ScimFilterVisitor<TResource>();

            Predicate = filterVisitor.Visit(parser.parse()).Compile().AsFunc<User, bool>();
        };

        protected static IEnumerable<User> Users;

        protected static string FilterExpression;

        protected static Func<User, bool> Predicate;

        protected static ISet<string> ResourceExtensionSchemas = new HashSet<string>();
    }
}