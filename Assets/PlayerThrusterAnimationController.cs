using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrusterAnimationController : MonoBehaviour
{
    private Animator _animator;

    private int _thrustersOnBoolHash;
    
    private void InjectDependencies()
    {
        
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        _thrustersOnBoolHash = Animator.StringToHash("ThrustersOn");
    }

    public void StopThrusters()
    {
        _animator.SetBool(_thrustersOnBoolHash, false);
    }
    
    public void StartThrusters()
    {
        _animator.SetBool(_thrustersOnBoolHash, true);
    }
}
