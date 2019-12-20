using DynamicTypeDemo.Entities;
using DynamicTypeDemo.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DynamicTypeDemo.Services
{
    static public class DynamicEntityExtension
    {

        static public object CallMethod(string method, Type[] genericMethodTypes, object[] parameters = null)
        {
            var m = typeof(DynamicEntityExtension).GetMethod(method);
            m = m.MakeGenericMethod(genericMethodTypes);
            return m.Invoke(null, parameters);
        }

        static public DynamicObject<T> ConvertDynamicObject<T>(object obj) where T:class
        {
            return new DynamicObject<T>(){ Object = obj};
        }

        static public DynamicObject DynamicMethods(this DynamicObject obj, string method, params object[] Params )
        {
            var typed = CallMethod("ConvertDynamicObject", new Type[] { obj.EntityType}, new object[] { obj.Object});
            IDynamicObject dynamic = typed as IDynamicObject;
            return dynamic.DynamicMethods(method, Params);            
        }
        static public DynamicObject Set(this DynamicObject obj)
        {
            return obj.DynamicMethods("Set");
        }
        static public DynamicObject ToList(this DynamicObject obj)
        {
            return obj.DynamicMethods("ToList");
        }
        static public DynamicObject Where(this DynamicObject obj, string field, object value)
        {
            return obj.DynamicMethods("Where", field, value);
        }

    }

    public class DynamicObject{
        //public DynamicObject(){}
        public DynamicObject(object obj, Type type) {
            Object = obj;
            EntityType = type;
        }
        public object Object { get; set; }
        public Type EntityType { get; set; }
    }

    public interface IDynamicObject
    {
        DynamicObject DynamicMethods(string method, params object[] Params);
    }

    public class DynamicObject<T>: IDynamicObject where T:class
    {
        public object Object;

        public DynamicObject DynamicMethods(string method, params object[] ps)
        {
            try
            {
                object result = null;
                if (method == "Set")
                {
                    result = Set();
                }
                else if (method == "Where")
                {
                    result = Where(ps[0] as string,ps[1]);
                }
                else if (method == "ToList")
                {
                    result = ToList();
                }
                return new DynamicObject(result, typeof(T));
            }
            catch
            {
                throw;
            }
        }
        DbSet Set()
        {
            DbContext db = Object as DbContext;
            return db.Set<T>();
        }

        List<T> ToList()
        {
            var list = Object as IEnumerable<T>;
            return list.ToList();
        }

        IQueryable<T> Where(string field, object value)
        {
            var list = Object as IQueryable<T>;
            var param = Expression.Parameter(typeof(T));
            var property = Expression.PropertyOrField(param, field);
            var body = Expression.Equal(property, Expression.Constant(value, value.GetType()));
            return list.Where(Expression.Lambda<Func<T, bool>>(body, param));
        }
    }


    public partial class DynamicEntityModel : DbContext
    {
        public Type Type { get; set; }

        public DynamicEntityModel(Type type)
            : base("name=Model1")
        {
            Database.SetInitializer<DynamicEntityModel>(null);
            Type = type;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (Type != null)
            {
                modelBuilder.RegisterEntityType(Type);
            }
        }
       
    }

    public class DynamicEntityService
    {
        static private Dictionary<string, Type> EntityTypes = new Dictionary<string, Type>();
        public string EntityName { get; set; }
        public TableTemplate TableTemplate { get; set; }

        private Model2 db = new Model2();
        private DynamicObject dynamic = null;


        public DynamicEntityService(Type entityType, string entityName)
        {
            EntityName = entityName;
            EntityTypes.Add(EntityName, entityType);
            dynamic = new DynamicObject(new DynamicEntityModel(EntityType), EntityType);
        }



        public DynamicEntityService(int templateId, string entityName, Type[] interfaces)
        {
            TableTemplate = db.TableTemplates.Find(templateId);
            if (TableTemplate == null)
            {
                throw new NotFoundException();
            }
            EntityName = entityName;
            dynamic = new DynamicObject(new DynamicEntityModel(EntityType), EntityType);
        }

        public Type EntityType
        {
            get
            {

                Type type;
                if (EntityTypes.ContainsKey(EntityName))
                {
                    type = EntityTypes[EntityName];
                }
                else 
                {
                    type = TableTemplate.CreateType(EntityName, null, new Type[] { typeof(IT_GeneralTable) });
                    EntityTypes.Add(EntityName, type);
                }
                return type;

            }
        }

        public object GetEntities()
        {
            //var obj = new DynamicObject(dynamic, EntityType);
            return dynamic.DynamicMethods("Set").DynamicMethods("ToList").Object;
        }

        public object GetEntities(object key)
        {
            //var obj = new DynamicObject(dynamic, EntityType);
            return dynamic.Set().Where("SYS_ID",key).ToList().Object;
        }

    }
}
