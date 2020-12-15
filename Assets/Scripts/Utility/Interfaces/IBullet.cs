using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBullet
{
    float BulletDamage { get; }
    BulletTypeScriptable BulletTypeData { get; }
    BulletType BulletType { get; }
}
