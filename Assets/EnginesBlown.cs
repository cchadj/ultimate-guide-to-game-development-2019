using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnginesBlown : MonoBehaviour
{
    [SerializeField] private ObservableFloatVariable _playerHealth;
    
    [SerializeField] private GameObject _leftEngineBlown;
    
    [SerializeField] private GameObject _rightEngineBlown;

    private void OnEnable()
    {
        _playerHealth.valueChanged += PlayerHealthChanged; 
    }
    
    private void OnDisable()
    {
        _playerHealth.valueChanged -= PlayerHealthChanged; 
    }

    private void PlayerHealthChanged(object sender, ScriptableEventArgs e)
    {
        var curHealth = e.GetArguments<FloatVariable>()?.Value;
        
        if (curHealth == null) return;
        
        if (curHealth < 3.0f)
        {
                _rightEngineBlown.gameObject.SetActive(true); 
                if (curHealth < 2.0f)
                    _leftEngineBlown.gameObject.SetActive(true); 
        }
        else
        {
            _rightEngineBlown.SetActive(false);
            _leftEngineBlown.SetActive(false);
        }
        
    }
}
