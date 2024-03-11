using AventStack.ExtentReports;
using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.Reporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtentReportProject.Hooks
{
    [Binding]
   public class SpecFlowHooks
    {
        public static ExtentReports extent;
        public static ExtentTest featureName;
        public static ExtentTest scenarioName;

        [BeforeTestRun] 
        public static void BeforeTestRun()
        {
            var htmlReporter = new ExtentHtmlReporter("C:\\Users\\iqbal\\Documents\\GitHub\\ExtentReportProject\\index.html");
            htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Dark;
            extent = new ExtentReports();
            extent.AttachReporter(htmlReporter);
            Console.WriteLine("In method Before Test Run");
        }

        [AfterTestRun] 
        public static void AfterTestRun() {
          
            extent.Flush();
            Console.WriteLine("In method After Test Run");
        }
      //  [BeforeFeature]
        public static void BeforeFeature()
        {
            featureName = extent.CreateTest<Feature>(FeatureContext.Current.FeatureInfo.Title);
            Console.WriteLine("In method Before Feature");

        }

       // [BeforeScenario] 
        public static void BeforeScenario() {
         //   ScenarioContext.Current["CurrentScenario"] = featureName.CreateNode<Scenario>(ScenarioContext.Current.ScenarioInfo.Title);
            Console.WriteLine("In Method Before Scenario");
             scenarioName=featureName.CreateNode<Scenario>(ScenarioContext.Current.ScenarioInfo.Title);
            Console.WriteLine("In Method Before Scenario");
        }

      // [AfterStep] 
        public static void AfterStep()
        {
          var stepType=  ScenarioStepContext.Current.StepInfo.StepDefinitionType.ToString();
            if (ScenarioContext.Current.TestError == null)
            {
                if (stepType == "Given")
                    scenarioName.CreateNode<Given>(ScenarioStepContext.Current.StepInfo.Text);
                else if (stepType == "When")
                    scenarioName.CreateNode<When>(ScenarioStepContext.Current.StepInfo.Text);
                else if (stepType == "Then")
                    scenarioName.CreateNode<Then>(ScenarioStepContext.Current.StepInfo.Text);
                else if (stepType == "And")
                    scenarioName.CreateNode<And>(ScenarioStepContext.Current.StepInfo.Text);
            }
            else if(ScenarioContext.Current.TestError != null)
            {
                if (stepType == "Given")
                    scenarioName.CreateNode<Given>(ScenarioStepContext.Current.StepInfo.Text).Fail(ScenarioContext.Current.TestError.Message);

                else if (stepType == "When")
                    scenarioName.CreateNode<When>(ScenarioStepContext.Current.StepInfo.Text).Fail(ScenarioContext.Current.TestError.Message);
                else if (stepType == "Then")
                    scenarioName.CreateNode<Then>(ScenarioStepContext.Current.StepInfo.Text).Fail(ScenarioContext.Current.TestError.Message);
                else if (stepType == "And")
                    scenarioName.CreateNode<And>(ScenarioStepContext.Current.StepInfo.Text).Fail(ScenarioContext.Current.TestError.Message);
            }
           
            Console.WriteLine("In Method After Step Hook");
        }
        public static void LogInfoFromStepDefinition(string message)
        {
            GetScenario().Info(message);
        }

        // Existing methods...

        // Retrieve the current scenario from ScenarioContext
        private static ExtentTest GetScenario()
        {
            if (scenarioName == null)
            {
                scenarioName = featureName.CreateNode<Scenario>(ScenarioContext.Current.ScenarioInfo.Title);
            }
            return scenarioName;
          
        }

    }
}
