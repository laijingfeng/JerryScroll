using UnityEditor;
using UnityEngine;

namespace Jerry
{
    [CustomEditor(typeof(LayoutEditor))]
    public class LayoutEditorInspector : Editor
    {
        protected LayoutEditor info;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUI.changed)
            {
                info = target as LayoutEditor;
                info.DoModify();
            }
        }
    }
}