﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sample : MonoBehaviour
{
    public GameObject prefab;
    public Transform layoutHContent;
    public Transform layoutVContent;
    public Button genData;
    public Button sortData;
    public Button addData;
    public Button minusData;
    public Text dataNum;

    private List<Item.ItemData> datas = new List<Item.ItemData>();
    private int dataAmt = 0;

    private Layout layoutH;
    private Layout layoutV;

    private bool sortFlag = true;

    void Awake()
    {
        layoutH = layoutHContent.gameObject.AddComponent<Layout>();
        layoutV = layoutVContent.gameObject.AddComponent<Layout>();
        RefreshDataNum();

        sortData.onClick.AddListener(() =>
        {
            if (datas.Count <= 0)
            {
                return;
            }
            if (sortFlag)
            {
                datas.Sort(SortCmp2);
            }
            else
            {
                datas.Sort(SortCmp1);
            }
            sortFlag = !sortFlag;
            layoutH.RefreshDatas();
            layoutV.RefreshDatas();
        });

        genData.onClick.AddListener(() =>
        {
            GenDatas();
            if (!layoutH.Inited)
            {
                layoutH.DoInit(new Layout.ConfigData()
                {
                    startIdx = 0,
                    bufferHalfAmt = 1,
                    cellSize = new Vector2(190, 190),
                    dir = Layout.Dir.Horizontal,
                    oneScreenAmt = 3,
                    prefab = prefab.transform,
                    spacing = 10,
                }, datas);
            }
            else
            {
                layoutH.RefreshDatas(datas);
            }

            if (!layoutV.Inited)
            {
                layoutV.DoInit(new Layout.ConfigData()
                {
                    startIdx = 0,
                    bufferHalfAmt = 1,
                    cellSize = new Vector2(190, 190),
                    dir = Layout.Dir.Vertical,
                    oneScreenAmt = 3,
                    prefab = prefab.transform,
                    spacing = 10,
                }, datas);
            }
            else
            {
                layoutV.RefreshDatas(datas);
            }
        });

        addData.onClick.AddListener(() =>
        {
            if (dataAmt > 100000)
            {
                return;
            }
            if (dataAmt == 0)
            {
                dataAmt++;
            }
            else
            {
                dataAmt *= 2;
            }
            RefreshDataNum();
        });

        minusData.onClick.AddListener(() =>
        {
            dataAmt /= 2;
            RefreshDataNum();
        });
    }

    private static int SortCmp1(Item.ItemData a, Item.ItemData b)
    {
        return a.id > b.id ? 1 : -1;
    }

    private static int SortCmp2(Item.ItemData a, Item.ItemData b)
    {
        return a.id < b.id ? 1 : -1;
    }

    private void RefreshDataNum()
    {
        dataNum.text = dataAmt.ToString();
    }

    #region 随机数据

    private void GenDatas()
    {
        idKey = 0;
        datas.Clear();
        for (int i = 0; i < dataAmt; i++)
        {
            datas.Add(RandomOneData());
        }
    }

    private int idKey = 0;
    private int GenId()
    {
        idKey++;
        return idKey;
    }

    private Item.ItemData RandomOneData()
    {
        Item.ItemData ret = new Item.ItemData();
        ret.id = GenId();
        ret.num = Random.Range(0, 1000);
        ret.head = string.Format("head{0}", Random.Range(0, 3));
        return ret;
    }

    #endregion 随机数据
}