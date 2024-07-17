using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_UI
{
    /// <summary>
    /// �洢UI����Ϣ��·�������� 
    /// </summary>
    [System.Serializable]
    public class UIType
    {
        public string Name { get; private set; }

        public string Path { get; private set; }

        public UIType(string Path)
        {
            this.Path = Path;
            Name = Path.Substring(Path.LastIndexOf('/') + 1);
        }
    }
}