using System;
using Photon.Pun;
using UnityEngine;

namespace Utils
{
    public class SingletonMonoPun<T>:MonoBehaviourPun where T : Component
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            Instance = this as T;
        }
    }
}