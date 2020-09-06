using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mukuro.Dialog
{
    public class MenuWindow : MenuWindowBase
    {
        [SerializeField] private GameObject activationTarget = default;

        [SerializeField] private Transform menuRoot = default;

        [SerializeField] private GameObject menuItemTemplate = default;

        private string[] Items { get; set; }
        private Text[] Labels { get; set; }
        
        private int CursorPosition { get; set; }
        
        private Action<int> OnSelected { get; set; }

        private float openTimer = 0;


        public override void Open(DialogShowMenuSettings settings, Action<int> onSelected)
        {
            activationTarget?.SetActive(true);
            Items = settings.Items;
            Labels = new Text[Items.Length];
            OnSelected = onSelected;

            //Clear 
            int childCount = menuRoot.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Destroy(menuRoot.GetChild(i).gameObject);
            }

            //Add
            for (int i = 0; i < Items.Length; i++)
            {
                var item = Instantiate(menuItemTemplate, menuRoot);
                item.SetActive(true);
                Labels[i] = item.GetComponent<Text>();
            }

            CursorPosition = 0;
            UpdateLabels();
            openTimer = Time.time;
        }

        private void UpdateLabels()
        {
            for (int i = 0; i < Items.Length; i++)
            {
                string l = "";
                if (i == CursorPosition)
                {
                    l = $"▶ {Items[i]}";
                }
                else
                {
                    l = $"<color=#ffffff00>▶</color> {Items[i]}";
                }

                Labels[i].text = l;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        private float tempVerticalInput;
        void Update()
        {
            if (Items == null || Time.time - openTimer < 0.2f) return;
            float v = Input.GetAxis("Vertical");
            if (tempVerticalInput <= 0 && v > 0)
            {
                CursorPosition--;
                CursorPosition += Items.Length;
                CursorPosition %= Items.Length;
                UpdateLabels();
            }else if (tempVerticalInput >= 0 && v < 0)
            {
                CursorPosition++;
                CursorPosition %= Items.Length;
                UpdateLabels();
            }
            
            tempVerticalInput = v;

            if (Items != null && Input.GetButtonDown("Fire1"))
            {
                OnSelected?.Invoke(CursorPosition);
                OnSelected = null;
                Items = null;
                activationTarget?.SetActive(false);
            }
        }
    }
}