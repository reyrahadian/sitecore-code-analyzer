using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RR.CodeAnalyzer.Analyzers.HelixArchitecture;
using TestHelper;

namespace RR.CodeAnalyzer.Test.Analyzers.HelixArchitecture
{
	[TestClass]
	public class FoundationLayerAnalyzerTests : DiagnosticVerifier
	{

		//No diagnostics expected to show up
		[TestMethod]
		public void EmptyFile_NoDiagnosticShouldShowUp()
		{
			var test = @"";

			VerifyCSharpDiagnostic(test);
		}

		[TestMethod]
		public void FoundationLayer_CannotDependOn_ProjectLayer()
		{
			var test = @"
    using Sitecore.Kernel;
    using Project.Demo;

    namespace Foundation.Serialization
    {
        class TypeName
        {   
        }
    }";
			var expected = new DiagnosticResult
			{
				Id = DiagnosticIds.HelixArchitecture.FoundationLayerAnalyzer,
				Message = string.Format(Resources.HelixArchitecture_FoundationLayerAnalyzer_MessageFormat, "Project"),
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", 3, 5)
						}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void FoundationLayer_CannotDependOn_FeatureLayer()
		{
			var test = @"
    using Sitecore.Kernel;
    using Feature.Banner;

    namespace Foundation.Serialization
    {
        class TypeName
        {   
        }
    }";
			var expected = new DiagnosticResult
			{
				Id = DiagnosticIds.HelixArchitecture.FoundationLayerAnalyzer,
				Message = string.Format(Resources.HelixArchitecture_FoundationLayerAnalyzer_MessageFormat, "Feature"),
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
						new DiagnosticResultLocation("Test0.cs", 3, 5)
					}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void FoundationLayer_CanDependOn_FoundationLayer()
		{
			var test = @"
    using Sitecore.Kernel;
    using Foundation.Serialization;

    namespace Foundation.SitecoreExtensions
    {
        class TypeName
        {   
        }
    }";

			VerifyCSharpDiagnostic(test);
		}		

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new FoundationLayerAnalyzer();
		}
	}
}
