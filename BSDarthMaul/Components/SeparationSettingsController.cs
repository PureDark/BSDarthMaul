using IllusionPlugin;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BSDarthMaul.Components
{
    public class SeparationSettingsController : GameOptionSetting
    {
        protected int[] _separations;

        public SeparationSettingsController(GameObject parent, GameObject target, string goname, string text) 
            : base(parent, target, goname, text)
        {
        }

        protected override void GetInitValues(out int idx, out int numberOfElements)
        {
            int step = 1;
            int min = 0;
            numberOfElements = 101;
            this._separations = new int[numberOfElements];
            for (int i = 0; i < this._separations.Length; i++)
            {
                this._separations[i] = min + step * i;
            }
            int separation = Plugin.Separation;
            idx = numberOfElements - 1;
            for (int j = 0; j < this._separations.Length; j++)
            {
                if (separation == this._separations[j])
                {
                    idx = j;
                    return;
                }
            }
        }

        protected override void ApplyValue(int idx)
        {
            Plugin.Separation = _separations[idx];
        }

        protected override string TextForValue(int idx)
        {
            return string.Format("{0:0.00}", 1f*this._separations[idx]/100);
        }
    }
}

