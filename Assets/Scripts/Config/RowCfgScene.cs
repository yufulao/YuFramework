
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;



public sealed partial class RowCfgScene : Luban.BeanBase
{
    public RowCfgScene(ByteBuf _buf) 
    {
        Id = _buf.ReadString();
        ScenePath = _buf.ReadString();
        BGM = _buf.ReadString();
    }

    public static RowCfgScene DeserializeRowCfgScene(ByteBuf _buf)
    {
        return new RowCfgScene(_buf);
    }

    /// <summary>
    /// 场景名
    /// </summary>
    public readonly string Id;
    /// <summary>
    /// 资源路径
    /// </summary>
    public readonly string ScenePath;
    /// <summary>
    /// 场景背景音乐
    /// </summary>
    public readonly string BGM;
   
    public const int __ID__ = -2129227166;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
        
        
        
    }

    public override string ToString()
    {
        return "{ "
        + "id:" + Id + ","
        + "scenePath:" + ScenePath + ","
        + "BGM:" + BGM + ","
        + "}";
    }
}

