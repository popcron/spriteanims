using System.Collections.Generic;
using UnityEditor;

namespace Popcron.Animations
{
    public class EditorGUILayoutExtra
    {
        public static void Popup(SerializedProperty property, List<string> options)
        {
            Popup(property, options.ToArray());
        }

        public static void Popup(SerializedProperty property, string[] options)
        {
            //fill in the original data and check for index too
            int selectedIndex = options.Length;
            string[] displayedOptions = new string[options.Length + 1];
            for (int i = 0; i < options.Length; i++)
            {
                displayedOptions[i] = options[i];
                if (displayedOptions[i] == property.stringValue)
                {
                    selectedIndex = i;
                }
            }

            //last index is None
            displayedOptions[options.Length] = "None";

            int newIndex = EditorGUILayout.Popup(property.displayName, selectedIndex, displayedOptions);
            if (newIndex != selectedIndex)
            {
                property.stringValue = displayedOptions[newIndex];
            }
        }
    }
}