﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Fraktalia.Core.Mathematics.Editor
{
    [CustomPropertyDrawer(typeof(Fbool2x2)), CustomPropertyDrawer(typeof(Fbool2x3)), CustomPropertyDrawer(typeof(Fbool2x4))]
    [CustomPropertyDrawer(typeof(Fbool3x2)), CustomPropertyDrawer(typeof(Fbool3x3)), CustomPropertyDrawer(typeof(Fbool3x4))]
    [CustomPropertyDrawer(typeof(Fbool4x2)), CustomPropertyDrawer(typeof(Fbool4x3)), CustomPropertyDrawer(typeof(Fbool4x4))]
    [CustomPropertyDrawer(typeof(Fdouble2x2)), CustomPropertyDrawer(typeof(Fdouble2x3)), CustomPropertyDrawer(typeof(Fdouble2x4))]
    [CustomPropertyDrawer(typeof(Fdouble3x2)), CustomPropertyDrawer(typeof(Fdouble3x3)), CustomPropertyDrawer(typeof(Fdouble3x4))]
    [CustomPropertyDrawer(typeof(Fdouble4x2)), CustomPropertyDrawer(typeof(Fdouble4x3)), CustomPropertyDrawer(typeof(Fdouble4x4))]
    [CustomPropertyDrawer(typeof(Ffloat2x2)), CustomPropertyDrawer(typeof(Ffloat2x3)), CustomPropertyDrawer(typeof(Ffloat2x4))]
    [CustomPropertyDrawer(typeof(Ffloat3x2)), CustomPropertyDrawer(typeof(Ffloat3x3)), CustomPropertyDrawer(typeof(Ffloat3x4))]
    [CustomPropertyDrawer(typeof(Ffloat4x2)), CustomPropertyDrawer(typeof(Ffloat4x3)), CustomPropertyDrawer(typeof(Ffloat4x4))]
    [CustomPropertyDrawer(typeof(Fint2x2)), CustomPropertyDrawer(typeof(Fint2x3)), CustomPropertyDrawer(typeof(Fint2x4))]
    [CustomPropertyDrawer(typeof(Fint3x2)), CustomPropertyDrawer(typeof(Fint3x3)), CustomPropertyDrawer(typeof(Fint3x4))]
    [CustomPropertyDrawer(typeof(Fint4x2)), CustomPropertyDrawer(typeof(Fint4x3)), CustomPropertyDrawer(typeof(Fint4x4))]
    [CustomPropertyDrawer(typeof(Fuint2x2)), CustomPropertyDrawer(typeof(Fuint2x3)), CustomPropertyDrawer(typeof(Fuint2x4))]
    [CustomPropertyDrawer(typeof(Fuint3x2)), CustomPropertyDrawer(typeof(Fuint3x3)), CustomPropertyDrawer(typeof(Fuint3x4))]
    [CustomPropertyDrawer(typeof(Fuint4x2)), CustomPropertyDrawer(typeof(Fuint4x3)), CustomPropertyDrawer(typeof(Fuint4x4))]
    class FMatrixDrawer : PropertyDrawer
    {
        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
                return EditorGUIUtility.singleLineHeight;
            var rows = 1 + property.type[property.type.Length - 3] - '0';
            return rows * EditorGUIUtility.singleLineHeight + (rows - 1) * EditorGUIUtility.standardVerticalSpacing;
        }

        static ReadOnlyCollection<string> k_ColPropertyPaths =
            new ReadOnlyCollection<string>(new[] { "c0", "c1", "c2", "c3" });
        static ReadOnlyCollection<string> k_RowPropertyPaths =
            new ReadOnlyCollection<string>(new[] { "x", "y", "z", "w" });

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, property, label, false);

            if (Event.current.type == EventType.ContextClick && position.Contains(Event.current.mousePosition))
            {
                DoUtilityMenu(property);
                Event.current.Use();
            }

            if (!property.isExpanded)
                return;

            var rows = property.type[property.type.Length - 3] - '0';
            var cols = property.type[property.type.Length - 1] - '0';

            ++EditorGUI.indentLevel;
            position = EditorGUI.IndentedRect(position);
            --EditorGUI.indentLevel;

            var elementType = property.FindPropertyRelative("c0.x").propertyType;
            for (var row = 0; row < rows; ++row)
            {
                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                var elementRect = new Rect(position)
                {
                    width = elementType == SerializedPropertyType.Boolean
                        ? EditorGUIUtility.singleLineHeight
                        : (position.width - (cols - 1) * EditorGUIUtility.standardVerticalSpacing) / cols
                };
                for (var col = 0; col < cols; ++col)
                {
                    EditorGUI.PropertyField(
                        elementRect,
                        property.FindPropertyRelative($"{k_ColPropertyPaths[col]}.{k_RowPropertyPaths[row]}"),
                        GUIContent.none
                    );
                    elementRect.x += elementRect.width + EditorGUIUtility.standardVerticalSpacing;
                }
            }
        }

        Dictionary<SerializedPropertyType, Action<SerializedProperty, bool>> k_UtilityValueSetters =
            new Dictionary<SerializedPropertyType, Action<SerializedProperty, bool>>
            {
                { SerializedPropertyType.Boolean, (property, b) => property.boolValue = b },
                { SerializedPropertyType.Float, (property, b) => property.floatValue = b ? 1f : 0f },
                { SerializedPropertyType.Integer, (property, b) => property.intValue = b ? 1 : 0 }
            };

        void DoUtilityMenu(SerializedProperty property)
        {
            var rows = property.type[property.type.Length - 3] - '0';
            var cols = property.type[property.type.Length - 1] - '0';
            var elementType = property.FindPropertyRelative("c0.x").propertyType;
            var setValue = k_UtilityValueSetters[elementType];
            var menu = new GenericMenu();
            property = property.Copy();
            menu.AddItem(
                EditorGUIUtility.TrTextContent("Set to Zero"),
                false,
                () =>
                {
                    property.serializedObject.Update();;
                    for (var row = 0; row < rows; ++row)
                    for (var col = 0; col < cols; ++col)
                        setValue(
                            property.FindPropertyRelative($"{k_ColPropertyPaths[col]}.{k_RowPropertyPaths[row]}"),
                            false
                        );
                    property.serializedObject.ApplyModifiedProperties();
                }
            );
            if (rows == cols)
            {
                menu.AddItem(
                    EditorGUIUtility.TrTextContent("Reset to Identity"),
                    false,
                    () =>
                    {
                        property.serializedObject.Update();
                        for (var row = 0; row < rows; ++row)
                        for (var col = 0; col < cols; ++col)
                            setValue(
                                property.FindPropertyRelative($"{k_ColPropertyPaths[col]}.{k_RowPropertyPaths[row]}"),
                                row == col
                            );
                        property.serializedObject.ApplyModifiedProperties();
                    }
                );
            }
            menu.ShowAsContext();
        }
    }
}
#endif