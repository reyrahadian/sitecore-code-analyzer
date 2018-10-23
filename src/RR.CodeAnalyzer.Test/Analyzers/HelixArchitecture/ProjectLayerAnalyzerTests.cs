using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RR.CodeAnalyzer.Analyzers.HelixArchitecture;
using TestHelper;

namespace RR.CodeAnalyzer.Test.Analyzers.HelixArchitecture
{
	[TestClass]
	public class ProjectLayerAnalyzerTests : CodeFixVerifier
	{

		//No diagnostics expected to show up
		[TestMethod]
		public void EmptyFile_NoDiagnosticShouldShowUp()
		{
			var test = @"";

			VerifyCSharpDiagnostic(test);
		}

		[TestMethod]
		public void ProjectLayer_CannotDependOn_ProjectLayer()
		{
			var test = @"
    using Sitecore.Kernel;
    using Project.Habitat;

    namespace Project.Demo
    {
        class TypeName
        {   
        }
    }";
			var expected = new DiagnosticResult
			{
				Id = DiagnosticIds.HelixArchitecture.ProjectLayerAnalyzer,
				Message = string.Format(Resources.HelixArchitecture_ProjectLayerAnalyzer_MessageFormat, "Project"),
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", 3, 5)
						}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void ProjectLayer_CanDependOn_FeatureLayer()
		{
			var test = @"
    using Sitecore.Kernel;
    using Feature.Account;

    namespace Project.Demo
    {
        class TypeName
        {   
        }
    }";

			VerifyCSharpDiagnostic(test);
		}

		[TestMethod]
		public void ProjectLayer_CanDependOn_FoundationLayer()
		{
			var test = @"
    using Sitecore.Kernel;
    using Foundation.SitecoreExtensions;

    namespace Project.Demo
    {
        class TypeName
        {   
        }
    }";

			VerifyCSharpDiagnostic(test);
		}

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new ProjectLayerAnalyzer();
		}
	}
}
