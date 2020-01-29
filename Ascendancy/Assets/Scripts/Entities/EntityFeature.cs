﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public abstract class EntityFeature : ScriptableObject
{
    /// <summary>
    /// The order in which the features are ranked when a click is registered. Higher values go first.
    /// </summary>
    public int clickPriority;

    public Entity entity { get; private set; }

    public virtual void Initialize(Entity entity)
    {
        this.entity = entity;
    }

    public virtual void UpdateOverride()
    {

    }

    public virtual void Update10Override()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="hit"></param>
    /// <param name="enqueue"></param>
    /// <returns>True if order was successfull, false otherwise.</returns>
    public virtual bool ClickOrder(RaycastHit hit, bool enqueue = false)
    {
        return true;
    }

    #if UNITY_EDITOR
    public void DrawLayout()
    {
        EditorGUILayout.LabelField(this.GetType().ToString(), EditorStyles.boldLabel);

        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty list = serializedObject.FindProperty("test");
        
        EditorGUI.BeginChangeCheck();

        SerializedProperty prop = serializedObject.GetIterator();

        if (prop.NextVisible(true))
        {
            do
                EditorGUILayout.PropertyField(serializedObject.FindProperty(prop.name), true);
            while (prop.NextVisible(false));

        }
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
    }
    #endif
}
