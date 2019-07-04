namespace com.espertech.esper.epl.parse
{
    /// <summary>
    /// For selection of the AST tree walk rule to use.
    /// Implementations can invoke a walk rule of their choice on the walker and AST passed in.
    /// </summary>
    public delegate void WalkRuleSelector(EPLTreeWalker walker);
}