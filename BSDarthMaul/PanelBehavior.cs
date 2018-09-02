using BSDarthMaul.Components;
using HMUI;
using PlayHooky;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace BSDarthMaul
{
    public class PanelBehavior : MonoBehaviour
    {
        private MainMenuViewController _mainMenuViewController = null;
        private SoloModeSelectionViewController _soloModeSelectionViewController;
        private StandardLevelSelectionFlowCoordinator _levelSelectionFlowCoordinator;
        private StandardLevelSelectionNavigationController _levelSelectionNavigationController;
        private StandardLevelListViewController _levelListViewController;
        private StandardLevelDetailViewController _levelDetailViewController;
        private StandardLevelDifficultyViewController _levelDifficultyViewController;
        private LevelCollectionsForGameplayModes _levelCollectionsForGameplayModes;
        StandardLevelListTableView listTableView = null;
        TableView tableView = null;

        private static Dictionary<string, Sprite> Icons = new Dictionary<string, Sprite>();

        private GameOptionToggle darthModeToggle;
        private GameOptionToggle oneHandedToggle;

        private LeaderboardsModelHooks leaderboardHooks;

        public bool IsDarthModeOn
        {
            get
            {
                return (darthModeToggle != null) ? darthModeToggle.Value : false;
            }

            set
            {
                if (darthModeToggle != null)
                    darthModeToggle.Value = value;
            }
        }

        public bool IsOneHanded
        {
            get
            {
                return (oneHandedToggle != null) ? oneHandedToggle.Value : false;
            }

            set
            {
                if (oneHandedToggle != null)
                    oneHandedToggle.Value = value;
            }
        }

        private void Awake()
        {
            try
            {
                _mainMenuViewController = Resources.FindObjectsOfTypeAll<MainMenuViewController>().First();
                _soloModeSelectionViewController = Resources.FindObjectsOfTypeAll<SoloModeSelectionViewController>().First();
                _levelSelectionFlowCoordinator = Resources.FindObjectsOfTypeAll<StandardLevelSelectionFlowCoordinator>().First();
                _levelSelectionNavigationController = _levelSelectionFlowCoordinator.GetPrivateField<StandardLevelSelectionNavigationController>("_levelSelectionNavigationController");
                _levelDetailViewController = _levelSelectionFlowCoordinator.GetPrivateField<StandardLevelDetailViewController>("_levelDetailViewController");
                _levelDifficultyViewController = _levelSelectionFlowCoordinator.GetPrivateField<StandardLevelDifficultyViewController>("_levelDifficultyViewController");
                _levelListViewController = _levelSelectionFlowCoordinator.GetPrivateField<StandardLevelListViewController>("_levelListViewController");
                listTableView = _levelListViewController.GetPrivateField<StandardLevelListTableView>("_levelListTableView");
                tableView = listTableView.GetPrivateField<TableView>("_tableView");
                GetBeatSaberIcons();
                AddModMenuButton();
                leaderboardHooks = new LeaderboardsModelHooks();
                leaderboardHooks.StartHooking();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void OnDestroy()
        {
            leaderboardHooks.UnHookAll();
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
            GameplayOptionsViewController _gameplayOptionsViewController = _levelDetailViewController.GetPrivateField<GameplayOptionsViewController>("_gameplayOptionsViewController");
            _gameplayOptionsViewController.transform.Find("InfoText").gameObject.SetActive(false);
            RectTransform container = (RectTransform)_gameplayOptionsViewController.transform.Find("Switches").Find("Container");
            //DumpChildren(_gameplayOptionsViewController.transform);
            //Console.WriteLine("container.sizeDelta.x : " + container.sizeDelta.x + " container.sizeDelta.y : " + container.sizeDelta.y);
            container.sizeDelta = new Vector2(container.sizeDelta.x, container.sizeDelta.y + 14f);
            container.Translate(new Vector3(0, -0.1f, 0));
            Transform noEnergy = container.Find("NoEnergy");
            bool IsDarthModeOn = Plugin.IsDarthModeOn;
            bool IsOneHanded = Plugin.IsOneHanded;
            darthModeToggle = new GameOptionToggle(container.gameObject, noEnergy.gameObject, Plugin.KeyDarthMode, Icons["NoteCutInfoIcon"], "DMaul Mode", IsDarthModeOn);
            oneHandedToggle = new GameOptionToggle(container.gameObject, noEnergy.gameObject, Plugin.KeyOneHanded, Icons["SingleSaberIcon"], "DM Onehanded", IsOneHanded);
            darthModeToggle.OnToggle += OnDarthMualModeToggle;
            oneHandedToggle.OnToggle += OnOneHandedToggle;
        }

        
        private void DumpChildren(Transform parent)
        {
            foreach (Transform child in parent.GetComponentsInChildren<Transform>())
            {
                Console.WriteLine("Child.name: " + child.name + " parent: " + child.parent.name);
                if (child.name == "Container")
                {
                    Console.WriteLine(child);
                }
            }
        }

        void OnDarthMualModeToggle(bool isOn)
        {
            CheckForDarthModeLeaderBoard();
        }

        void OnOneHandedToggle(bool isOn)
        {
            if(Plugin.IsDarthModeOn)
                CheckForDarthModeLeaderBoard();
        }

        private void CheckForDarthModeLeaderBoard()
        {
            try
            {
                var mode = _levelSelectionFlowCoordinator.GetPrivateField<GameplayMode>("_gameplayMode");
                if (mode != GameplayMode.SoloStandard && mode != GameplayMode.SoloNoArrows)
                    return;
                if (_levelCollectionsForGameplayModes == null)
                    _levelCollectionsForGameplayModes = FindObjectOfType<LevelCollectionsForGameplayModes>();

                var difficultyLevel = _levelDetailViewController.GetPrivateField<IStandardLevelDifficultyBeatmap>("_difficultyLevel");
                
                var levels = _levelCollectionsForGameplayModes.GetLevels(mode);

                /****Doesn't work, abandoned.****/
                /*if (Plugin.IsDarthModeOn)
                {
                    foreach (IStandardLevel level in levels)
                    {
                        if(!level.levelID.Contains("DarthMaulMode"))
                            level.SetPrivateField("_levelID", level.levelID + "DarthMaulMode");
                    }
                }
                else
                {
                    foreach (IStandardLevel level in levels)
                    {
                        if (level.levelID.Contains("DarthMaulMode"))
                            level.SetPrivateField("_levelID", StringReplace(level.levelID, "DarthMaulMode", ""));
                    }
                }*/

                _levelSelectionNavigationController.DismissModalViewController(null, true);
                _levelSelectionFlowCoordinator.Present(_soloModeSelectionViewController, levels, mode);

                if (difficultyLevel != null)
                {
                    int row = listTableView.RowNumberForLevelID(difficultyLevel.level.levelID);
                    tableView.SelectRow(row, true);
                    tableView.ScrollToRow(row, false);
                    _levelDifficultyViewController.SetDifficultyLevels(difficultyLevel.level.difficultyBeatmaps, difficultyLevel);
                    _levelDetailViewController.SetContent(difficultyLevel, mode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }

        }
        

        private void Update()
        {
            try
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

        public class LeaderboardsModelHooks
        {
            private HookManager hookManager;
            private Dictionary<string, MethodInfo> hooks;

            public void StartHooking()
            {
                this.hookManager = new HookManager();
                this.hooks = new Dictionary<string, MethodInfo>();
                this.Hook("LeaderboardsModel", typeof(LeaderboardsModel).GetMethod("GetLeaderboardID"), typeof(LeaderboardsModelDetours).GetMethod("GetLeaderboardID"));
            }

            public void UnHookAll()
            {
                foreach (string key in this.hooks.Keys)
                    this.UnHook(key);
            }

            private bool Hook(string key, MethodInfo target, MethodInfo hook)
            {
                if (this.hooks.ContainsKey(key))
                    return false;
                try
                {
                    this.hooks.Add(key, target);
                    this.hookManager.Hook(target, hook);
                    Console.WriteLine($"{key} hooked!");
                    return true;
                }
                catch (Win32Exception ex)
                {
                    Console.WriteLine($"Unrecoverable Windows API error: {(object)ex}");
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unable to hook method, : {(object)ex}");
                    return false;
                }
            }

            private bool UnHook(string key)
            {
                MethodInfo original;
                if (!this.hooks.TryGetValue(key, out original))
                    return false;
                this.hookManager.Unhook(original);
                return true;
            }
        }
    }
}
