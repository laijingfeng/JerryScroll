��Ŀ | ����
---|---
���� | JerryScroll
Ŀ¼ | Github/��Ŀ
��ǩ | Github
��ע | [Github](https://github.com/laijingfeng/JerryScroll)
���� | 2017-06-24 09:49:42
���� | 2017-09-25 18:26:09

[TOC]

# ˵��

��UGUI��LayoutGroup���Ż�

# InfinitelyGridLayoutGroup(������Ӳ�����)

ԭ������Grid��С�̶������ԣ�ÿһ��Ԫ����Content��λ���ǿ��Լ���ģ���Content�Ĵ�С����Ϊ�ܴ�С�������Ͳ��ù��Ļ�����ϸ�ڣ��������̸���Contentλ�ü�����Ұ�ڵ�Ԫ�أ�ˢ�±߽��Ԫ�ء��Ӷ�����ͬʱ���ڵ�Ԫ����������Ԫ�صĴ������١�

> ע�⣺
>
> ����ֻ֧���������϶��룬��֧�־��ж���
>
> ���ݵ���һ��������ʱ����10w����Pos��ֵ���ܻ�ǳ��󣬻�Ӱ�컬����Ч�����������Կ���progressΪ0��1ʱ�����ָ��ǲ�һ���ģ����Ըо�����ֵ[����ʱ(spacing+cell)*count]̫����ɵģ���Ȼʵ��ʹ���У����ݵ�1w�����ټ��ٰ�

![image](http://odk2uwdl8.bkt.clouddn.com/2017-09-25_JerryScroll.png)

## ʹ�ã��ο�Sample��

- �½�Ԫ��UI�࣬�̳�LayoutItem����д����ˢ��UI�ĺ���
    - `public override void TryRefreshUI(ILayoutItemData data)`
- �½�Ԫ�������࣬�̳�ILayoutItemData
- �½�һ���µľ���Layout�࣬�̳�`InfinitelyLinearGridLayoutGroup<T, F>`��ָ�������Ԫ��UI�ṹT(LayoutItem)��Ԫ�����ݽṹF(ILayoutItemData)
    - ����Ҫ�������ݣ�ֱ�ӿ�����
    - Ϊʲô��ֱ���ø��ࣿ
        - Unity��֧�ֹ���������Mono
- �ű�������`Scroll View/Viewport/Content`���
    - �󲿷�ʱ�򣬺ܶ�Ĳ�������UIͬѧ��Art����ȥ���ú͵����ģ���ʱ���ڸý���¹���==LayoutEditor==�ű���LayoutConfig�����ʱ���нӿڶ�ȡ��Щ���� 
- ==��ʼ�����ú�����==������˵�������棩
    - `public void DoInit(LayoutConfig tconfig, List<F> tdatas)`
- ==���ݱ��==
    - `public void RefreshDatas(List<F> tdatas = null, ModifyConfig modifyConfig = null)`
    - �µ�����
    - ����΢�����ֲ������ṹ������˵��
- ==��ȡ����==
    - `public float CurProgress()`
    - ������������´λָ�
- �Ż�����
    - Ԫ�����ݽṹ�����۴�����ݣ�����ͷ��/ͼ���Ҫ����ж����Դ����ˢ��UI��ʱ����в���Ƚϣ�ˢ��UI��ʱ���жϲ�����޸�

�������ݣ�LayoutConfig����
- Transform prefab Ԥ��
- LayoutDir dir ��������
- int dirCellWidth �̶��������������������Ż���ţ�
- float dirviewLen �������򳤶ȣ����������
- Vector2 cellSize ��С
- Vector2 spacing ���
- int bufHalfCnt ���߶��⻺�������������Ϊ1
- float progress ��ʼ����
- int frameWorkCnt һ֡�����¼���Ԫ�أ�0������
    - ��һ����ʾ�����ݺܶ࣬��һ�γ�ʼ���Ῠ�٣����������������

����΢����ModifyConfig����
- float? progress
- Vector2? spacing

## �༭������

`LayoutEditor`���ص�Content����ϣ�������Ϣ�����Լ�ʱ����Ч����������

˵����
- ��д��ز���������`EditorCreateCnt`�Ϳ��Դ�����Ӧ������Item��Ԥ��Ч��
- ����ʱ`EditorCreateCnt`��Ч
- ������ɺ�`EditorCreateCnt`�ǵ���Ϊ0

## QA

- Q�����ʵ��Padding��
    - A����Mask��`Viewport`�Ƶ�`Scroll View`������`Viewport`��`RectTransform`��`Left/Right/Top/Bottom`��