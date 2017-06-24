using UnityEngine;
using System.Collections;

public class InfinitelyLinearGridLayoutGroup : MonoBehaviour
{
    public Dir dir = Dir.Horizontal;
    public Vector2 cellSize = new Vector2(100f, 100f);
    public float spacing = 10f;

    void Awake()
    {

    }

    void Start()
    {

    }

    void Update()
    {

    }

    public enum Dir
    {
        Horizontal = 0,
        Vertical,
    }
}