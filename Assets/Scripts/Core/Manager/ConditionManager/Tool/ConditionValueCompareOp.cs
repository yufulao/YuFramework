// ******************************************************************
//@file         ConditionValueCompareOp.cs
//@brief        值比较操作符工具类
//@author       yufulao, yufulao@qq.com
//@createTime   2025.08.28 00:42:52 
// ******************************************************************

using System;

namespace Yu
{
    public static class ConditionValueCompareOp
    {
        private const int GT = 1;
        private const int LT = 2;
        private const int EQ = 3;
        private const int GE = 4;
        private const int LE = 5;

        /// <summary>
        /// 数值比较
        /// </summary>
        /// <param name="op">约定操作符</param>
        /// <param name="num1">操作数1</param>
        /// <param name="num2">操作数2</param>
        public static bool NumberCompare(int op, float num1, float num2)
        {
            switch (op)
            {
                case GT:
                    return num1 > num2;
                case LT:
                    return num1 < num2;
                case EQ:
                    return Math.Abs(num1 - num2) < 1e-6f;
                case GE:
                    return num1 >= num2;
                case LE:
                    return num1 <= num2;
            }

            GameLog.Error("无效操作符", op);
            return false;
        }
    }
}