using UnityEngine;
using UnityEngine.UI;

public interface ILayoutItemData
{
}

public abstract class LayoutItem : MonoBehaviour
{
    private int gridIdx;
    private bool gridInUse;

    public void SetGridHide()
    {
        gridIdx = -1;
        if (this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(false);
        }
    }

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

    public void SetGridState(bool inUse)
    {
        gridInUse = inUse;
        if (gridInUse == true && !this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(true);
        }
    }

    public bool GetGridState()
    {
        return gridInUse;
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

    /// <summary>
    /// dir方向元素扩展
    /// </summary>
    public int dirCellWidth = 1;

    /// <summary>
    /// 可视区域dir方向长度，用来算进度
    /// </summary>
    public float dirViewLen = 1f;

    public Vector2 cellSize = new Vector2(100f, 100f);

    public Vector2 spacing = new Vector2(10, 10);

    /// <summary>
    /// 两端各额外缓存的行数或列数
    /// </summary>
    public int bufHalfCnt = 0;

    /// <summary>
    /// 进度
    /// </summary>
    public float progress = 0;

    /// <summary>
    /// 一帧工作数量，0无限
    /// </summary>
    public int frameWorkCnt = 0;
}

/// <summary>
/// <para>修改配置</para>
/// <para>重刷数据的时候，可以修改部分配置</para>
/// <para>特别是想复用之前的Prefab，有些细微配置不一样，可以用RefreshDatas而不用DoInit</para>
/// </summary>
public class ModifyConfig
{
    public float? progress = null;
    public Vector2? spacing = null;

    public bool HaveChange(float curProgress, Vector2 curSpacing)
    {
        bool ret = false;
        if (ret == false
            && progress.HasValue
            && !Mathf.Approximately(progress.Value, curProgress))
        {
            ret = true;
        }
        if (ret == false
            && spacing.HasValue
            && !Vector2.Equals(spacing.Value, curSpacing))
        {
            ret = true;
        }
        return ret;
    }
}