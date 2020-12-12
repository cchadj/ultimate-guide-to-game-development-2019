using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TripleShot : MonoBehaviour
{

    [SerializeField] private List<GameObject> _lasers;
    private List<Vector3> _cachedLaserLocalPositions;
    
    private int _objectsDisabled;

    private void Awake()
    {
        _cachedLaserLocalPositions = new List<Vector3>();
        foreach (var laser in _lasers)
        {
            _cachedLaserLocalPositions.Add(laser.transform.localPosition);
        }
    }

//    private void Start()
//    {
//        Debug.Log("Hello");
//    }

    private void OnEnable()
    {
        _objectsDisabled = 0;
        for (var i = 0; i < _lasers.Count; i++)
        {
            var laser = _lasers[i];
            var listener = laser.gameObject.GetStateListener();
            laser.transform.localPosition = _cachedLaserLocalPositions[i];
            listener.Disabled +=  OnObjectDisabled;
            laser.SetActive(true);
        }
    }

    private void OnDisable()
    {
        foreach (var laser in _lasers)
        {
            var listener = laser.gameObject.GetStateListener();
            listener.Disabled -=  OnObjectDisabled;
        }
    }

    private void OnObjectDisabled()
    {
        _objectsDisabled++;
        if (_objectsDisabled == _lasers.Count)
        {
            // Deactivating the parent also deactivates children.
            // That means that the child that was deactivated last will also
            // deactivate twice in the same frame. Make sure we  wait for next frame 
            // until we deactivate.
            StartCoroutine(DeactivateNextFrame());
        }
    }

    private IEnumerator DeactivateNextFrame()
    {
        // Waits for one frame before deactivating. 
        yield return null;
        gameObject.SetActive(false);
    }
    
}
 
public static class ObjectStateExtensions
{
    public static IStateListener GetStateListener(this GameObject obj)
    {
        return obj.GetComponent<ObjectStateListener>() ?? obj.AddComponent<ObjectStateListener>();
    }
 
    public interface IStateListener
    {
        event Action Enabled;
        event Action Disabled;
    }

    private class ObjectStateListener : MonoBehaviour, IStateListener
    {
        public event Action Enabled;
        public event Action Disabled;

        private void Awake()
        {
            hideFlags = HideFlags.DontSaveInBuild | HideFlags.HideInInspector;
        }
 
        private void OnEnable()
        {
            Enabled?.Invoke();
        }

        private void OnDisable()
        {
            Disabled?.Invoke();
        }

    }
}
