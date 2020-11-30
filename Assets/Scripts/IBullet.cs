using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBullet
{
    BulletTypeScriptable BulletTypeData { get; }
    BulletType BulletType { get; }
}
