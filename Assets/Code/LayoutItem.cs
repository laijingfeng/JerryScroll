using UnityEngine;
using UnityEngine.UI;

public interface ILayoutItemData
{
}

public abstract class LayoutItem : MonoBehaviour
{
    private int gridIdx;
    private bool gridState;

    public void SetGridIdx(int idx, Vector3 localPos, ILayoutItemData data)
    {
        gridIdx = idx;
        this.transform.localPosition = localPos;
        TryRefreshUI(data);
        //Debug.LogWarning("xxx " + idx);
    }

    public abstract void TryRefreshUI(ILayoutItemData data);

    public int GetGridIdx()
    {
        return gridIdx;
    }

    public void SetGridState(bool state)
    {
        gridState = state;
    }

    public bool GetGridState()
    {
        return gridState;
    }

    public static bool ValDiff<T>(T a, T b)
    {
        if (a == null)
        {
            return !(b == null);
        }
        //备注：为null的string执行Equals要报错的
        return !a.Equals(b);
    }
}

[System.Serializable]
public class LayoutConfig
{
    public Transform prefab;

    public GridLayoutGroup.Axis dir = GridLayoutGroup.Axis.Horizontal;

    public Vector2 cellSize = new Vector2(100f, 100f);

    public Vector2 spacing = new Vector2(10, 10);

    /// <summary>
    /// 固定行数或列数
    /// </summary>
    public int fixedRowOrColumnCount = 1;

    /// <summary>
    /// 单位宽度可视数量
    /// </summary>
    public float fiexdDirViewCountF = 1f;

    /// <summary>
    /// 额外缓存的行数或列数
    /// </summary>
    public int viewCountHalfBuffer = 0;

    public float startProgress = 0;

    /// <summary>
    /// 0无限
    /// </summary>
    public int workCountPerFrame = 0;
}