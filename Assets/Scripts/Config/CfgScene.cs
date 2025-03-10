
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;



public partial class CfgScene
{
    private readonly System.Collections.Generic.Dictionary<string, RowCfgScene> _dataMap;
    private readonly System.Collections.Generic.List<RowCfgScene> _dataList;
    
    public CfgScene(ByteBuf _buf)
    {
        _dataMap = new System.Collections.Generic.Dictionary<string, RowCfgScene>();
        _dataList = new System.Collections.Generic.List<RowCfgScene>();
        
        for(int n = _buf.ReadSize() ; n > 0 ; --n)
        {
            RowCfgScene _v;
            _v = RowCfgScene.DeserializeRowCfgScene(_buf);
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
    }

    public System.Collections.Generic.Dictionary<string, RowCfgScene> DataMap => _dataMap;
    public System.Collections.Generic.List<RowCfgScene> DataList => _dataList;

    public RowCfgScene GetOrDefault(string key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public RowCfgScene Get(string key) => _dataMap[key];
    public RowCfgScene this[string key] => _dataMap[key];

    public void ResolveRef(Tables tables)
    {
        foreach(var _v in _dataList)
        {
            _v.ResolveRef(tables);
        }
    }

}



