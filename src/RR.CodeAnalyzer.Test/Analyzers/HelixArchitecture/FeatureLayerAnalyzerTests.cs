using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RR.CodeAnalyzer.Analyzers.HelixArchitecture;
using TestHelper;

namespace RR.CodeAnalyzer.Test.Analyzers.HelixArchitecture
{
	[TestClass]
	public class FeatureLayerAnalyzerTests : DiagnosticVerifier
	{

		//No diagnostics expected to show up
		[TestMethod]
		public void EmptyFile_NoDiagnosticShouldShowUp()
		{
			var test = @"";

			VerifyCSharpDiagnostic(test);
		}

		[TestMethod]
		public void FeatureLayer_CannotDependOn_ProjectLayer()
		{
			var test = @"
    using Sitecore.Kernel;
    using Project.Demo;

    namespace Feature.Account
    {
        class TypeName
        {   
        }
    }";
			var expected = new DiagnosticResult
			{
				Id = DiagnosticIds.HelixArchitecture.FeatureLayerAnalyzer,
				Message = string.Format(Resources.HelixArchitecture_FeatureLayerAnalyzer_MessageFormat, "Project"),
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", 3, 5)
						}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void FeatureLayer_CannotDependOn_FeatureLayer()
		{
			var test = @"
    using Sitecore.Kernel;
    using Feature.Banner;

    namespace Feature.Account
    {
        class TypeName
        {   
        }
    }";
			var expected = new DiagnosticResult
			{
				Id = DiagnosticIds.HelixArchitecture.FeatureLayerAnalyzer,
				Message = string.Format(Resources.HelixArchitecture_FeatureLayerAnalyzer_MessageFormat, "Feature"),
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
						new DiagnosticResultLocation("Test0.cs", 3, 5)
					}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void FeatureLayer_CanDependOn_FoundationLayer()
		{
			var test = @"
    using Sitecore.Kernel;
    using Foundation.Serialization;

    namespace Feature.Account
    {
        class TypeName
        {   
        }
    }";

			VerifyCSharpDiagnostic(test);
		}		

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new FeatureLayerAnalyzer();
		}
	}
}
