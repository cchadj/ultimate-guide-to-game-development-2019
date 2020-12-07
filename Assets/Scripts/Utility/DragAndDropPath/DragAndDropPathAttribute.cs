using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class DragAndDropPathAttribute : PropertyAttribute
{
        public string FieldPath;

        public bool AcceptFolder { get; set; } = true;
        
        public bool AcceptFile { get; set; } = true;
}

