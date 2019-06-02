using System.Collections.Generic;
using UnityEditor;

namespace Popcron.Animations
{
    [CustomEditor(typeof(SpriteAnimator))]
    public class SpriteAnimatorInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            SpriteAnimator animator = (SpriteAnimator)target;

            //draw the default animation field
            SerializedProperty defaultAnimation = serializedObject.FindProperty("defaultAnimation");
            List<string> options = new List<string>();
            for (int i = 0; i < animator.Animations.Count; i++)
            {
                options.Add(animator.Animations[i].Name);
            }

            EditorGUILayoutExtra.Popup(defaultAnimation, options);

            //draw the array of animations
            SerializedProperty animations = serializedObject.FindProperty("animations");
            EditorGUILayout.PropertyField(animations, true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}