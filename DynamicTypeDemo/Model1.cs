namespace DynamicTypeDemo
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Collections.Generic;

    public partial class Model1 : DbContext
    {
        public List<Type> Types { get; set; }

        public Model1(List<Type> types = null)
            : base("name=Model1")
        {
            //type = TypeCreator.Creator("T_TEST", 10);
            Types = types;
        }

        //public virtual DbSet<T_CM_ORDER> T_CM_ORDER { get; set; }

        //public Type type { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.RegisterEntityType(typeof(T_CM_PRODUCT));
            //modelBuilder.RegisterEntityType(type);
            if (Types != null)
            {
                foreach (var type in Types)
                {
                    modelBuilder.RegisterEntityType(type);
                }
            }

        }

        public object getdata(Type type, string id, object value)
        {
            var where = CallMethod("where", new Type[] { type, typeof(int) }, new object[] { id, value });
            var list = CallMethod("queryData", new Type[] { type }, new object[] { this, where });
            return list;

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
