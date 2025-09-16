// ******************************************************************
//@file         DynamicSorting.cs
//@brief        利用z轴完成2D俯视角伪透视，动态调整场景物体的前后遮挡
//@author       yufulao, yufulao@qq.com
//@createTime   2024.07.02 16:21:45
// ******************************************************************

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace Yu
{
    public class DynamicSorting : MonoBehaviour
    {
        [LabelText("是否是静态物体")] public bool isStatic = true;
        [LabelText("Z轴值偏移量")] public float offset;
        [Title("调试")] [LabelText("是否保持常亮")] public bool debugLineAlwaysActive = false;
        [LabelText("辅助线颜色")] public Color debugLineColor = Color.red;
        [LabelText("辅助线长度")] public float debugLineLength = 1f;
        [LabelText("辅助线横向偏移度")] public float debugLineXOffset = 0f;
        private Vector3 _cachePosition; //用来判断位置是否变化，优化性能


        private void OnEnable()
        {
            SetZDepth();
        }

        private void LateUpdate()
        {
            if (isStatic)
            {
                return;
            }

            if (transform.position == _cachePosition)
            {
                return;
            }

            SetZDepth();
        }

        private void OnDrawGizmosSelected() //仅在选中物体时绘制Gizmos，调试offset
        {
            if (debugLineAlwaysActive)
            {
                return;
            }

            DrawDebugLine();
        }

        private void OnDrawGizmos()
        {
            if (!debugLineAlwaysActive)
            {
                return;
            }

            DrawDebugLine();
        }

        /// <summary>
        /// 设置z轴值
        /// </summary>
        private void SetZDepth()
        {
            var trans = transform;
            var pos = trans.position;
            _cachePosition = pos;
            //动态调整Z-Depth
            pos.z = pos.y + offset;
            trans.position = pos;
        }

        /// <summary>
        /// 手动更新Z值，预览效果
        /// </summary>
        [Button("更新Z轴值")]
        private void UpdateZDepth()
        {
            SetZDepth();
            var sp = GetComponent<SpriteRenderer>();
            if (sp)
            {
                sp.sortingLayerName = "SceneObject";
                sp.sortingOrder = 0;
            }

            var sortingGroup = GetComponent<SortingGroup>();
            if (!sortingGroup)
            {
                return;
            }

            sortingGroup.sortingLayerName = "SceneObject";
            sortingGroup.sortingOrder = 0;
        }

        /// <summary>
        /// 绘制辅助线
        /// </summary>
        private void DrawDebugLine()
        {
            //选择Gizmos的颜色
            Gizmos.color = debugLineColor;
            var position = transform.position;
            var startPoint = new Vector3(position.x - debugLineLength / 2 + debugLineXOffset, position.y + offset, position.z);
            var endPoint = new Vector3(position.x + debugLineLength / 2 + debugLineXOffset, position.y + offset, position.z);
            //绘制横线
            Gizmos.DrawLine(startPoint, endPoint);
        }
    }
}