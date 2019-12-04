using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public static class GameObjectUtils
{

    /// Returns the currentSelectedGameObject from the EventSystem.
    /// Normally, this is the UI control that has the focus (e.g. a Button, InputField or Toggle).
    public static GameObject GetSelectedGameObject()
    {
        EventSystem eventSystem = GameObjectUtils.FindComponentWithTag<EventSystem>("EventSystem");
        GameObject result = eventSystem.currentSelectedGameObject;
        return result;
    }

    /// Looks in the GameObject with the given tag
    /// for the component that is specified by the generic type parameter.
    public static T FindComponentWithTag<T>(string tag)
    {
        T component;
        GameObject obj = GameObject.FindGameObjectWithTag(tag);
        if (obj)
        {
            component = obj.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError($"Did not find Component '{typeof(T)}' in GameObject with tag '{tag}'.", obj);
            }
            return component;
        }

        return default(T);
    }

    // Looks for a GameObject with the given component, optionally including inactive components.
    // Note that this is a costly method (it searches through all Transforms and their components)
    // that should not be called frequently.
    public static T FindObjectOfType<T>(bool includeInactive) where T : MonoBehaviour
    {
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        if (includeInactive)
        {
            foreach (GameObject rootObject in rootObjects)
            {
                T obj = rootObject.GetComponentInChildren<T>(true);
                if (obj != null)
                {
                    return obj;
                }
            }
            Debug.LogWarning("No object of Type " + typeof(T) + " has been found in the scene.");
            return null;
        }
        else
        {
            return GameObject.FindObjectOfType<T>();
        }
    }
}