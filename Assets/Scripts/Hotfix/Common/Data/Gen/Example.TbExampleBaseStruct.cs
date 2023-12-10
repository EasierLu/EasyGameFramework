
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;


namespace Hotfix.Common.Data.Example
{
/// <summary>
/// 例子
/// </summary>
public partial class TbExampleBaseStruct
{
    private readonly System.Collections.Generic.Dictionary<int, ExampleBaseStruct> _dataMap;
    private readonly System.Collections.Generic.List<ExampleBaseStruct> _dataList;
    
    public TbExampleBaseStruct(ByteBuf _buf)
    {
        _dataMap = new System.Collections.Generic.Dictionary<int, ExampleBaseStruct>();
        _dataList = new System.Collections.Generic.List<ExampleBaseStruct>();
        
        for(int n = _buf.ReadSize() ; n > 0 ; --n)
        {
            ExampleBaseStruct _v;
            _v = ExampleBaseStruct.DeserializeExampleBaseStruct(_buf);
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
    }

    public System.Collections.Generic.Dictionary<int, ExampleBaseStruct> DataMap => _dataMap;
    public System.Collections.Generic.List<ExampleBaseStruct> DataList => _dataList;

    public ExampleBaseStruct GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public ExampleBaseStruct Get(int key) => _dataMap[key];
    public ExampleBaseStruct this[int key] => _dataMap[key];

    public void ResolveRef(Tables tables)
    {
        foreach(var _v in _dataList)
        {
            _v.ResolveRef(tables);
        }
    }

}

}
