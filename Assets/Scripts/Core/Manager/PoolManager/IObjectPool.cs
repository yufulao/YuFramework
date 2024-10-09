// ******************************************************************
//@file         IObjectPool.cs
//@brief        对象池接口，面向接口编程，处理泛型对象池
//@author       yufulao, yufulao@qq.com
//@createTime   2024.07.02 00:38:42
// ******************************************************************
namespace Yu
{
    public interface IObjectPool
    {
        void AutoDestroy();//定期检查并自动销毁闲置时间过长的对象
    }
}