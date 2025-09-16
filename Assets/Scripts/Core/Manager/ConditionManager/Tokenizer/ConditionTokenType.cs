// ******************************************************************
//@file         ConditionTokenType.cs
//@brief        条件表达式分词类型
//@author       yufulao, yufulao@qq.com
//@createTime   2025.08.12 13:39:15 
// ******************************************************************

namespace Yu
{
    public enum ConditionTokenType
    {
        Null,//异常状态
        And, //&&
        Or, //||
        Not, //!
        End, //表达式结束
        LParen, //左括号
        RParen, //右括号
        AtomConditionId, //原子条件id
    }
}