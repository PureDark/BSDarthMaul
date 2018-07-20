using System;
using UnityEngine;

namespace BSDarthMaul
{
    class DarthMaulToggler : MonoBehaviour
    {
        void Start()
        {
        }

        public void ToggleOneHanded()
        {
            Plugin.ToggleOneHanded();
        }

        public void ToggleDarthMode()
        {
            Plugin.ToggleDarthMode();
        }
    }
}
