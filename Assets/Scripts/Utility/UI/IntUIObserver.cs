using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IntUIObserver : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textField;
    [SerializeField] private ObservableIntVariable _observableValue;
    
    private string _initialString;

    private void Awake()
    {
        _initialString = _textField.text;
        UpdateText(_observableValue);
    }

    private void OnEnable()
    {
        _observableValue.ValueChanged.AddListener<ObservableIntVariable>(this, UpdateText);
    }

    private void OnDisable()
    {
        _observableValue.ValueChanged.RemoveListener<ObservableIntVariable>(this, UpdateText);
    }

    private void UpdateText(ObservableIntVariable value)
    {
        _textField.SetText($"{_initialString} {value.Value}");     
    }

}
