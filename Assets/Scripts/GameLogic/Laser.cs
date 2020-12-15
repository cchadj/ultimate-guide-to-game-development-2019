using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Zenject;

public class Laser : MonoBehaviour, IDestructible, IBullet
{
    public enum BulletDirection {
        Up, 
        Down
    }
    
    #region SetByUnity
    [SerializeField] private float _speed;
    [field: SerializeField] public BulletTypeScriptable BulletTypeData { get; private set; }
    
    [SerializeField] private BulletDirection _bulletDirection;
    [field: SerializeField] public float BulletDamage { get; private set; } = 1;
    [field:SerializeField] public BulletType BulletType { get; private set; }
    #endregion

    [SerializeField, ReadOnly] private SceneDataScriptable _sceneData;
    private Transform _transform;

    [Inject]
    private void InjectDependencies(SceneDataScriptable sceneData)
    {
        _sceneData = sceneData;
    }
    
    
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
        var direction = _bulletDirection == BulletDirection.Up ? 1.0f : -1.0f;
        _transform.Translate(new Vector3(.0f, direction * Time.deltaTime * _speed ,.0f));

        var bulletPosition = _transform.position;
        var isBulletOutOfBounds = bulletPosition.y > _sceneData.TopBound ||
                                  bulletPosition.y < _sceneData.BottomBound;
        if (isBulletOutOfBounds)
            Destroy();
    }

    public void Destroy()
    {
        gameObject.SetActive(false); 
    }

}
