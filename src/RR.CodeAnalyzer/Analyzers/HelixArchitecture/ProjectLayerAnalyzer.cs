using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RR.CodeAnalyzer.Analyzers.HelixArchitecture
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class ProjectLayerAnalyzer : DiagnosticAnalyzer
	{
		private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
			DiagnosticIds.HelixArchitecture.ProjectLayerAnalyzer,
			Resources.HelixArchitecture_ProjectLayerAnalyzer_Title,
			Resources.HelixArchitecture_ProjectLayerAnalyzer_MessageFormat,
			CodeAnalyzerCategory.Design,
			DiagnosticSeverity.Warning,
			true,
			Resources.HelixArchitecture_ProjectLayerAnalyzer_Description,
			Resources.HelixArchitecture_ProjectLayerAnalyzer_HelpLinkUri);

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
			if (currentClassNamespace.StartsWith("project."))
			{
				if (referencedNamespace.StartsWith("project."))
					ReportDiagnostic(context, usingDirectiveSyntax, "Project");				
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