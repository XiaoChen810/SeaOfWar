using Fusion;
using UnityEngine;

namespace ChenChen_Core
{
    public enum MyButtons
    {
        Test,
    }

    public struct InputData : INetworkInput
    {
        public NetworkButtons Buttons;

    }
}