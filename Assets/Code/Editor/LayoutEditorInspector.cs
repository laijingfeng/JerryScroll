using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Jerry
{
    [CustomEditor(typeof(LayoutEditor))]
    public class LayoutEditorInspector : Editor
    {
        protected LayoutEditor info;

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            info = target as LayoutEditor;

            info.config.dir = (GridLayoutGroup.Axis)EditorGUILayout.EnumPopup(new GUIContent("滑动方向", "横向或竖向"), info.config.dir);

            info.config.prefab = EditorGUILayout.ObjectField(new GUIContent("元素预设", "创建元素的模版"), info.config.prefab, typeof(Transform), true) as Transform;
            info.config.cellSize = EditorGUILayout.Vector2Field(new GUIContent("预设大小", "创建的元素将设置为这个大小"), info.config.cellSize);

            if (info.config.dir == GridLayoutGroup.Axis.Horizontal)
            {
                info.config.dirCellWidth = EditorGUILayout.IntField(new GUIContent("排版行数", "排版行数"), info.config.dirCellWidth);
            }
            else
            {
                info.config.dirCellWidth = EditorGUILayout.IntField(new GUIContent("排版列数", "排版列数"), info.config.dirCellWidth);
            }

            if (info.config.dir == GridLayoutGroup.Axis.Horizontal)
            {
                info.config.dirViewLen = EditorGUILayout.FloatField(new GUIContent("可视宽度", "可视区域宽度，一般是Scroll View的Width"), info.config.dirViewLen);
            }
            else
            {
                info.config.dirViewLen = EditorGUILayout.FloatField(new GUIContent("可视高度", "可视区域高度，一般是Scroll View的Height"), info.config.dirViewLen);
            }

            info.config.spacing = EditorGUILayout.Vector2Field(new GUIContent("元素间隔", "两个元素的间隔"), info.config.spacing);

            GUILayout.Space(10);

            GUI.color = Color.green;
            info.editorCreateCnt = EditorGUILayout.IntField(new GUIContent("创建若干元素看排版效果", "提交前请设置为0"), info.editorCreateCnt);
            GUI.color = Color.white;

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
            {
                info.DoModify();
            }
        }
    }
}