// ****************************************************************** 
//@file         RadarChart.cs
//@brief        自定义的雷达图UI
//@author       yufulao, yufulao@qq.com
//@createTime   2025.01.14 17:41:48
// ******************************************************************

using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(CanvasRenderer))]
public class RadarChart : MaskableGraphic
{
    [SerializeField] private bool showOutline; //是否显示外轮廓
    [SerializeField] private Color outlineColor; //外轮廓颜色
    [SerializeField] private float outlineWidth; //外轮廓宽度
    [SerializeField] private bool showInline; //是否显示内轮廓
    [SerializeField] private Color inlineColor; //内轮廓颜色
    [SerializeField] private float inlineWidth; //内轮廓宽度
    [SerializeField] private bool showCenterPoint; //是否显示中心点
    [SerializeField] private Color centerPointColor; //中心点颜色
    [SerializeField] private float centerPointRadius; //中心点半径
    [SerializeField] private RadarData[] dataArray; //线段数据


    [System.Serializable]
    public class RadarData
    {
        [Range(0f, 1f)] public float value = 1f; //当前值
        public float angle; //区块角度
        public float maxLength = 40f; //线段总长度
    }

    /// <summary>
    /// 重绘
    /// </summary>
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        if (dataArray == null || dataArray.Length < 3)
        {
            return;
        }

        var center = Vector2.zero;
        var vertices = new Vector2[dataArray.Length];
        for (var i = 0; i < dataArray.Length; i++)
        {
            var radian = dataArray[i].angle * Mathf.Deg2Rad;
            var distance = Mathf.Lerp(0, dataArray[i].maxLength, dataArray[i].value);
            vertices[i] = new Vector2(Mathf.Cos(radian) * distance, Mathf.Sin(radian) * distance);
        }

        for (var i = 0; i < dataArray.Length; i++) //填充区域
        {
            var nextIndex = (i + 1) % dataArray.Length;
            AddTriangle(vh, center, vertices[i], vertices[nextIndex], color);
        }

        if (showOutline) //绘制外轮廓
        {
            DrawOuterOutline(vh, vertices, outlineColor, outlineWidth);
        }

        if (showInline) //绘制内部边
        {
            for (var i = 0; i < dataArray.Length; i++)
            {
                DrawLine(vh, center, vertices[i], inlineColor, inlineWidth);
            }
        }

        if (showCenterPoint) //绘制中心点
        {
            DrawCenterPoint(vh, center, centerPointColor, centerPointRadius);
        }
    }

    /// <summary>
    /// 刷新数据
    /// </summary>
    public void RefreshRadarData(RadarData[] newData)
    {
        if (newData == null || newData.Length < 3)
        {
            Debug.LogError("非法数据");
            return;
        }

        dataArray = newData;
        SetVerticesDirty();
    }

    /// <summary>
    /// 刷新数据
    /// </summary>
    public void RefreshRadarData(int index, float value)
    {
        if (index >= dataArray.Length)
        {
            Debug.LogError("越界");
            return;
        }

        var data = dataArray[index];
        data.value = value;
        SetVerticesDirty();
    }

    /// <summary>
    /// 添加三角形
    /// </summary>
    private static void AddTriangle(VertexHelper vh, Vector2 v0, Vector2 v1, Vector2 v2, Color color)
    {
        var vert0 = UIVertex.simpleVert;
        var vert1 = UIVertex.simpleVert;
        var vert2 = UIVertex.simpleVert;
        vert0.color = color;
        vert1.color = color;
        vert2.color = color;
        vert0.position = v0;
        vert1.position = v1;
        vert2.position = v2;
        vh.AddVert(vert0);
        vh.AddVert(vert1);
        vh.AddVert(vert2);

        var index = vh.currentVertCount;
        vh.AddTriangle(index - 3, index - 2, index - 1);
    }

    /// <summary>
    /// 绘制外轮廓
    /// </summary>
    private static void DrawOuterOutline(VertexHelper vh, Vector2[] vertices, Color colorT, float width)
    {
        for (var i = 0; i < vertices.Length; i++)
        {
            var nextIndex = (i + 1) % vertices.Length;
            DrawLine(vh, vertices[i], vertices[nextIndex], colorT, width);
        }
    }

    /// <summary>
    /// 绘制线段
    /// </summary>
    private static void DrawLine(VertexHelper vh, Vector2 start, Vector2 end, Color colorT, float width)
    {
        var direction = (end - start).normalized;
        var perpendicular = new Vector2(-direction.y, direction.x) * width * 0.5f;

        var v0 = start - perpendicular;
        var v1 = start + perpendicular;
        var v2 = end + perpendicular;
        var v3 = end - perpendicular;
        var vert0 = UIVertex.simpleVert;
        var vert1 = UIVertex.simpleVert;
        var vert2 = UIVertex.simpleVert;
        var vert3 = UIVertex.simpleVert;
        vert0.color = colorT;
        vert1.color = colorT;
        vert2.color = colorT;
        vert3.color = colorT;
        vert0.position = v0;
        vert1.position = v1;
        vert2.position = v2;
        vert3.position = v3;
        vh.AddVert(vert0);
        vh.AddVert(vert1);
        vh.AddVert(vert2);
        vh.AddVert(vert3);

        var index = vh.currentVertCount;
        vh.AddTriangle(index - 4, index - 3, index - 2);
        vh.AddTriangle(index - 4, index - 2, index - 1);
    }

    /// <summary>
    /// 绘制中心点
    /// </summary>
    private static void DrawCenterPoint(VertexHelper vh, Vector2 center, Color color, float radius)
    {
        var prevPoint = center + new Vector2(radius, 0);
        for (var i = 1; i <= 20; i++)
        {
            var angle = i * 20 * Mathf.Deg2Rad; //20段画圆
            var nextPoint = center + new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            AddTriangle(vh, center, prevPoint, nextPoint, color);
            prevPoint = nextPoint;
        }
    }
}