using UnityEngine;

namespace Utils
{
    public class SingletonMono<T> : MonoBehaviour where T : Component
    {
        public static T Instance { get; private set; }

        protected virtual void Awake() => Instance = this as T;
    }
}