using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.XR;

namespace BSDarthMaul 
{
    class HapticFeedbackControllerDetours 
    {
        public static void Rumble(HapticFeedbackController t, XRNode node, float duration, float impulseLength, float intervalTime) 
        {
            if (!ReflectionUtil.GetPrivateField<MainSettingsModel>(t, "_mainSettingsModel").controllersRumbleEnabled) 
            {
                return;
            }
            if (Plugin.IsOneHanded && node == XRNode.LeftHand) node = XRNode.RightHand;
            SharedCoroutineStarter.instance.StartCoroutine(t.OneShotRumbleCoroutine(node, duration, impulseLength, intervalTime));
        }
    };
}
