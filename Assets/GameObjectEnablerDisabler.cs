using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectEnablerDisabler : MonoBehaviour
{
    [SerializeField] private GameObject _gameObject; 
    
    public void Enable()
    {
        _gameObject.SetActive(true);
    }
    
    public void Disable()
    {
        _gameObject.SetActive(false);
    }
}
