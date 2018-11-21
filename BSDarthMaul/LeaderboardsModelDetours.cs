using IllusionPlugin;
using System;
using System.Reflection;

namespace BSDarthMaul
{
    public class LeaderboardsModelDetours
    {
        private static bool isHDModChecked;
        private static bool isHDMod;
        private static IPlugin HDModPlugin;

        public static string GetLeaderboardID(IStandardLevelDifficultyBeatmap difficultyLevel, GameplayMode gameplayMode)
        {
            CheckForHiddenBlocks();
            string text = "Unknown";
            switch (difficultyLevel.difficulty)
            {
                case LevelDifficulty.Easy:
                    text = "Easy";
                    break;
                case LevelDifficulty.Normal:
                    text = "Normal";
                    break;
                case LevelDifficulty.Hard:
                    text = "Hard";
                    break;
                case LevelDifficulty.Expert:
                    text = "Expert";
                    break;
                case LevelDifficulty.ExpertPlus:
                    text = "ExpertPlus";
                    break;
            }
            string text2 = "Unknown";
            switch (gameplayMode)
            {
                case GameplayMode.SoloStandard:
                    text2 = "SoloStandard";
                    break;
                case GameplayMode.SoloOneSaber:
                    text2 = "SoloOneSaber";
                    break;
                case GameplayMode.SoloNoArrows:
                    text2 = "SoloNoArrows";
                    break;
                case GameplayMode.PartyStandard:
                    text2 = "PartyStandard";
                    break;
            }
            string leaderboardID = string.Concat(new string[]
            {
            difficultyLevel.level.levelID,
            "_",
            text,
            "_",
            text2
            });
            if (isHDMod)
                leaderboardID += "HD";
            if (Plugin.IsDarthModeOn && Plugin.NoArrowRandLv == 0)
            {
                leaderboardID += "DM";
                if (Plugin.IsOneHanded)
                    leaderboardID += "OH";
            }
            return leaderboardID;
        }

        private static void CheckForHiddenBlocks()
        {
            try
            {
                if (isHDModChecked && HDModPlugin == null)
                    return;
                isHDModChecked = true;

                if (HDModPlugin == null)
                {
                    foreach (var plugin in IllusionInjector.PluginManager.Plugins)
                    {
                        if (plugin.Name == "HiddenBlocks")
                        {
                            HDModPlugin = plugin;
                            break;
                        }
                    }
                }
                    
                if (HDModPlugin != null)
                {
                    Type type = HDModPlugin.GetType();
                    isHDMod = (bool)type.GetField("enableHiddenBlocks", BindingFlags.Static | BindingFlags.Public).GetValue(null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

    }
}
