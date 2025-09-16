// ******************************************************************
//@file         UIAnimBezierPath.cs
//@brief        贝塞尔曲线路径组件
//@author       yufulao, yufulao@qq.com
//@createTime   2025.07.08 00:59:40 
// ******************************************************************

using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteAlways]
public class UIAnimBezierPath : MonoBehaviour
{
    [LabelText("曲线中点")] public Transform center;
    [LabelText("曲线终点")] public Transform target;
    [LabelText("播放延迟")] public float delay;
    [LabelText("动画时长")] public float duration = 1f;
    [LabelText("动画速率曲线")] public AnimationCurve rateCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [LabelText("到中点后的惯性")] public float inertiaDistance;
    [LabelText("到终点后的惯性")] public float curvePullDistance;
    [LabelText("显示曲线参考线")] public bool drawBezierCurve = true;
    public bool IsPlaying => _tweener == null; // 当前是否正在播放动画
    private Vector3 _originalPosition; //初始位置
    private Vector3 _p0, _p1, _p2, _p3; //曲线所用点位
    private float _curAnimProgress; //当前曲线动画进度
    private Tweener _tweener;
    private Action _onTweenStart;
    private Action _onTweenEnd;

    private void Start()
    {
        _originalPosition = transform.position;
    }
    
    private void OnDestroy()
    {
        _tweener?.Kill();
    }

    /// <summary>
    /// 设置动画初始位置
    /// </summary>
    public void SetOriginalPosition(Vector3 pos)
    {
        _originalPosition = pos;
    }

    /// <summary>
    /// 绑定动画事件
    /// </summary>
    public void BindAnimEvent(Action onTweenStart, Action onTweenEnd)
    {
        _onTweenStart = onTweenStart;
        _onTweenEnd = onTweenEnd;
    }

    /// <summary>
    /// 显示隐藏a
    /// </summary>
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    public void Play()
    {
        if (!center|| !target)
        {
            GameLog.Error("BezierPath必须设置center和target");
            return;
        }
        
        SetBezierPoints();
        _curAnimProgress = 0f;
        transform.SetAsLastSibling();
        _tweener?.Kill();
        SetTween();
    }

    /// <summary>
    /// 强制完成
    /// </summary>
    public void ForceComplete()
    {
        _tweener?.Complete();
    }

    /// <summary>
    /// 设置曲线动画
    /// </summary>
    private void SetTween()
    {
        _tweener = DOTween.To(() => _curAnimProgress, OnAnimProgressTween, 1f, duration);
        _tweener.SetDelay(delay);
        _tweener.OnPlay(OnTweenPlay);
        _tweener.SetEase(rateCurve);
        _tweener.OnComplete(OnTweenComplete);
    }

    /// <summary>
    /// 曲线动画播放时
    /// </summary>
    private void OnTweenPlay()
    {
        _onTweenStart?.Invoke();
    }

    /// <summary>
    /// 曲线动画进度变化时
    /// </summary>
    private void OnAnimProgressTween(float curAnimProgress)
    {
        _curAnimProgress = curAnimProgress;
        transform.position = Bezier3(_p0, _p1, _p2, _p3, _curAnimProgress);
    }

    /// <summary>
    /// 曲线动画结束时
    /// </summary>
    private void OnTweenComplete()
    {
        _curAnimProgress = 1f;
        _tweener = null;
        _onTweenEnd?.Invoke();
    }

    /// <summary>
    /// 设置贝塞尔数据
    /// </summary>
    private void SetBezierPoints()
    {
        _p0 = _originalPosition;
        var centerPos = center.position;
        var toCenter = (centerPos - _p0).normalized;
        _p1 = centerPos + toCenter * inertiaDistance;
        var targetPos = target.position;
        var toTarget = (targetPos - centerPos).normalized;
        _p2 = centerPos + toTarget * curvePullDistance;
        _p3 = targetPos;
    }

    /// <summary>
    /// t时在曲线的位置
    /// </summary>
    private static Vector3 Bezier3(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        var u = 1f - t;
        var tt = t * t;
        var uu = u * u;
        var uuu = uu * u;
        var ttt = tt * t;
        var p = uuu * p0; // (1-t)^3 * P0
        p += 3f * uu * t * p1; // 3*(1-t)^2 * t * P1
        p += 3f * u * tt * p2; // 3*(1-t) * t^2 * P2
        p += ttt * p3; // t^3 * P3
        return p;
    }

#if UNITY_EDITOR
    /// <summary>
    /// 绘制曲线
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!drawBezierCurve || center == null || target == null)
        {
            return;
        }

        _originalPosition = transform.position;
        SetBezierPoints();
        Gizmos.color = Color.green;
        var prev = _p0;
        const int segments = 30;
        for (var i = 1; i <= segments; i++)
        {
            var tt = i / (float) segments;
            var curr = Bezier3(_p0, _p1, _p2, _p3, tt);
            Gizmos.DrawLine(prev, curr);
            prev = curr;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_p1, 5f);
        Gizmos.DrawSphere(_p2, 5f);
    }
#endif
}