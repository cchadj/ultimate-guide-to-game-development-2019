using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    [SerializeField] private Enemy _enemy;
    
    [SerializeField] private Animator _animator;

    private string _lastClip;

    private int _onDeathTriggerHash;
    private const string OnDeathTrigger = "OnEnemyDeath";

    private bool IsLastClipStillPlaying
    {
        get => _animator.GetCurrentAnimatorStateInfo(0).IsName(OnDeathTrigger);
    }
    
    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _animator = GetComponent<Animator>();

        _onDeathTriggerHash = Animator.StringToHash(OnDeathTrigger);
    }

    public void PlayDeathAnimation()
    {
        _animator.SetTrigger(_onDeathTriggerHash);
        _lastClip = "";
    }

    public void Reset()
    {
        _animator.ResetTrigger(OnDeathTrigger);
    }
}
