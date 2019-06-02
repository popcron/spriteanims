using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Popcron.Animations
{
    [CustomPropertyDrawer(typeof(SpriteAnimationClip))]
    public class SpriteAnimationClipInspector : PropertyDrawer
    {
        private static Dictionary<int, ReorderableList> list = null;
        private bool initialized = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!initialized)
            {
                initialized = true;
                Initialize(property);
            }

            SerializedProperty name = property.FindPropertyRelative("name");
            SerializedProperty frames = property.FindPropertyRelative("frames");
            SerializedProperty loop = property.FindPropertyRelative("loop");

            position.height = 17;
            EditorGUI.PropertyField(position, frames, label);
            if (frames.isExpanded)
            {
                EditorGUI.indentLevel++;
                position.y += 18;
                EditorGUI.PropertyField(position, name, new GUIContent("Name"));

                position.y += 18;
                EditorGUI.PropertyField(position, loop, new GUIContent("Loop"));

                position.y += 18;

                int id = GetID(property);
                if (list.ContainsKey(id))
                {
                    list[id].DoList(position);
                }
                else
                {
                    Initialize(property);
                }

                EditorGUI.indentLevel--;
            }
        }

        private int GetID(SerializedProperty property)
        {
            int id = property.serializedObject.targetObject.GetHashCode();
            int pathId = property.propertyPath.GetHashCode();

            return (((id << 21) + id) ^ pathId);
        }

        private void Initialize(SerializedProperty property)
        {
            if (list == null)
            {
                list = new Dictionary<int, ReorderableList>();
            }

            int id = GetID(property);
            SerializedProperty frames = property.FindPropertyRelative("frames");
            list[id] = ReorderableListUtility.CreateAutoLayout(frames);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int h = 18;
            SerializedProperty frames = property.FindPropertyRelative("frames");
            if (frames.isExpanded)
            {
                h += 18;
                h += 18;
                h += 18;
                h += 20 * Mathf.Max(frames.arraySize + 1, 2);
            }
            return h;
        }
    }
}