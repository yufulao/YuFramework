
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;



public sealed partial class RowCfgCamera : Luban.BeanBase
{
    public RowCfgCamera(ByteBuf _buf) 
    {
        Id = _buf.ReadString();
        Position = ExternalTypeUtil.NewVector3(vector3.Deserializevector3(_buf));
        Rotation = ExternalTypeUtil.NewVector3(vector3.Deserializevector3(_buf));
        FieldOfView = _buf.ReadFloat();
    }

    public static RowCfgCamera DeserializeRowCfgCamera(ByteBuf _buf)
    {
        return new RowCfgCamera(_buf);
    }

    /// <summary>
    /// 镜头名字
    /// </summary>
    public readonly string Id;
    /// <summary>
    /// position
    /// </summary>
    public readonly UnityEngine.Vector3 Position;
    /// <summary>
    /// rotation
    /// </summary>
    public readonly UnityEngine.Vector3 Rotation;
    /// <summary>
    /// 视野
    /// </summary>
    public readonly float FieldOfView;
   
    public const int __ID__ = -2041215985;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
        
        
        
        
    }

    public override string ToString()
    {
        return "{ "
        + "id:" + Id + ","
        + "position:" + Position + ","
        + "rotation:" + Rotation + ","
        + "fieldOfView:" + FieldOfView + ","
        + "}";
    }
}

