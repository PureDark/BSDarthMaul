using HMUI;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BSDarthMaul.Components
{
    public abstract class GameOptionSetting
    {
        public GameObject gameObject;
        
        private TextMeshProUGUI _nameText;
        
        private Button _decButton;
        
        private Button _incButton;

        private TextMeshProUGUI _text;

        private int _idx;

        private int _numberOfElements;

        protected abstract void GetInitValues(out int idx, out int numberOfElements);

        protected abstract void ApplyValue(int idx);

        protected abstract string TextForValue(int idx);

        internal string NameText
        {
            get
            {
                return this._nameText.text;
            }
            set
            {
                this._nameText.text = value;
            }
        }

        protected string text
        {
            set
            {
                this._text.text = value;
            }
        }

        protected bool enableDec
        {
            set
            {
                this._decButton.interactable = value;
            }
        }

        protected bool enableInc
        {
            set
            {
                this._incButton.interactable = value;
            }
        }

        protected void OnEnable()
        {
            this._incButton.onClick.AddListener(new UnityAction(this.IncButtonPressed));
            this._decButton.onClick.AddListener(new UnityAction(this.DecButtonPressed));
        }

        private void OnDisable()
        {
            this._incButton.onClick.RemoveListener(new UnityAction(this.IncButtonPressed));
            this._decButton.onClick.RemoveListener(new UnityAction(this.DecButtonPressed));
        }


        public void Init()
        {
            this.GetInitValues(out this._idx, out this._numberOfElements);
            this.RefreshUI();
        }

        public void ApplySettings()
        {
            this.ApplyValue(this._idx);
        }

        private void RefreshUI()
        {
            text = this.TextForValue(this._idx);
            enableDec = (this._idx > 0);
            enableInc = (this._idx < this._numberOfElements - 1);
            ApplySettings();
        }

        public void IncButtonPressed()
        {
            if (this._idx < this._numberOfElements - 1)
            {
                this._idx++;
            }
            this.RefreshUI();
        }

        public void DecButtonPressed()
        {
            if (this._idx > 0)
            {
                this._idx--;
            }
            this.RefreshUI();
        }

        internal GameOptionSetting(GameObject parent, GameObject target, string gonename, string text)
        {
            this.gameObject = NGUIUtil.SetCloneChild(parent, target, gonename);
            foreach(ListSettingsController component in gameObject.GetComponentsInChildren<ListSettingsController>())
            {
                UnityEngine.Object.Destroy(component);
            }
            this._nameText = gameObject.GetComponentsInChildren<TextMeshProUGUI>()[0];
            this.NameText = text;
            this._decButton = gameObject.GetComponentsInChildren<Button>()[0];
            this._incButton = gameObject.GetComponentsInChildren<Button>()[1];
            this._text = gameObject.GetComponentsInChildren<TextMeshProUGUI>()[1];
            OnEnable();
            Init();
        }
        
    }
}
