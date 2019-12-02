namespace DynamicTypeDemo
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Linq.Expressions;

    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Model1")
        {
            type = TypeCreator.Creator("T_TEST", 10);
        }

        //public virtual DbSet<T_CM_ORDER> T_CM_ORDER { get; set; }

        public Type type { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.RegisterEntityType(typeof(T_CM_PRODUCT));
            modelBuilder.RegisterEntityType(type);

        }

        public object getdata()
        {
            /*var method = this.GetType().GetMethod("Set", new Type[] { });
            method = method.MakeGenericMethod(type);
            object set = method.Invoke(this, null);
            method = typeof(Enumerable).GetMethod("ToList");
            method = method.MakeGenericMethod(type);
            object obj = method.Invoke(null, new object[] { set });*/
            var where = CallMethod("where", new Type[] { type, typeof(int) }, new object[] { "id", 1 });
            var list = CallMethod("queryData", new Type[] { type }, new object[] { this, where });
            return list;
            /*Type ettype = typeof(T_CM_PRODUCT);
            Type dbtype = db.GetType();
            var method = dbtype.GetMethod("Set", new Type[] { });
            method = method.MakeGenericMethod(ettype);
            object set = method.Invoke(db, null);

            var property = set.GetType().GetProperty("Sql");
            var sql = property.GetValue(set);

            var list = db.Set<T_CM_PRODUCT>();
            method = typeof(Enumerable).GetMethod("ToList");
            method = method.MakeGenericMethod(typeof(T_CM_PRODUCT));
            obj = method.Invoke(null,new object[] { list });*/

        }
        static public object CallMethod(string method, Type[] genericMethodTypes, object[] parameters = null)
        {
            var m = typeof(Model1).GetMethod(method);
            m = m.MakeGenericMethod(genericMethodTypes);
            return m.Invoke(null, parameters);
        }
        static public Expression<Func<T, bool>> where<T, V>(string field, V value)
        {
            var param = Expression.Parameter(typeof(T));
            var property = Expression.PropertyOrField(param, field);
            var body = Expression.Equal(property, Expression.Constant(value, typeof(V)));
            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        static public System.Collections.Generic.List<T> queryData<T>(object db, object where) where T : class
        {
            Expression<Func<T, bool>> Where = where as Expression<Func<T, bool>>;
            System.Data.Entity.DbContext Db = db as System.Data.Entity.DbContext;
            return Db.Set<T>().Where(Where).ToList();
        }
    }
}
