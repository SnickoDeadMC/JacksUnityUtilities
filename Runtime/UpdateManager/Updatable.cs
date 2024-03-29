using System;
using System.Reflection;
using MyBox;
using UnityEngine;

namespace JacksUtils
{
    /// <summary>
    /// A faster way to run updates than MonoBevahiour.
    /// From a test of 10,000 updates, this method ran on average 0.0346ms per call, whereas Update() ran in 1.4ms
    /// Aside from speed, it makes scripts more managable and includes other helpful behaviours like Initialise()
    /// </summary>
    public class Updatable : MonoBehaviour
    {

        private static readonly Type baseType = typeof(Updatable);

        [Foldout("Updatable", true)]
        [SerializeField, ReadOnly] protected bool initialised = false;

        public bool IsInitialised => initialised;

        [Space(5)]
        [SerializeField, ReadOnly] protected bool isUpdateUsed;

        [SerializeField, ReadOnly] protected bool isLateUpdateUsed;
        [SerializeField, ReadOnly] protected bool isFixedUpdateUsed;

        [Space(5)]
        [SerializeField, ReadOnly] protected bool updateIsRegistered;

        [SerializeField, ReadOnly] protected bool lateUpdateIsRegistered;

        [Foldout("Updatable")] //end the foldout
        [SerializeField, ReadOnly] protected bool fixedUpdateIsRegistered;

        private void OnValidate()
        {
            //the initialised value should never be true while in editor (this can happen if copying components etc.)
            if (initialised && !Application.isPlaying)
                initialised = false;
        }

        protected virtual void Awake()
        {
            if (!initialised)
            {
                Initialise();
            }
        }

        protected virtual void Initialise()
        {
            initialised = true;

            Type finalType = GetType();
            isUpdateUsed = IsMethodUsed(finalType, "FastUpdate");
            isLateUpdateUsed = IsMethodUsed(finalType, "FastLateUpdate");
            isFixedUpdateUsed = IsMethodUsed(finalType, "FastFixedUpdate");
        }

        protected virtual void OnEnable()
        {
            ListenToUpdateManager();
        }

        protected virtual void OnDisable()
        {
            StopListeningToUpdateManager();
        }

        private void ListenToUpdateManager()
        {
            if (isUpdateUsed)
            {
                UpdateManager.Instance.OnUpdate += FastUpdate;
                updateIsRegistered = true;
            }

            if (isLateUpdateUsed)
            {
                UpdateManager.Instance.OnLateUpdate += FastLateUpdate;
                lateUpdateIsRegistered = true;
            }

            if (isFixedUpdateUsed)
            {
                UpdateManager.Instance.OnFixedUpdate += FastFixedUpdate;
                fixedUpdateIsRegistered = true;
            }
        }

        private void StopListeningToUpdateManager()
        {
            if (isUpdateUsed)
            {
                if (UpdateManager.ExistsRuntime)
                    UpdateManager.Instance.OnUpdate -= FastUpdate;
                updateIsRegistered = false;
            }

            if (isLateUpdateUsed)
            {
                if (UpdateManager.ExistsRuntime)
                    UpdateManager.Instance.OnLateUpdate -= FastLateUpdate;
                lateUpdateIsRegistered = false;
            }

            if (isFixedUpdateUsed)
            {
                if (UpdateManager.ExistsRuntime)
                    UpdateManager.Instance.OnFixedUpdate -= FastFixedUpdate;
                fixedUpdateIsRegistered = false;
            }
        }

        private bool IsMethodUsed(Type type, string method) =>
            type.GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)?.DeclaringType != baseType;

        protected virtual void FastUpdate()
        {

        }

        protected virtual void FastLateUpdate()
        {

        }

        protected virtual void FastFixedUpdate()
        {

        }

    }
}
