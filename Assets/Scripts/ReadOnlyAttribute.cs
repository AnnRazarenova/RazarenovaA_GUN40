using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts
{
    internal class ReadOnlyAttribute : PropertyAttribute { }

#if UNITY_EDITOR

    [UnityEditor.CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyPropertyDrawer : UnityEditor.PropertyDrawer
    {
        //Для старой отриовки через IMGUI
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            UnityEditor.EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var element = base.CreatePropertyGUI(property)
                ?? new UnityEditor.UIElements.PropertyField(property);
            
            element.SetEnabled(false);
            return element;
        }
    }

#endif
}
