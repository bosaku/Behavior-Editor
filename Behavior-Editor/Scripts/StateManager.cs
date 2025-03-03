using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace SFIBehavior
{
    public class StateManager : MonoBehaviour
    {
        public State currentState;

        //public ScriptableObject refsSO; // = new ReorderableList();

        public List<Object> aheadOfTimeObjects = new List<Object>();
        
        [HideInInspector] public float delta;
        [HideInInspector] public Transform mTransform;
        public Dictionary<string,Object> currentObjects = new Dictionary<string, Object>();
        public string EditorSettingsToLoad = "EditorSettings";

        public void Setup()
        {
            Debug.Log("Setup State Manager on " + gameObject.name);
            //EditorSettingsToLoad = "";
        } 
        
        private void Start()
        {
            mTransform = this.transform;
            // objects = new ReorderableList(aheadOfTimeObjects);
            //My hack...
            currentState.OnEnter(this);
        }

        private void Update()
        {
            if (currentState != null)
            {
                currentState.Tick(this);
            }
        }

        private void FixedUpdate()
        {
            if (currentState != null)
            {
                currentState.FixedTick(this);
            }
        }

        public T FindCurrentComponent<T>() where T : Component
        {
            Object o;

            if (!currentObjects.TryGetValue(typeof(T).FullName, out o))
            {
                for (int i = 0; i < SceneManager.loadedSceneCount; i++)
                {
                    o = SFITools.Find.FindObjectByTypeInSceneByName<T>(SceneManager.GetSceneAt(i).name);
                    if(o) continue;
                }
               // o = SFITools.Find.FindObjectByTypeInSceneByName<T>("Waypoint");
                if (o == null) o = FindObjectOfType<T>(true);

                if (o == null)
                {
                    Debug.LogWarning("Missing : " + typeof(T).FullName);
                }
                else
                {
                    currentObjects.Add(typeof(T).FullName, o);
                }
            }

            return o as T;
        }

        public Object FindCurrentObject<T>()
        {
            Object o;
            if (!currentObjects.TryGetValue(typeof(T).FullName, out o))
            {
                Debug.LogError(typeof(T).FullName + " not registered ");
                return null;
            }

            else return o;
        }

        public T FindCurrentComponent<T>(T obj) where T : Component
        {
            if (obj != null) return obj;

            return FindCurrentComponent<T>();
        }
    }
}