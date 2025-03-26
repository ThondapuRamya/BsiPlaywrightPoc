namespace BsiPlaywrightPoc.Hooks
{
    public class HookOrdering
    {
        public const int BeforeAndAfterSetting = 1;
        public const int BetweenFeatureAndScenarioSetting = 2;
        public const int ScenarioHooksSetting = 5;
        public const int ApiSetting = 10;
        public const int WebSetting = 15;
        public const int ReportSetting = 20;
        public const int User = 25;
        public const int ScreenShot = 30;
        public const int AfterTestRun = 31;
        public const int Maximum = int.MaxValue;
    }
}
