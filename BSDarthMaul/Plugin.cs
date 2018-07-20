using BSDarthMaul.Components;
using HMUI;
using IllusionPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using VRUI;

namespace BSDarthMaul
{
	public class Plugin : IEnhancedPlugin, IPlugin
	{
        public static string PluginName = "Darth Maul Plugin";
        public const string VersionNum = "0.2.0";

        private static Plugin _instance;

        private DarthMaulBehavior darthMaulBehavior;
        private static GameOptionToggle darthModeToggle;
        private static GameOptionToggle oneHandedToggle;

        public static Dictionary<string, Sprite> Icons = new Dictionary<string, Sprite>();

        public const string KeyDarthMode = "DMDarthMode";
        public const string KeyOneHanded = "DMOneHanded";
        public const string KeySeparation = "DMSerparation";
        public const string KeyAutoDetect = "DMAutoDetect";

        public static bool IsDarthModeOn
        {
            get
            {
                return (darthModeToggle != null) ? darthModeToggle.Value : false;
            }

            set
            {
                if(darthModeToggle != null)
                    darthModeToggle.Value = value;
                ModPrefs.SetBool(PluginName, KeyDarthMode, value);
            }
        }

        public static bool IsOneHanded
        {
            get
            {
                return (oneHandedToggle != null) ? oneHandedToggle.Value : false;
            }

            set
            {
                if (oneHandedToggle != null)
                    oneHandedToggle.Value = value;
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

        public string Name
		{
			get
			{
				return PluginName;
			}
		}

		public string Version
		{
			get
			{
				return VersionNum;
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
                if (scene.buildIndex == 1)
                {
                    GetBeatSaberIcons();
                    AddModMenuButton();
                }
                if (scene.buildIndex == 5)
                {
                    this.darthMaulBehavior = new GameObject("DarthMaulBehavior").AddComponent<DarthMaulBehavior>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
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

        private void GetBeatSaberIcons()
        {
            if (Icons.Count > 0)
            {
                return;
            }
            Sprite[] array = Resources.FindObjectsOfTypeAll<Sprite>();
            for (int i = 0; i < array.Length; i++)
            {
                Sprite item = array[i];
                Icons.Add(item.name, item);
            }
        }

        private void AddModMenuButton()
        {
            MainMenuViewController _mainMenuViewController = Resources.FindObjectsOfTypeAll<MainMenuViewController>().First();
            GameplayOptionsViewController _gameplayOptionsViewController = Resources.FindObjectsOfTypeAll<GameplayOptionsViewController>().First();
            _gameplayOptionsViewController.transform.Find("InfoText").gameObject.SetActive(false);
            RectTransform container = (RectTransform)_gameplayOptionsViewController.transform.Find("Container");
            //Console.WriteLine("container.sizeDelta.x : " + container.sizeDelta.x + " container.sizeDelta.y : " + container.sizeDelta.y);
            container.sizeDelta = new Vector2(container.sizeDelta.x, container.sizeDelta.y + 14f);
            container.Translate(new Vector3(0, -0.1f, 0));
            Transform noEnergy = container.Find("NoEnergy");
            bool IsDarthModeOn = ModPrefs.GetBool(PluginName, KeyDarthMode, false);
            bool IsOneHanded = ModPrefs.GetBool(PluginName, KeyOneHanded, false);
            darthModeToggle = new GameOptionToggle(container.gameObject, noEnergy.gameObject, KeyDarthMode, Icons["NoteCutInfoIcon"], "DMaul Mode", IsDarthModeOn);
            oneHandedToggle = new GameOptionToggle(container.gameObject, noEnergy.gameObject, KeyOneHanded, Icons["SingleSaberIcon"], "OneHanded", IsOneHanded);
        }

        public static void ToggleDarthMode()
        {
            if (_instance != null && _instance.darthMaulBehavior != null)
            {
                _instance.darthMaulBehavior.ToggleDarthMode();
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
            if (_instance != null && _instance.darthMaulBehavior != null)
            {
                _instance.darthMaulBehavior.ToggleOneHanded();
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
