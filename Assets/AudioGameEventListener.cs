using ModestTree;
using UnityEngine;

public class AudioGameEventListener : MonoBehaviour
{
    [SerializeField] private GameEvent _playerFired;

    [SerializeField] private AudioClip[] _audioClips;
    
    [SerializeField] private AudioSource _audioSource;

    private void Awake()
    {
        if (_audioClips == null || _audioClips.IsEmpty())
        {
            Debug.LogError($"{nameof(AudioGameEventListener)}::{nameof(Awake)}: Make sure that {nameof(_audioClips)} are set through the editor");
            enabled = false;
        }
    }

    private void OnEnable()
    {
        _playerFired.AddListener(this, PlaySound);
    }
    
    private void OnDisable()
    {
        _playerFired.RemoveListener(this, PlaySound);
    }

    private void PlaySound()
    {
        _audioSource.clip = _audioClips[Random.Range(0, _audioClips.Length)];
        _audioSource.Play();
    }
}
