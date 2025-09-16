// ******************************************************************
//@file         CustomFillOriginImage.cs
//@brief        自定义起始填充角度的image
//@author       yufulao, yufulao@qq.com
//@createTime   2024.07.22 01:24:49
// ******************************************************************

using UnityEngine;
using UnityEngine.UI;

namespace Yu
{
    public class CustomFillOriginImage : Image
    {
        [SerializeField] private float customFillOrigin = 0f; // 自定义起始角度，范围0-360
        [SerializeField] private float customFillMaxAngle = 360f; // 自定义最大角度，范围0-360
        [SerializeField] private bool clockwise = true; // 填充方向：顺时针或逆时针
        [SerializeField] private float lineLength = 100f; // 线段长度

        public float CustomFillOrigin
        {
            get => customFillOrigin;
            set
            {
                customFillOrigin = Mathf.Clamp(value, 0f, 360f);
                SetVerticesDirty();
            }
        }

        public float CustomFillMaxAngle
        {
            get => customFillMaxAngle;
            set
            {
                customFillMaxAngle = Mathf.Clamp(value, 0f, 360f);
                SetVerticesDirty();
            }
        }

        public bool Clockwise
        {
            get => clockwise;
            set
            {
                clockwise = value;
                SetVerticesDirty();
            }
        }

        public float LineLength
        {
            get => lineLength;
            set
            {
                lineLength = Mathf.Max(0f, value);
                SetVerticesDirty();
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            vh.Clear();
            if (overrideSprite == null)
            {
                return;
            }

            var rect = GetPixelAdjustedRect();
            var v = new Vector4(rect.x, rect.y, rect.x + rect.width, rect.y + rect.height);
            var center = new Vector2((v.x + v.z) / 2, (v.y + v.w) / 2);
            var radius = (v.z - v.x) / 2;
            var startAngle = customFillOrigin * Mathf.Deg2Rad;
            startAngle -= Mathf.PI / 2;
            vh.AddVert(center, color, new Vector2(0.5f, 0.5f));
            var fillAmountInDegrees = customFillMaxAngle * fillAmount;
            var steps = Mathf.CeilToInt(fillAmountInDegrees);
            if (clockwise)
            {
                for (var i = 0; i <= steps; i++)
                {
                    var currentAngle = startAngle + (i * Mathf.Deg2Rad);
                    var dir = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle));
                    var pos = center + dir * radius;
                    var uv = new Vector2(0.5f + dir.x * 0.5f, 0.5f + dir.y * 0.5f);
                    vh.AddVert(pos, color, uv);
                }
            }
            else
            {
                for (var i = 0; i <= steps; i++)
                {
                    var currentAngle = startAngle - (i * Mathf.Deg2Rad);
                    var dir = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle));
                    var pos = center + dir * radius;
                    var uv = new Vector2(0.5f + dir.x * 0.5f, 0.5f + dir.y * 0.5f);
                    vh.AddVert(pos, color, uv);
                }
            }

            for (var i = 1; i < vh.currentVertCount - 1; i++)
            {
                vh.AddTriangle(0, i, i + 1);
            }
        }
    }
}