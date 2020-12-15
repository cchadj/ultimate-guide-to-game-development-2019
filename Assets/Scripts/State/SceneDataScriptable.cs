using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Zenject;

[CreateAssetMenu]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
[SuppressMessage("ReSharper", "Unity.RedundantAttributeOnTarget")]
public class SceneDataScriptable : ScriptableObject
{
	[field: SerializeField] public float TopBound { get; private set; }
	[field: SerializeField] public float RightBound { get; private set; }
	[field: SerializeField] public float BottomBound { get; private set; }
	[field: SerializeField] public float LeftBound { get; private set; }

	[Inject]
	private void InjectDependencies(Camera mainCamera)
	{
		var halfHeight = mainCamera.orthographicSize;
		var height = 2 * halfHeight;
		// aspect = width / height <=> width = height * aspect;
		var width =  height * mainCamera.aspect;
		var halfWidth = width * .5f;

		// Bounds assume camera is centered at origin
		TopBound = halfHeight;
		BottomBound = -halfHeight;
		LeftBound = -halfWidth;
		RightBound = halfWidth;
	}
}
