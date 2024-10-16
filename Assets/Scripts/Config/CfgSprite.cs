
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;



public partial class CfgSprite
{
    private readonly System.Collections.Generic.Dictionary<string, RowCfgSprite> _dataMap;
    private readonly System.Collections.Generic.List<RowCfgSprite> _dataList;
    
    public CfgSprite(ByteBuf _buf)
    {
        _dataMap = new System.Collections.Generic.Dictionary<string, RowCfgSprite>();
        _dataList = new System.Collections.Generic.List<RowCfgSprite>();
        
        for(int n = _buf.ReadSize() ; n > 0 ; --n)
        {
            RowCfgSprite _v;
            _v = RowCfgSprite.DeserializeRowCfgSprite(_buf);
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
    }

    public System.Collections.Generic.Dictionary<string, RowCfgSprite> DataMap => _dataMap;
    public System.Collections.Generic.List<RowCfgSprite> DataList => _dataList;

    public RowCfgSprite GetOrDefault(string key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public RowCfgSprite Get(string key) => _dataMap[key];
    public RowCfgSprite this[string key] => _dataMap[key];

    public void ResolveRef(Tables tables)
    {
        foreach(var _v in _dataList)
        {
            _v.ResolveRef(tables);
        }
    }

}



