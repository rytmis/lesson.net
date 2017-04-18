using Antlr4.Runtime.Tree;
using LessonNet.Grammar;
using LessonNet.Parser.ParseTree;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser
{
	public abstract class LessEvaluatorVisitorBase : ILessParserVisitor<LessNode>
	{
		public virtual LessNode Visit(IParseTree tree)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitChildren(IRuleNode node)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitTerminal(ITerminalNode node)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitErrorNode(IErrorNode node)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitStylesheet(LessParser.StylesheetContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitStatement(LessParser.StatementContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitVariableName(LessParser.VariableNameContext context)
		{
			return new Variable(context.Identifier().GetText());
		}

		public virtual LessNode VisitMathCharacter(LessParser.MathCharacterContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitParenthesizedExpression(LessParser.ParenthesizedExpressionContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitExpressionList(LessParser.ExpressionListContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitColor(LessParser.ColorContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitExpression(LessParser.ExpressionContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitQuotedExpression(LessParser.QuotedExpressionContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitFunctionName(LessParser.FunctionNameContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitFunction(LessParser.FunctionContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitMixinGuardConditions(LessParser.MixinGuardConditionsContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitConditionList(LessParser.ConditionListContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitCondition(LessParser.ConditionContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitComparisonOperator(LessParser.ComparisonOperatorContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitComparison(LessParser.ComparisonContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitConditionStatement(LessParser.ConditionStatementContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitVariableDeclaration(LessParser.VariableDeclarationContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitImportDeclaration(LessParser.ImportDeclarationContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitReferenceUrl(LessParser.ReferenceUrlContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitImportMediaTypes(LessParser.ImportMediaTypesContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitRuleset(LessParser.RulesetContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitBlock(LessParser.BlockContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitMixinDefinition(LessParser.MixinDefinitionContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitMixinCallArgument(LessParser.MixinCallArgumentContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitMixinCall(LessParser.MixinCallContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitMixinGuard(LessParser.MixinGuardContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitMixinDefinitionParam(LessParser.MixinDefinitionParamContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitSelectors(LessParser.SelectorsContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitSelector(LessParser.SelectorContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitAttrib(LessParser.AttribContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitParentSelectorReference(LessParser.ParentSelectorReferenceContext context) {
			throw new System.NotImplementedException();
		}

		public LessNode VisitCombinator(LessParser.CombinatorContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitSelectorElement(LessParser.SelectorElementContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitAttribRelate(LessParser.AttribRelateContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitIdentifier(LessParser.IdentifierContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitIdentifierPart(LessParser.IdentifierPartContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitIdentifierVariableName(LessParser.IdentifierVariableNameContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitProperty(LessParser.PropertyContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitValueList(LessParser.ValueListContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitCommaSeparatedExpressionList(LessParser.CommaSeparatedExpressionListContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitMeasurementList(LessParser.MeasurementListContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitUrl(LessParser.UrlContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitFraction(LessParser.FractionContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitMeasurement(LessParser.MeasurementContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitFeatureQuery(LessParser.FeatureQueryContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitMediaQuery(LessParser.MediaQueryContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitMediaBlock(LessParser.MediaBlockContext context) {
			throw new System.NotImplementedException();
		}
	}
}