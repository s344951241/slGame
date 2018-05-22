using System.Collections.Generic;


    [System.Serializable]
    public class ScreenSafeAreaConfigs
    {
        public int Version;

        public Dictionary<string, List<ModelMatchRule>> ModelMatchRules = new Dictionary<string, List<ModelMatchRule>>();

        public Dictionary<string, ScreenSafeAreaConfig> Configs = new Dictionary<string, ScreenSafeAreaConfig>();
    }
