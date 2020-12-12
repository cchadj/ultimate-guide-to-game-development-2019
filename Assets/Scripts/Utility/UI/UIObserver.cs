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
        UpdateText(_observableValue.Value);
    }

    private void OnEnable()
    {
        _observableValue.valueChanged += UpdateText;
    }

    private void OnDisable()
    {
        _observableValue.valueChanged -= UpdateText;
    }

    private void UpdateText(object sender, ScriptableEventArgs valueArgs)
    {
        UpdateText(valueArgs.GetArguments<FloatVariable>().Value);
    }

    private void UpdateText(float value)
    {
        _textField.SetText($"{_initialString} {value}");     
    }

}
