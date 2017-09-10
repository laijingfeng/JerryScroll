using Jerry;
using UnityEngine;
using UnityEngine.UI;

public class LayoutEditor : MonoBehaviour
{
    public LayoutConfig config = new LayoutConfig();

#if UNITY_EDITOR

    [Tooltip("创建若干个元素预览，提交前请置为0")]
    public int editorCreateCnt = 0;

    private RectTransform rectTran;

    public void DoModify()
    {
        TryWork();

        if (config.prefab == null)
        {
            return;
        }

        for (int i = 0; i < editorCreateCnt; i++)
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

    private Vector3 Idx2LocalPos(int idx)
    {
        Vector3 ret = Vector3.zero;
        int gridX = 0, gridY = 0;
        switch (config.dir)
        {
            case GridLayoutGroup.Axis.Horizontal:
                {
                    gridX = idx / config.dirCellWidth;
                    gridY = idx % config.dirCellWidth;
                }
                break;
            case GridLayoutGroup.Axis.Vertical:
                {
                    gridX = idx % config.dirCellWidth;
                    gridY = idx / config.dirCellWidth;
                }
                break;
        }
        ret.x += gridX * (config.spacing.x + config.cellSize.x);
        ret.y -= gridY * (config.spacing.y + config.cellSize.y);
        return ret;
    }

    private void TryWork()
    {
        rectTran = this.transform as RectTransform;
        switch (config.dir)
        {
            case GridLayoutGroup.Axis.Horizontal:
                {
                    (rectTran.parent as RectTransform).pivot = new Vector2(0, 0.5f);
                    rectTran.anchorMin = new Vector2(0, 0.5f);
                    rectTran.anchorMax = new Vector2(0, 0.5f);
                }
                break;
            case GridLayoutGroup.Axis.Vertical:
                {
                    (rectTran.parent as RectTransform).pivot = new Vector2(0.5f, 1.0f);
                    rectTran.anchorMin = new Vector2(0.5f, 1.0f);
                    rectTran.anchorMax = new Vector2(0.5f, 1.0f);
                }
                break;
        }
        rectTran.pivot = new Vector2(0, 1);

        JerryUtil.DestroyAllChildren(this.transform, true);

        ResetPos();
        ResetDelta();
    }

    /// <summary>
    /// 方向总长度（Item数）
    /// </summary>
    private int DirLen
    {
        get
        {
            return (editorCreateCnt / config.dirCellWidth) + (editorCreateCnt % config.dirCellWidth == 0 ? 0 : 1);
        }
    }

    private void ResetPos()
    {
        Vector3 pos = Vector3.zero;
        float dirLen = DirLen * 1.0f;
        float fDirLen = 0;
        switch (config.dir)
        {
            case GridLayoutGroup.Axis.Horizontal:
                {
                    fDirLen = config.cellSize.x * dirLen;
                    if (dirLen > 1)
                    {
                        fDirLen += config.spacing.x * (dirLen - 1);
                    }
                    fDirLen -= config.dirViewLen;
                    if (fDirLen < 0)
                    {
                        fDirLen = 0;
                    }
                    pos.x -= fDirLen * config.progress;

                    pos.y += config.cellSize.y * config.dirCellWidth + config.spacing.y * (config.dirCellWidth - 1);
                    pos.y *= 0.5f;
                }
                break;
            case GridLayoutGroup.Axis.Vertical:
                {
                    pos.x -= config.cellSize.x * config.dirCellWidth + config.spacing.x * (config.dirCellWidth - 1);
                    pos.x *= 0.5f;

                    fDirLen = config.cellSize.y * dirLen;
                    if (dirLen > 1)
                    {
                        fDirLen += config.spacing.y * (dirLen - 1);
                    }
                    fDirLen -= config.dirViewLen;
                    if (fDirLen < 0)
                    {
                        fDirLen = 0;
                    }
                    pos.y += fDirLen * config.progress;
                }
                break;
        }
        this.transform.localPosition = pos;
    }

    private void ResetDelta()
    {
        Vector2 size = config.cellSize * config.dirCellWidth;
        int dirLen = DirLen;
        switch (config.dir)
        {
            case GridLayoutGroup.Axis.Horizontal:
                {
                    size.x = dirLen * config.cellSize.x;
                    if (dirLen > 0)
                    {
                        size.x += (dirLen - 1) * config.spacing.x;
                    }

                    if (config.dirCellWidth > 1)
                    {
                        size.y += (config.dirCellWidth - 1) * config.spacing.y;
                    }
                }
                break;
            case GridLayoutGroup.Axis.Vertical:
                {
                    if (config.dirCellWidth > 1)
                    {
                        size.x += (config.dirCellWidth - 1) * config.spacing.x;
                    }

                    size.y = dirLen * config.cellSize.y;
                    if (dirLen > 0)
                    {
                        size.y += (dirLen - 1) * config.spacing.y;
                    }
                }
                break;
        }
        rectTran.sizeDelta = size;
    }
#endif
}