namespace Owin.Scim.Tests.Querying.Filtering
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Antlr;

    using Antlr4.Runtime;

    using Machine.Specifications;

    using Model.Users;

    using Scim.Extensions;
    using Scim.Filtering;
    using Scim.Querying;

    public abstract class when_parsing_a_filter_expression<TResource>
    {
        Establish context = () =>
        {
            Users = new List<User>
            {
                new User { UserName = "BJensen", Active = true },
                new User { UserName = "LSmith", Active = true, Emails = new List<Email>() },
                new User { UserName = "DGioulakis", Active = false, Emails = new List<Email> { new Email { Value = "my@email.com", Primary = true, Type = "work" } } },
                new User { UserName = "PDms", Active = true },
                new User { UserName = "ROMalley", Active = true, Name = new Name { FamilyName = "O'Malley", GivenName = "Ryan" } }
            };
        };

        Because of = () =>
        {
            var lexer = new ScimFilterLexer(new AntlrInputStream(FilterExpression));
            var parser = new ScimFilterParser(new CommonTokenStream(lexer));
            var filterVisitor = new ScimFilterVisitor<TResource>();

            Predicate = filterVisitor.Visit(parser.parse()).Compile().AsFunc<User, bool>();
        };

        protected static IEnumerable<User> Users;

        protected static ScimFilter FilterExpression;

        protected static Func<User, bool> Predicate;
    }

    public class with_string_equality_comparison : when_parsing_a_filter_expression<User>
    {
        Establish context = () => FilterExpression = new ScimFilter("userName eq \"bjensen\"");

        It should_filter = () => Users.SingleOrDefault(Predicate).ShouldNotBeNull();
    }

    public class with_boolean_equality_comparison : when_parsing_a_filter_expression<User>
    {
        Establish context = () => FilterExpression = new ScimFilter("active eq \"true\"");

        It should_filter = () => Users.Where(Predicate).ShouldNotBeEmpty();
    }

    public class with_complex_property_comparison : when_parsing_a_filter_expression<User>
    {
        Establish context = () => FilterExpression = new ScimFilter("name.familyName eq \"O'Malley\"");

        It should_filter = () => Users.SingleOrDefault(Predicate).ShouldNotBeNull();
    }

    public class with_enumerable_present : when_parsing_a_filter_expression<User>
    {
        Establish context = () => FilterExpression = new ScimFilter("emails pr");

        It should_filter = () => Users.Where(Predicate).ShouldNotBeEmpty();
    }
}