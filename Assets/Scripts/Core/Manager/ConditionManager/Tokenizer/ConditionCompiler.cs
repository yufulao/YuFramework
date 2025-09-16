// ******************************************************************
//@file         ConditionCompiler.cs
//@brief        条件表达式手动编译
//@author       yufulao, yufulao@qq.com
//@createTime   2025.08.12 13:52:02 
// ******************************************************************

using System;

namespace Yu
{
    public class ConditionCompiler
    {
        private readonly ConditionTokenizer _tokenizer;
        private readonly string[][] _params;
        private int _paramIndex;

        public ConditionCompiler(string conditionStr, string[][] paramList)
        {
            _tokenizer = new ConditionTokenizer(conditionStr);
            _params = paramList;
            _tokenizer.Next();
            _paramIndex = 0;
        }

        /// <summary>
        /// 识别主函数
        /// </summary>
        public Func<bool> Compile()
        {
            var result = ParseOr();
            if (_tokenizer.Type == ConditionTokenType.End)
            {
                return result;
            }
            
            GameLog.Error("错误结束");
            return () => false;

        }

        /// <summary>
        /// 分析下一个词
        /// </summary>
        private void NextToken(ConditionTokenType type)
        {
            if (_tokenizer.Type != type)
            {
                GameLog.Error("错误类型", type, _tokenizer.Type);
                return;
            }

            _tokenizer.Next();
        }

        /// <summary>
        /// 操作符|
        /// </summary>
        private Func<bool> ParseOr()
        {
            var left = ParseAnd();
            while (_tokenizer.Type == ConditionTokenType.Or)
            {
                NextToken(ConditionTokenType.Or);
                var right = ParseAnd();
                var l = left;
                var r = right;
                left = () => l() || r();
            }

            return left;
        }

        /// <summary>
        /// 操作符&
        /// </summary>
        private Func<bool> ParseAnd()
        {
            var left = ParseNot();
            while (_tokenizer.Type == ConditionTokenType.And)
            {
                NextToken(ConditionTokenType.And);
                var right = ParseNot();
                var l = left;
                var r = right;
                left = () => l() && r();
            }

            return left;
        }

        /// <summary>
        /// 操作符!
        /// </summary>
        private Func<bool> ParseNot()
        {
            if (_tokenizer.Type != ConditionTokenType.Not)
            {
                return ParsePrimary();
            }

            NextToken(ConditionTokenType.Not);
            var inner = ParsePrimary();
            return () => !inner();
        }

        /// <summary>
        /// ()括号嵌套
        /// </summary>
        private Func<bool> ParsePrimary()
        {
            switch (_tokenizer.Type)
            {
                case ConditionTokenType.LParen:
                {
                    NextToken(ConditionTokenType.LParen);
                    var inner = ParseOr();
                    NextToken(ConditionTokenType.RParen);
                    return inner;
                }

                case ConditionTokenType.AtomConditionId:
                {
                    var atomConditionId = _tokenizer.AtomConditionId;
                    var paramIndex = _paramIndex++;
                    NextToken(ConditionTokenType.AtomConditionId);
                    return () => ConditionManager.GetAtomCondition(atomConditionId, out var func) && func(_params[paramIndex]);
                }

                default:
                    GameLog.Error("未实现分词类型逻辑", _tokenizer.Type);
                    return null;
            }
        }
    }
}