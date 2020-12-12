using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldAnimationController : MonoBehaviour
{
    [SerializeField] private ObservableIntVariable _playerShield;
    [SerializeField] private GameObject _shieldGameObject;

    private void OnEnable()
    {
        _playerShield.ValueChanged.AddListener<IntVariable>(this, ValueChanged);
    }

    private void OnDisable()
    {
       _playerShield.ValueChanged.RemoveListener<IntVariable>(this, ValueChanged);
    }
    
    private void ValueChanged(IntVariable shieldPoints)
    {
        var isShieldActive = shieldPoints.Value != 0 ;
        _shieldGameObject.SetActive(isShieldActive);
    }
}
