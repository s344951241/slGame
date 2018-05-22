using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

    /// <summary>
    /// 屏幕安全区管理器。
    /// </summary>
    [ExecuteInEditMode]
    public class ScreenSafeAreaManager : MonoBehaviour
    {
        private List<IScreenSafeAreaController> m_Controllers = new List<IScreenSafeAreaController>();

        [SerializeField]
        private float m_FullScreenModeAllowance = .02f;

#pragma warning disable 0414

        [SerializeField]
        private string m_SimulateConfigName = string.Empty;

        [SerializeField]
        private ScreenOrientation m_SimulateOrientation = ScreenOrientation.LandscapeLeft;

#pragma warning restore 0414

        [SerializeField]
        private TextAsset m_DefaultConfigAsset;

        private ScreenOrientation m_CachedScreenOrientation;
        private ScreenSafeAreaOrientationConfig m_OrientationConfig = null;

        [SerializeField]
        private string m_PersistentConfigRelativePath;

        private string m_PersistentConfigPath = null;

        public string PersistentConfigPath
        {
            get
            {
                if (Application.isEditor || m_PersistentConfigPath == null)
                {
                    m_PersistentConfigPath = Path.Combine(Application.persistentDataPath, m_PersistentConfigRelativePath);
                }

                return m_PersistentConfigPath;
            }
        }

        private ScreenSafeAreaConfigs m_Configs = new ScreenSafeAreaConfigs();

        private string m_ConfigName = null;

        private string ConfigName
        {
            get
            {
                if (m_ConfigName == null)
                {
                    string willChooseConfigName = string.Empty;

                    string deviceModel = SystemInfo.deviceModel;

                    List<ModelMatchRule> modelMatchRules;
                    modelMatchRules = GetModelMatchRules();

                    for (int i = 0; i < modelMatchRules.Count; i++)
                    {
                        var rule = modelMatchRules[i];
                        if (RuleMatchesModel(deviceModel, rule))
                        {
                            willChooseConfigName = rule.ConfigName;
                            break;
                        }
                    }

#if UNITY_EDITOR
                    willChooseConfigName = m_SimulateConfigName;
#endif

                    // 屏幕实际比例和期望的比例有明显差别，放弃适配。如有些刘海屏手机可以设置让应用以全屏模式运行还是以安全区模式运行。
                    ScreenSafeAreaConfig willChooseConfig;
                    if (m_Configs.Configs.TryGetValue(willChooseConfigName, out willChooseConfig))
                    {
                        int configWidth = willChooseConfig.AspectRatio.Width;
                        int configHeight = willChooseConfig.AspectRatio.Height;
                        if (ScreenOrientation == ScreenOrientation.Portrait || ScreenOrientation == ScreenOrientation.PortraitUpsideDown)
                        {
                            configWidth = willChooseConfig.AspectRatio.Height;
                            configHeight = willChooseConfig.AspectRatio.Width;
                        }

                        if (Mathf.Abs(Screen.width * configHeight - Screen.height * configWidth) >= Mathf.RoundToInt(m_FullScreenModeAllowance * Screen.height * configWidth))
                        {
                            willChooseConfigName = string.Empty;
                            Debug.LogWarning("[ScreenSafeAreaManager get_SafeAreaConfigName] Not in full screen mode.");
                        }
                    }

                    if (willChooseConfig == null)
                    {
                        Debug.LogWarning(string.Format("[ScreenSafeAreaManager get_SafeAreaConfigName] Config for name '{0}' not found.", willChooseConfigName));
                    }

                    m_ConfigName = willChooseConfigName;
                }

                return m_ConfigName;
            }
        }

#if UNITY_EDITOR

        public IEnumerable<string> GetConfigNames()
        {
            return m_Configs.Configs.Keys;
        }

#endif

        private List<ModelMatchRule> GetModelMatchRules()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return m_Configs.ModelMatchRules["Android"];
#elif UNITY_IOS && !UNITY_EDITOR
            return m_Configs.ModelMatchRules["iOS"];
#else
            return new List<ModelMatchRule>();
#endif
        }

        private ScreenOrientation ScreenOrientation
        {
            get
            {
#if UNITY_EDITOR
                return m_SimulateOrientation;
#else
                return m_CachedScreenOrientation;
#endif
            }
        }

        public ScreenSafeAreaOrientationConfig OrientationConfig
        {
            get
            {
                if (m_OrientationConfig == null)
                {
                    m_OrientationConfig = GetOrientationConfig(ConfigName, ScreenOrientation);
                }

                return m_OrientationConfig;
            }
        }

        public ScreenSafeAreaConfig GetConfig(string configName)
        {
            ScreenSafeAreaConfig config;
            m_Configs.Configs.TryGetValue(configName, out config);
            return config;
        }

        public static ScreenSafeAreaManager Instance { get; private set; }

        public bool AddController(IScreenSafeAreaController controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }

            if (m_Controllers.Contains(controller))
            {
                return false;
            }

            m_Controllers.Add(controller);
            return true;
        }

        public bool RemoveController(IScreenSafeAreaController controller)
        {
            return m_Controllers.Remove(controller);
        }

#if UNITY_EDITOR

        public void ReloadDefaultConfigs()
        {
            if (m_DefaultConfigAsset != null)
            {
                ParseConfigs(m_DefaultConfigAsset.text);
            }
            else
            {
                m_Configs.Configs.Clear();
                m_Configs.ModelMatchRules.Clear();
            }

            PostReloadConfigs();
        }

#endif

        public void ReloadPersistentConfigs()
        {
            ParsePersistentConfigsOrThrow();
            PostReloadConfigs();
        }

        #region MonoBaheviour

        private void Awake()
        {
            if (Instance != null)
            {
                throw new System.InvalidOperationException(string.Format("There is already an instance of {0}.", GetType().Name));
            }

            Instance = this;
            m_CachedScreenOrientation = Screen.orientation;

#if UNITY_EDITOR
            if (m_SimulateOrientation == ScreenOrientation.AutoRotation || m_SimulateOrientation == ScreenOrientation.Unknown)
            {
                m_SimulateOrientation = ScreenOrientation.LandscapeLeft;
            }
#endif
            InitConfigs();
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void Update()
        {
            UpdateScreenOrientation();
        }

        #endregion MonoBaheviour

        private void InitConfigs()
        {
            if ((Application.isEditor && !Application.isPlaying) || !File.Exists(PersistentConfigPath))
            {
                if (m_DefaultConfigAsset != null)
                {
                    ParseConfigs(m_DefaultConfigAsset.text);
                }
            }
            else
            {
                ReloadPersistentConfigs();
            }
        }

        private void ParsePersistentConfigsOrThrow()
        {
            try
            {
                ParseConfigs(File.ReadAllText(PersistentConfigPath, System.Text.Encoding.UTF8));
            }
            catch (IOException)
            {
                throw;
            }
            catch (Exception)
            {
                // 无法解析配置文件，将其删除。
                if (File.Exists(PersistentConfigPath))
                {
                    File.Delete(PersistentConfigPath);
                }

                throw;
            }
        }

        private void ParseConfigs(string configText)
        {
            try
            {
                JsonMapper.RegisterImporter<double, float>(Double2Float);
                m_Configs = JsonMapper.ToObject<ScreenSafeAreaConfigs>(configText);
            }
            finally
            {
                JsonMapper.UnregisterImporters();
            }
        }

        private ScreenSafeAreaOrientationConfig GetOrientationConfig(string configName, ScreenOrientation screenOrientation)
        {
            ScreenSafeAreaConfig screenSafeAreaConfig;
            if (!m_Configs.Configs.TryGetValue(configName, out screenSafeAreaConfig))
            {
                return ScreenSafeAreaOrientationConfig.Default;
            }

            ScreenSafeAreaOrientationConfig ret = null;

            switch (screenOrientation)
            {
                case ScreenOrientation.LandscapeLeft:
                    ret = screenSafeAreaConfig.LandscapeLeft ?? screenSafeAreaConfig.LandscapeRight;
                    break;
                case ScreenOrientation.LandscapeRight:
                    ret = screenSafeAreaConfig.LandscapeRight ?? screenSafeAreaConfig.LandscapeLeft;
                    break;
                case ScreenOrientation.Portrait:
                    ret = screenSafeAreaConfig.Portrait ?? screenSafeAreaConfig.PortraitUpsideDown;
                    break;
                case ScreenOrientation.PortraitUpsideDown:
                    ret = screenSafeAreaConfig.PortraitUpsideDown ?? screenSafeAreaConfig.Portrait;
                    break;
            }

            return ret ?? ScreenSafeAreaOrientationConfig.Default;
        }

        private bool RuleMatchesModel(string deviceModel, ModelMatchRule rule)
        {
            if (string.IsNullOrEmpty(deviceModel))
            {
                return false;
            }

            switch (rule.Match)
            {
                case ModelMatchMethod.StartsWith:
                    return deviceModel.StartsWith(rule.KeyWord);
                case ModelMatchMethod.EndsWith:
                    return deviceModel.EndsWith(rule.KeyWord);
                case ModelMatchMethod.Contains:
                    return deviceModel.Contains(rule.KeyWord);
                case ModelMatchMethod.EqualTo:
                default:
                    return deviceModel == rule.KeyWord;
            }
        }

        private void PostReloadConfigs()
        {
            m_ConfigName = null;
            m_OrientationConfig = null;

            for (int i = 0; i < m_Controllers.Count; i++)
            {
                m_Controllers[i].Refresh();
            }
        }

        private void UpdateScreenOrientation()
        {
            if (m_CachedScreenOrientation == Screen.orientation)
            {
                return;
            }

            m_CachedScreenOrientation = Screen.orientation;

            m_OrientationConfig = null;

            for (int i = 0; i < m_Controllers.Count; i++)
            {
                m_Controllers[i].Refresh();
            }
        }

        private static float Double2Float(double number)
        {
            return (float)number;
        }
    }
