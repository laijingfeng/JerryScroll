using UnityEngine;

public interface ILayoutItemData
{
}

public interface ILayoutItem
{
    void SetGridIdx(int idx, Vector3 localPos, ILayoutItemData data);
    int GetGridIdx();

    void SetGridState(bool state);
    bool GetGridState();
}

public abstract class LayoutItem : MonoBehaviour, ILayoutItem
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