
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;

namespace Hotfix.Common.Data
{
    public partial class Tables
    {
        public const int TABLES_COUNT = 7;

        /// <summary> 例子 </summary>
        public Example.TbExampleBaseStruct TbExampleBaseStruct {get; private set;}
        /// <summary> 进阶例子 </summary>
        public Example.TbExampleExtendStruct TbExampleExtendStruct {get; private set;}
        /// <summary> 测试 </summary>
        public Example.TbBenchmark TbBenchmark {get; private set;}
        /// <summary> 道具例子 </summary>
        public Example.TbExampleItemData TbExampleItemData {get; private set;}
        /// <summary> 道具表 </summary>
        public Item.TbItemData TbItemData {get; private set;}
        /// <summary> 全局配置表 </summary>
        public Config.TbGlobalConfig TbGlobalConfig {get; private set;}
        /// <summary> 语言表 </summary>
        public L10n.TbLanguage TbLanguage {get; private set;}

        private void Load(System.Func<string, ByteBuf> loader)
        {
            //例子
            TbExampleBaseStruct = new Example.TbExampleBaseStruct(loader("example_tbexamplebasestruct"));
            //进阶例子
            TbExampleExtendStruct = new Example.TbExampleExtendStruct(loader("example_tbexampleextendstruct"));
            //测试
            TbBenchmark = new Example.TbBenchmark(loader("example_tbbenchmark"));
            //道具例子
            TbExampleItemData = new Example.TbExampleItemData(loader("example_tbexampleitemdata"));
            //道具表
            TbItemData = new Item.TbItemData(loader("item_tbitemdata"));
            //全局配置表
            TbGlobalConfig = new Config.TbGlobalConfig(loader("config_tbglobalconfig"));
            //语言表
            TbLanguage = new L10n.TbLanguage(loader("l10n_tblanguage"));
            ResolveRef();
        }

        private async Cysharp.Threading.Tasks.UniTask LoadAsync(System.Func<string, Cysharp.Threading.Tasks.UniTask<ByteBuf>> loader)
        {
            var startTime = System.DateTime.Now;

            Cysharp.Threading.Tasks.UniTask<ByteBuf>[] tasks = new Cysharp.Threading.Tasks.UniTask<ByteBuf>[TABLES_COUNT];

            
            tasks[0] = loader("example_tbexamplebasestruct");
            tasks[1] = loader("example_tbexampleextendstruct");
            tasks[2] = loader("example_tbbenchmark");
            tasks[3] = loader("example_tbexampleitemdata");
            tasks[4] = loader("item_tbitemdata");
            tasks[5] = loader("config_tbglobalconfig");
            tasks[6] = loader("l10n_tblanguage");

            var bytebufs = await Cysharp.Threading.Tasks.UniTask.WhenAll(tasks);

            float time = (float)(System.DateTime.Now - startTime).TotalMilliseconds;
            EGFramework.Runtime.Util.Log.DebugFormat("加载耗时:{0}", time);
            startTime = System.DateTime.Now;

            
            TbExampleBaseStruct = new Example.TbExampleBaseStruct(bytebufs[0]);
            TbExampleExtendStruct = new Example.TbExampleExtendStruct(bytebufs[1]);
            TbBenchmark = new Example.TbBenchmark(bytebufs[2]);
            TbExampleItemData = new Example.TbExampleItemData(bytebufs[3]);
            TbItemData = new Item.TbItemData(bytebufs[4]);
            TbGlobalConfig = new Config.TbGlobalConfig(bytebufs[5]);
            TbLanguage = new L10n.TbLanguage(bytebufs[6]);

            ResolveRef();

            time = (float)(System.DateTime.Now - startTime).TotalMilliseconds;
            EGFramework.Runtime.Util.Log.DebugFormat("处理耗时:{0}", time);
            startTime = System.DateTime.Now;
        }
        
        private void ResolveRef()
        {
            TbExampleBaseStruct.ResolveRef(this);
            TbExampleExtendStruct.ResolveRef(this);
            TbBenchmark.ResolveRef(this);
            TbExampleItemData.ResolveRef(this);
            TbItemData.ResolveRef(this);
            TbGlobalConfig.ResolveRef(this);
            TbLanguage.ResolveRef(this);
        }

        public void Dispose()
        {
            //例子
            TbExampleBaseStruct = null;
            //进阶例子
            TbExampleExtendStruct = null;
            //测试
            TbBenchmark = null;
            //道具例子
            TbExampleItemData = null;
            //道具表
            TbItemData = null;
            //全局配置表
            TbGlobalConfig = null;
            //语言表
            TbLanguage = null;
        }
    }

}
