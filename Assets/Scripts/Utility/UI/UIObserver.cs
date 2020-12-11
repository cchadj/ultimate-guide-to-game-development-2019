using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIObserver : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textField;
    [SerializeField] private ObservableFloatVariable _observableValue;
    
    private string _initialString;

    private void Awake()
    {
        _initialString = _textField.text;
    }

    private void OnEnable()
    {
        _observableValue.valueChanged += UpdatePlayerHealthText;
    }

    private void OnDisable()
    {
        _observableValue.valueChanged -= UpdatePlayerHealthText;
    }

    private void UpdatePlayerHealthText(object sender, ScriptableEventArgs valueArgs)
    {
        _textField.SetText($"{_initialString} {valueArgs.GetArguments<FloatVariable>().Value}");     
    }

}
