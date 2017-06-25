using UnityEngine;
using System.Collections;
using Jerry;

public class InfinitelyLinearGridLayoutGroup : MonoBehaviour
{
    [Header("Settings")]

    public Dir dir = Dir.Horizontal;
    public Vector2 cellSize = new Vector2(100f, 100f);
    public float spacing = 10f;

    [Header("Init")]
    public Transform prefab;
    public int totalAmt;

    private RectTransform rectTran;

    void Awake()
    {
        rectTran = this.transform as RectTransform;
        Refresh();
    }

    [ContextMenu("重建数据")]
    private void Refresh()
    {
        JerryUtil.DestroyAllChildren(this.transform);
        ResetDelta();
        for (int i = 0; i < totalAmt; i++)
        {
            GameObject go = JerryUtil.CloneGo(new JerryUtil.CloneGoData()
            {
                name = i.ToString(),
                parant = this.transform,
                prefab = prefab.gameObject,
                active = true,
            });
            go.transform.localPosition = new Vector3(i * cellSize.x + i * spacing, 0, 0);
        }
    }

    private void ResetDelta()
    {
        Vector2 size = cellSize;
        switch (dir)
        {
            case Dir.Horizontal:
                {
                    size.x = totalAmt * cellSize.x;
                    if (totalAmt > 0)
                    {
                        size.x += (totalAmt - 1) * spacing;
                    }
                }
                break;
            case Dir.Vertical:
                {
                    size.y = totalAmt * cellSize.y;
                    if (totalAmt > 0)
                    {
                        size.y += (totalAmt - 1) * spacing;
                    }
                }
                break;
        }
        rectTran.sizeDelta = size;
    }

    public enum Dir
    {
        Horizontal = 0,
        Vertical,
    }
}