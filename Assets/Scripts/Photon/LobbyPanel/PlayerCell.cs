using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_Lobby
{
    public class PlayerCell : MonoBehaviour
    {
        public string Name = null;
        public bool IsReady = false;
        public Text Name_Text = null;
        public Image IsReady_Image = null;

        public void ResetInfo()
        {
            Name = null;
            Name_Text.text = Name;

            IsReady = false;
            IsReady_Image.color = Color.white;
        }

        public void SetInfo(string name, bool isReady)
        {
            Name = name;
            Name_Text.text = Name;

            IsReady = isReady;
            IsReady_Image.color = isReady ? Color.green : Color.red;
        }
    }
}