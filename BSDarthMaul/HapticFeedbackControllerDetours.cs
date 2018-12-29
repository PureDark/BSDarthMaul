using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.XR;
using static BSDarthMaul.Plugin;

namespace BSDarthMaul 
{
    class HapticFeedbackControllerDetours 
    {

        public static void Rumble(HapticFeedbackController t, XRNode node, float duration, float impulseStrength, float intervalDuration) 
        {
            if (!ReflectionUtil.GetPrivateField<MainSettingsModel>(t, "_mainSettingsModel").controllersRumbleEnabled) 
            {
                return;
            }
            if (Plugin.IsOneHanded)
            {
                if(Plugin.MainController == ControllerType.RIGHT && node == XRNode.LeftHand)
                {
                    node = XRNode.RightHand;
                }
                else if (Plugin.MainController == ControllerType.LEFT && node == XRNode.RightHand)
                {
                    node = XRNode.LeftHand;
                }
            }
            SharedCoroutineStarter.instance.StartCoroutine(t.OneShotRumbleCoroutine(node, duration, impulseStrength, intervalDuration));
        }
    };
}
