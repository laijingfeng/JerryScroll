﻿using Jerry;
using UnityEngine;
using UnityEngine.UI;

public class LayoutEditor : InfinitelyGridLayoutGroup<LayoutItem, ILayoutItemData>
{
    public int editorCreateCnt = 1;

    [ContextMenu("创建元素")]
    protected void CreateElementsForEditor()
    {
        awaked = true;
        inited = true;
        TryWork();

        for (int i = 0; i < TotalCount; i++)
        {
            GameObject go = JerryUtil.CloneGo(new JerryUtil.CloneGoData()
            {
                name = i.ToString(),
                parant = this.transform,
                prefab = config.prefab.gameObject,
                active = true,
            });

            switch (config.dir)
            {
                case GridLayoutGroup.Axis.Horizontal:
                    {
                        (go.transform as RectTransform).anchorMin = new Vector2(0.5f, 0.5f);
                        (go.transform as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);
                    }
                    break;
                case GridLayoutGroup.Axis.Vertical:
                    {
                        (go.transform as RectTransform).anchorMin = new Vector2(0.5f, 0.5f);
                        (go.transform as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);
                    }
                    break;
            }
            (go.transform as RectTransform).pivot = new Vector2(0, 1);
            (go.transform as RectTransform).sizeDelta = config.cellSize;
            go.transform.localPosition = Idx2LocalPos(i);
        }
    }

    [ContextMenu("输出当前进度")]
    protected void PrintProgress()
    {
        Debug.LogWarning("进度:" + CurProgress());
    }

    protected override int TotalCount
    {
        get
        {
            return editorCreateCnt;
        }
    }

    protected override bool IsEditorMode
    {
        get
        {
            return true;
        }
    }
}