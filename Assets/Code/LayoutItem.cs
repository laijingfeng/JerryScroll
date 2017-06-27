using UnityEngine;

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
}

public class DiffUtil
{
    public static bool DiffStr(string a, string b)
    {
        if (a == b || string.IsNullOrEmpty(a))
        {
            return false;
        }
        return !a.Equals(b);
    }
}