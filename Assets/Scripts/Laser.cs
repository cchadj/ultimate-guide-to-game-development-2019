using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private Transform _transform;

    [SerializeField] private float _speed;

    private const float TopBound = 5.5f;
    private const float DefaultSpeed = 8;
    
    // Update is called once per frame
    private void Awake()
    {
        _transform = GetComponent<Transform>();
        
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
            Destroy(this.gameObject);
    }
}
