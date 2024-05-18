// ******************************************************************
//@file         ForceAwakeSetActive.cs
//@brief        强制设定Awake后的gameObj激活状态
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:32:12
// ******************************************************************
using UnityEngine;

public class ForceAwakeSetActive : MonoBehaviour
{
    public GameObject setActiveObj;
    public bool setActiveBool;

    private void Awake()
    {
        if (setActiveObj.activeInHierarchy!= setActiveBool)
        {
            setActiveObj.SetActive(setActiveBool);
        }
    }
}
