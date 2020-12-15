using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

public enum BulletDirection {
    Up, 
    Down
}
public class Laser : MonoBehaviour, IDestructible, IBullet
{

    #region SetByUnity
    [SerializeField] private float _speed;
    [field: SerializeField] public BulletTypeScriptable BulletTypeData { get; private set; }

    [SerializeField] private BulletDirection _bulletDirection;

    [field: SerializeField] public float BulletDamage { get; private set; } = 1;

    #endregion

    public BulletType BulletType { get; private set; }

    private Transform _transform;
    
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

        var direction = _bulletDirection == BulletDirection.Up ? 1 : -1;
        _transform.Translate(new Vector3(.0f, direction * Time.deltaTime * _speed ,.0f));
       
        if (_transform.position.y > TopBound)
            gameObject.SetActive(false);
    }

    public void Destroy()
    {
        gameObject.SetActive(false); 
    }

}
