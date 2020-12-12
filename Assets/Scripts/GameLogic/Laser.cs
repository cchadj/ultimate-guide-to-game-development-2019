using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject.SpaceFighter;

public class Laser : MonoBehaviour, IDestructible, IBullet
{
    private Transform _transform;

    [SerializeField] private float _speed;

    [field: SerializeField] public BulletTypeScriptable BulletTypeData { get; private set; }

    public BulletType BulletType { get; private set; }

    private const float TopBound = 5.5f;
    private const float DefaultSpeed = 8;
    
    // Update is called once per frame
    private void Awake()
    {
        _transform = GetComponent<Transform>();

        BulletType = (BulletType)Enum.Parse(typeof(BulletType), BulletTypeData.EnumEntryName);
        
        if (_speed < 0.01f)
        {
            _speed = DefaultSpeed;
        }
    }

    private void Update()
    {
        var prevPosition = _transform.position;
        _transform.Translate(new Vector3(.0f, Time.deltaTime * _speed ,.0f));
       
        if (_transform.position.y > TopBound)
            gameObject.SetActive(false);
    }

    public void Destroy()
    {
        gameObject.SetActive(false); 
    }

}
