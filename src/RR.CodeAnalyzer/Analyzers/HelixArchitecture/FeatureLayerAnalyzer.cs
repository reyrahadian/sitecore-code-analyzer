using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RR.CodeAnalyzer.Analyzers.HelixArchitecture
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class FeatureLayerAnalyzer : DiagnosticAnalyzer
	{
		private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
			DiagnosticIds.HelixArchitecture.FeatureLayerAnalyzer,
			Resources.HelixArchitecture_FeatureLayerAnalyzer_Title,
			Resources.HelixArchitecture_FeatureLayerAnalyzer_MessageFormat,
			CodeAnalyzerCategory.Design,
			DiagnosticSeverity.Warning,
			true,
			Resources.HelixArchitecture_FeatureLayerAnalyzer_Description,
			Resources.HelixArchitecture_DependencyDirection_HelpLinkUri);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
		{
			get { return ImmutableArray.Create(Rule); }
		}

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxNodeAction(SyntaxNodeAction, SyntaxKind.UsingDirective);
		}

		private static void SyntaxNodeAction(SyntaxNodeAnalysisContext context)
		{
			var usingDirectiveSyntax = (UsingDirectiveSyntax) context.Node;
			if (usingDirectiveSyntax == null) return;

			var referencedNamespace = usingDirectiveSyntax.Name.ToString().ToLower();
			var childNode =
				context.SemanticModel.SyntaxTree.GetRoot().ChildNodes()
					.FirstOrDefault(x => x is NamespaceDeclarationSyntax) as NamespaceDeclarationSyntax;
			if (childNode == null) return;

			var currentClassNamespace = childNode.Name.ToString().ToLower();
			if (currentClassNamespace.StartsWith("feature."))
			{
				if (referencedNamespace.StartsWith("project."))
					ReportDiagnostic(context, usingDirectiveSyntax, "Project");
				else if (referencedNamespace.StartsWith("feature."))
					ReportDiagnostic(context, usingDirectiveSyntax, "Feature");
			}
		}

		private static void ReportDiagnostic(SyntaxNodeAnalysisContext context,
			UsingDirectiveSyntax usingDirectiveSyntax, string targetLayer)
		{
			var diagnostic = Diagnostic.Create(Rule, usingDirectiveSyntax.GetLocation(), 
				targetLayer);
			context.ReportDiagnostic(diagnostic);
		}
	}
}