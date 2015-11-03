namespace Owin.Scim.Querying
{
    using System.Linq.Expressions;

    using Antlr4.Runtime.Tree;

    public interface IScimFilterVisitor
    {
        LambdaExpression VisitExpression(IParseTree tree);
    }
}