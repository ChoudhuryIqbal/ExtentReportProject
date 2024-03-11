
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
using NUnit.Framework;
using ExtentReportProject.Hooks;
using NUnit.Framework.Internal;
using AventStack.ExtentReports.MarkupUtils;
using AventStack.ExtentReports.Model;
using AventStack.ExtentReports;

namespace ExtentReportProject.StepDefinitions
{
    [Binding]
    public class PDFCompareSteps : SpecFlowHooks
    {
        private string uatFilePath;
        private string prodFilePath;
        private List<string> differences;


        [Given(@"I have the UAT PDF file ""([^""]*)""")]
        public void GivenIHaveTheUATPDFFile(string uatPath)

        {
            featureName = SpecFlowHooks.extent.CreateTest("Comparing 2 PDF Files");
            uatFilePath = uatPath;
            scenarioName = featureName.CreateNode("Step: UAT File Loading").Log(AventStack.ExtentReports.Status.Info, "Uat File Loaded from" + uatPath);


          

        }

        [Given(@"I have the Prod PDF file ""([^""]*)""")]
        public void GivenIHaveTheProdPDFFile(string prodPath)
        {
            //  featureName = SpecFlowHooks.extent.CreateTest("Loading Production file from " + prodPath).Log(AventStack.ExtentReports.Status.Pass, "File Loaded");
            prodFilePath = prodPath;
            featureName.CreateNode("Step: Prod File Loading ").Log(AventStack.ExtentReports.Status.Info, "Prod File Loaded from" + prodPath);
        }

        [When(@"I compare the PDF files")]
        public void WhenICompareThePDFFiles()
        {
            differences = ComparePdfFiles(uatFilePath, prodFilePath);
            featureName.CreateNode("Comparing Files ").Log(AventStack.ExtentReports.Status.Info, "Running Comparison");

            /*

               differences = ComparePdfFiles(uatFilePath, prodFilePath);

               // Log the differences to the Extent Report
               foreach (var difference in differences)
               {
                   ExtentReportHelper.LogFail(difference);
               }
            */
        }

        [Then(@"the differences should be displayed")]
        public void ThenTheDifferencesShouldBeDisplayed()
        {

           /* string[][] data =
           {
               new string [] {"one","two"},
               new string [] { "1additional string/Table parameters can be defined on the step definition", "2additional string/Table parameters can be defined on the step definitionadditional string/Table parameters can be defined on the step definition3additional string/Table parameters can be defined on the step definitionadditional string/Table parameters can be defined on the step definitionadditional string/Table parameters can be defined on the step definition"},
               new string [] {"1","2"},
               new string [] {"1","2"},
                             new string [] { "1additional string/Table parameters can be defined on the step definition2additional string/Table parameters can be defined on the step definitionadditional string/Table parameters can be defined on the step definition", "3additional string/Table parameters can be defined on the step definitionadditional string/Table parameters can be defined on the step definitionadditional string/Table parameters can be defined on the step definition"},
                                           new string [] { "1additional string/Table parameters can be defined on the step definition", "2additional string/Table parameters can be defined on the step definitionadditional string/Table parameters can be defined on the step definition 3additional string/Table parameters can be defined on the step definitionadditional string/Table parameters can be defined on the step definitionadditional string/Table parameters can be defined on the step definition"}

            };*/


            if (differences.Count == 0)
            {
                featureName.CreateNode("No Difference Found ").Log(AventStack.ExtentReports.Status.Pass, "Files Matched");
            }
            else
            {
                var differencesData = differences.Select(diff =>
                {
                    var parts = diff.Split(": ");
                    var pageInfo = parts[0].Trim();
                    var textInfo = parts[1].Trim();

                    // Extracting line information from pageInfo
                    var lineInfo = pageInfo.Split(',')[1].Trim();

                    return new string[] { lineInfo, $"UAT - {textInfo}", $"Prod - {textInfo}" };
                }).ToArray();

                // Adding headers to the differencesData array
                differencesData = new string[][] { new string[] { "Line Info", "UAT", "Prod" } }.Concat(differencesData).ToArray();

                featureName.CreateNode("Print Differences ").Log(AventStack.ExtentReports.Status.Info, MarkupHelper.CreateTable(differencesData));
                //  featureName.CreateNode("Print Differences ").Log(AventStack.ExtentReports.Status.Info, MarkupHelper.CreateTable(data));
            }
            Assert.IsEmpty(differences, $"Differences found:{Environment.NewLine}{string.Join(Environment.NewLine, differences)}");

          //  Assert.IsEmpty(differences, $"Differences found:{Environment.NewLine}{string.Join(Environment.NewLine, differences)}");

        }
        private List<string> ComparePdfFiles(string uatPath, string prodPath)
        {
            var allDifferences = new List<string>();

            PdfDocument uatDocument = null;
            PdfDocument prodDocument = null;

            try
            {
                uatDocument = new PdfDocument(new PdfReader(uatPath));
                prodDocument = new PdfDocument(new PdfReader(prodPath));

                for (int pageIndex = 1; pageIndex <= uatDocument.GetNumberOfPages(); pageIndex++)
                {
                    var uatPage = uatDocument.GetPage(pageIndex);
                    var prodPage = prodDocument.GetPage(pageIndex);

                    var uatText = ExtractTextFromPage(uatPage);
                    var prodText = ExtractTextFromPage(prodPage);

                    if (uatText != prodText)
                    {
                        var differences = FindDifferences(uatText, prodText);
                        if (differences.Count > 0)
                        {
                            allDifferences.AddRange(differences.Select(diff =>
                                $"Page {pageIndex}, Line {diff.LineNumber}: UAT - {diff.UatText}, Prod - {diff.ProdText}"));
                        }
                    }
                }
            }
            finally
            {
                uatDocument?.Close();
                prodDocument?.Close();
            }

            return allDifferences;
        }
        private List<Difference> FindDifferences(string uatText, string prodText)
        {
            var differences = new List<Difference>();
            var uatLines = uatText.Split('\n');
            var prodLines = prodText.Split('\n');

            for (int lineNumber = 0; lineNumber < Math.Max(uatLines.Length, prodLines.Length); lineNumber++)
            {
                var uatLine = lineNumber < uatLines.Length ? uatLines[lineNumber].Trim() : string.Empty;
                var prodLine = lineNumber < prodLines.Length ? prodLines[lineNumber].Trim() : string.Empty;

                if (uatLine != prodLine)
                {
                    differences.Add(new Difference(lineNumber + 1, uatLine, prodLine));
                }
            }

            return differences;
        }
        private string ExtractTextFromPage(PdfPage page)
        {
            var textExtractionStrategy = new LocationTextExtractionStrategy();
            var processor = new PdfCanvasProcessor(textExtractionStrategy);
            processor.ProcessPageContent(page);
            return textExtractionStrategy.GetResultantText();
        }

        private class Difference
        {
            public int LineNumber { get; }
            public string UatText { get; }
            public string ProdText { get; }

            public Difference(int lineNumber, string uatText, string prodText)
            {
                LineNumber = lineNumber;
                UatText = uatText;
                ProdText = prodText;
            }
        }
    }
}

