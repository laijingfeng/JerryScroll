using UnityEngine;

public class Item : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {
    }

    [ContextMenu("本地坐标")]
    private void Pos()
    {
        Debug.LogWarning(this.transform.localPosition);
    }
}