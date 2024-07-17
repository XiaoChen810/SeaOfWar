using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricFogAndMist2;

namespace ChenChen_Core
{
    public class FogOfWar : MonoBehaviour
    {
        public VolumetricFog fogVolume;
        public float fogHoleRadius = 8f;    // �Ƴ�����ķ�Χ
        public float clearDuration = 0.2f;  // ����ļ��

        public void ClearFog(Vector3 position)
        {
            fogVolume.SetFogOfWarAlpha(position, radius: fogHoleRadius, fogNewAlpha: 0, duration: clearDuration);
        }

        public void ClearFog(Vector3 position, float radius)
        {
            fogVolume.SetFogOfWarAlpha(position, radius: radius, fogNewAlpha: 0, duration: clearDuration);
        }
    }
}