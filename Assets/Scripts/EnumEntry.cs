using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EnumEntry/EnumEntry")]
public class EnumEntry : ScriptableObject, IEnumEntry
{
    [field: SerializeField] public string EnumEntryName { get; private set; }
}
