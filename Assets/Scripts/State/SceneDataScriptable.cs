using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

[CreateAssetMenu]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
[SuppressMessage("ReSharper", "Unity.RedundantAttributeOnTarget")]
public class SceneDataScriptable : ScriptableObject
{
	[field: SerializeField] public float TopBound { get; private set; }
	[field: SerializeField] public float RightBound { get; private set; }
	[field: SerializeField] public float BottomBound { get; private set; }
	[field: SerializeField] public float LeftBound { get; private set; }
}
