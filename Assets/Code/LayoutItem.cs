using UnityEngine;

public interface LayoutItem
{
    void SetGridIdx(int idx, Vector3 localPos);
    int GetGridIdx();

    void SetGridState(bool state);
    bool GetGridState();
}