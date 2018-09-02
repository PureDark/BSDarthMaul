using IllusionPlugin;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BSDarthMaul
{
	public class Plugin : IEnhancedPlugin, IPlugin
	{
        public static string PluginName = "Darth Maul Plugin";
        public const string VersionNum = "0.3.0";

        public string Name => PluginName;
        public string Version => VersionNum;

        private static Plugin _instance;
        
        private static PanelBehavior panelBehavior;
        private static DarthMaulBehavior darthMaulBehavior;

        private static AsyncScenesLoader loader;

        public const string KeyDarthMode = "DMDarthMode";
        public const string KeyOneHanded = "DMOneHanded";
        public const string KeySeparation = "DMSerparation";
        public const string KeyAutoDetect = "DMAutoDetect";

        public static bool IsDarthModeOn
        {
            get
            {
                return ModPrefs.GetBool(PluginName, KeyDarthMode, true);
            }

            set
            {
                ModPrefs.SetBool(PluginName, KeyDarthMode, value);
            }
        }

        public static bool IsOneHanded
        {
            get
            {
                return ModPrefs.GetBool(PluginName, KeyOneHanded, true);
            }

            set
            {
                ModPrefs.SetBool(PluginName, KeyOneHanded, value);
            }
        }

        public static bool IsAutoDetect
        {
            get
            {
                return ModPrefs.GetBool(PluginName, KeyAutoDetect, true);
            }

            set
            {
                ModPrefs.SetBool(PluginName, KeyAutoDetect, value);
            }
        }

        public static int Separation
        {
            get
            {
                return ModPrefs.GetInt(PluginName, KeySeparation, 15);
            }

            set
            {
                ModPrefs.SetInt(PluginName, KeySeparation, value);
            }
        }

        public string[] Filter { get; }

        public void OnApplicationStart()
        {
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            CheckForUserDataFolder();
            _instance = this;
        }

		public void OnApplicationQuit()
		{
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            _instance = null;
        }

		private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
		{
            try
            {
                if (scene.name == "Menu")
                {
                    panelBehavior = new GameObject("panelBehavior").AddComponent<PanelBehavior>();
                }
                else if (scene.name == "StandardLevelLoader")
                {
                    if (!loader)
                        loader = Resources.FindObjectsOfTypeAll<AsyncScenesLoader>().FirstOrDefault();
                    loader.loadingDidFinishEvent += OnLoadingDidFinishGame;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

        public void OnLoadingDidFinishGame()
        {
            darthMaulBehavior = new GameObject("DarthMaulBehavior").AddComponent<DarthMaulBehavior>();
        }

        private void CheckForUserDataFolder()
        {
            string userDataPath = Environment.CurrentDirectory + "/UserData";
            if (!Directory.Exists(userDataPath))
            {
                Directory.CreateDirectory(userDataPath);
            }
            if ("".Equals(ModPrefs.GetString(PluginName, KeyDarthMode, "")))
            {
                ModPrefs.SetBool(PluginName, KeyDarthMode, false);
            }
            if ("".Equals(ModPrefs.GetString(PluginName, KeyOneHanded, "")))
            {
                ModPrefs.SetBool(PluginName, KeyOneHanded, false);
            }
            if ("".Equals(ModPrefs.GetString(PluginName, KeyAutoDetect, "")))
            {
                ModPrefs.SetBool(PluginName, KeyAutoDetect, false);
            }
            if ("".Equals(ModPrefs.GetString(PluginName, KeySeparation, "")))
            {
                ModPrefs.SetInt(PluginName, KeySeparation, 15);
            }
        }

        public static void ToggleDarthMode()
        {
            if (darthMaulBehavior != null)
            {
                darthMaulBehavior.ToggleDarthMode();
            }
        }

        public static void ToggleDarthMode(bool enable)
        {
            if (enable != IsDarthModeOn)
            {
                ToggleDarthMode();
            }
        }

        public static void ToggleOneHanded()
        {
            if (darthMaulBehavior != null)
            {
                darthMaulBehavior.ToggleOneHanded();
            }
        }

        public static void ToggleOneHanded(bool enable)
        {
            if (enable != IsOneHanded)
            {
                ToggleOneHanded();
            }
        }

        public void OnLevelWasLoaded(int level)
		{
		}

		public void OnLevelWasInitialized(int level)
		{
		}

		public void OnUpdate()
		{
        }

        public void OnLateUpdate()
        {

        }

        public void OnFixedUpdate()
		{
		}
	}
}
