using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Lobby
{
    public class LoadingPanel : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        public void Loading()
        {
            animator.SetBool("Loading", true);
        }

        public void EndLoading()
        {
            animator.SetBool("Loading", false);
        }
    }
}