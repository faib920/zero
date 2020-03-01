using Fireasy.Data.Entity;
using Fireasy.Zero.Models;

namespace Fireasy.Zero.Services.Impls
{
    public class MongodbContext : EntityContext
    {
        public MongodbContext()
        {
        }

        //由于startup中注册了多个 EntityContext，因此这里必须使用泛型的 EntityContextOptions
        //如果只有一个 EntityContext，则可以使用 EntityContextOptions
        public MongodbContext(EntityContextOptions<MongodbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// 获取或设置 系统日志 实体仓储。
        /// </summary> 
        public IRepository<SysLog> SysLogs { get; set; }
    }
}
