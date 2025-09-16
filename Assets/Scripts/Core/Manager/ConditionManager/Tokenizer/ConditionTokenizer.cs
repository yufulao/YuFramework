// ******************************************************************
//@file         ConditionTokenizer.cs
//@brief        条件表达式分词器
//@author       yufulao, yufulao@qq.com
//@createTime   2025.08.12 13:37:40 
// ******************************************************************

namespace Yu
{
    public class ConditionTokenizer
    {
        public ConditionTokenType Type;
        public int AtomConditionId;
        private readonly string _conditionStr; //条件表达式
        private int _charIndex; //当前字符识别位置

        public ConditionTokenizer(string conditionStr)
        {
            _conditionStr = conditionStr;
            Type = ConditionTokenType.Null;
        }

        /// <summary>
        /// 获取下一个token
        /// </summary>
        public void Next()
        {
            Type = ConditionTokenType.Null;
            while (_charIndex < _conditionStr.Length && char.IsWhiteSpace(_conditionStr[_charIndex]))
            {
                _charIndex++;
            }

            if (_charIndex >= _conditionStr.Length)
            {
                Type = ConditionTokenType.End;
                return;
            }

            var c = _conditionStr[_charIndex];
            _charIndex++;
            switch (c)
            {
                case '&':
                    Type = ConditionTokenType.And;
                    return;
                case '|':
                    Type = ConditionTokenType.Or;
                    return;
                case '!':
                    Type = ConditionTokenType.Not;
                    return;
                case '(':
                    Type = ConditionTokenType.LParen;
                    return;
                case ')':
                    Type = ConditionTokenType.RParen;
                    return;
                case '[':
                    ParseCondition();
                    return;
                default:
                    GameLog.Error("条件表达式含无效识别符", c);
                    return;
            }
        }

        /// <summary>
        /// []获取原子条件id
        /// </summary>
        private void ParseCondition()
        {
            var start = _charIndex;
            while (_charIndex < _conditionStr.Length && char.IsDigit(_conditionStr[_charIndex]))
            {
                _charIndex++;
            }
            
            if (_charIndex >= _conditionStr.Length || _conditionStr[_charIndex] != ']')
            {
                GameLog.Error("条件表达式[]不匹配");
            }

            var id = int.Parse(_conditionStr.Substring(start, _charIndex - start));
            _charIndex++;
            Type = ConditionTokenType.AtomConditionId;
            AtomConditionId = id;
        }
    }
}