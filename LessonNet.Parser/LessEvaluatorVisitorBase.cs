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

		public virtual LessNode VisitBlockStatement(LessParser.BlockStatementContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitLineStatement(LessParser.LineStatementContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitAtRule(LessParser.AtRuleContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitToplevelAtRule(LessParser.ToplevelAtRuleContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitNestedAtRule(LessParser.NestedAtRuleContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitCharsetAtRule(LessParser.CharsetAtRuleContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitNamespaceAtRule(LessParser.NamespaceAtRuleContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitSupportsAtRule(LessParser.SupportsAtRuleContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitSupportsDeclaration(LessParser.SupportsDeclarationContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitSupportsCondition(LessParser.SupportsConditionContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitSupportsConditionList(LessParser.SupportsConditionListContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitDocumentAtRule(LessParser.DocumentAtRuleContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitDocumentSpecifierList(LessParser.DocumentSpecifierListContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitDocumentSpecifier(LessParser.DocumentSpecifierContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitPageAtRule(LessParser.PageAtRuleContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitKeyframesAtRule(LessParser.KeyframesAtRuleContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitKeyframesBlock(LessParser.KeyframesBlockContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitKeyframe(LessParser.KeyframeContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitGenericAtRule(LessParser.GenericAtRuleContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitVariableName(LessParser.VariableNameContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitParenthesizedExpression(LessParser.ParenthesizedExpressionContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitColor(LessParser.ColorContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitExpression(LessParser.ExpressionContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitSingleValuedExpression(LessParser.SingleValuedExpressionContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitBooleanValue(LessParser.BooleanValueContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitString(LessParser.StringContext context) {
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

		public virtual LessNode VisitGuardConditions(LessParser.GuardConditionsContext context) {
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

		public virtual LessNode VisitRulesetGuard(LessParser.RulesetGuardContext context) {
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

		public virtual LessNode VisitMixinDeclaration(LessParser.MixinDeclarationContext context) {
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

		public virtual LessNode VisitMixinGuardDefault(LessParser.MixinGuardDefaultContext context) {
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

		public virtual LessNode VisitSelectorListElement(LessParser.SelectorListElementContext context) {
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

		public virtual LessNode VisitAttribValue(LessParser.AttribValueContext context) {
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

		public LessNode VisitExtend(LessParser.ExtendContext context) {
			throw new System.NotImplementedException();
		}

		public LessNode VisitExtenderList(LessParser.ExtenderListContext context) {
			throw new System.NotImplementedException();
		}

		public LessNode VisitExtender(LessParser.ExtenderContext context) {
			throw new System.NotImplementedException();
		}

		public LessNode VisitPseudoClass(LessParser.PseudoClassContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitAttribRelate(LessParser.AttribRelateContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitKeywordAsIdentifier(LessParser.KeywordAsIdentifierContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitKeywordAsPseudoclassIdentifier(LessParser.KeywordAsPseudoclassIdentifierContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitVariableInterpolation(LessParser.VariableInterpolationContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitIdentifier(LessParser.IdentifierContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitPseudoclassIdentifier(LessParser.PseudoclassIdentifierContext context) {
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

		public virtual LessNode VisitUrl(LessParser.UrlContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitUnit(LessParser.UnitContext context) {
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitMeasurement(LessParser.MeasurementContext context)
		{
			throw new System.NotImplementedException();
		}

		public virtual LessNode VisitMediaQueryModifier(LessParser.MediaQueryModifierContext context) {
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