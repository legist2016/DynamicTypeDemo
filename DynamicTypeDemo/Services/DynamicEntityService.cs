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

        static public IDynamicObject Dynamic(this DynamicObject obj)
        {
            var typed = CallMethod("ConvertDynamicObject", new Type[] { obj.EntityType }, new object[] { obj.Object });
            IDynamicObject dynamic = typed as IDynamicObject;
            return dynamic;
        }

        static public DynamicObject Set(this DynamicObject obj)
        {
            return obj.Dynamic().Set();
        }
        static public DynamicObject ToList(this DynamicObject obj)
        {
            return obj.Dynamic().ToList();
        }
        static public DynamicObject Where(this DynamicObject obj, string field, string operation, object value)
        {
            return obj.Dynamic().Where(field, operation, value);
        }

    }

    public class DynamicObject{
        public DynamicObject(object obj, Type type) {
            Object = obj;
            EntityType = type;
        }
        public object Object { get; set; }
        public Type EntityType { get; set; }
    }

    public interface IDynamicObject
    {
        DynamicObject Set();
        DynamicObject ToList();
        DynamicObject Where(string field,string operation , object value);
    }

    public class DynamicObject<T>: IDynamicObject where T:class
    {
        public object Object;

        private DynamicObject dynamic(object obj)
        {
            return new DynamicObject(obj, typeof(T));
        }
        public DynamicObject Set()
        {
            DbContext db = Object as DbContext;
            return dynamic(db.Set<T>());
        }

        public DynamicObject ToList()
        {
            var list = Object as IEnumerable<T>;
            return dynamic(list.ToList());
        }

        public DynamicObject Where(string field, string operation, object value)
        {
            var list = Object as IQueryable<T>;
            var param = Expression.Parameter(typeof(T));
            var property = Expression.PropertyOrField(param, field);
            BinaryExpression body = null;

            switch (operation)
            {
                case "==":
                    body = Expression.Equal(property, Expression.Constant(value, value.GetType()));
                    break;
                case ">=":
                    body = Expression.GreaterThanOrEqual(property, Expression.Constant(value, value.GetType()));
                    break;
                case "<=":
                    body = Expression.LessThanOrEqual(property, Expression.Constant(value, value.GetType()));
                    break;
                case ">":
                    body = Expression.GreaterThan(property, Expression.Constant(value, value.GetType()));
                    break;
                case "<":
                    body = Expression.LessThan(property, Expression.Constant(value, value.GetType()));
                    break;
                case "!=":
                    body = Expression.NotEqual(property, Expression.Constant(value, value.GetType()));
                    break;
            }

            return dynamic(list.Where(Expression.Lambda<Func<T, bool>>(body, param)));
        }
    }


    public partial class DynamicEntityModel : DbContext
    {
        public Type Type { get; set; }

        public DynamicEntityModel(Type type)
            : base("name=Model1")
        {
            Database.SetInitializer<DynamicEntityModel>(null);
            Database.Log = (log) => { System.Diagnostics.Debug.WriteLine(log); };
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

        private DynamicEntityModel dynamicDb;

        public TableTemplate TableTemplate { get; set; }

        private Model2 db = new Model2();
        private DynamicObject dynamic = null;
        private Type _entityType = null;
        private Type[] _interfaces = null;

        public DynamicEntityService(Type entityType, string entityName)
        {
            EntityName = entityName;
            _entityType = entityType;
            dynamicDb = new DynamicEntityModel(EntityType);
            dynamic = new DynamicObject(dynamicDb, EntityType);
        }


        public DynamicEntityService(int templateId, string entityName, Type[] interfaces)
        {
            TableTemplate = db.TableTemplates.Find(templateId);
            if (TableTemplate == null)
            {
                throw new NotFoundException();
            }
            EntityName = entityName;
            _interfaces = interfaces;

            dynamicDb = new DynamicEntityModel(EntityType);
            dynamic = new DynamicObject(dynamicDb, EntityType);
        }

        public Type EntityType
        {
            get
            {
                if (_entityType == null)
                {
                    if (EntityTypes.ContainsKey(EntityName))
                    {
                        _entityType = EntityTypes[EntityName];
                    }
                    else
                    {
                        _entityType = TableTemplate.CreateType(EntityName ,null, _interfaces);
                        EntityTypes.Add(EntityName, _entityType);
                    }
                }
                return _entityType;
            }
        }

        public object GetEntities()
        {
            //var obj = new DynamicObject(dynamic, EntityType);
            return dynamic.Set().ToList().Object;
        }

        public object GetEntity(object key)
        {
            //var obj = new DynamicObject(dynamic, EntityType);
            return dynamic.Set().Where("SYS_ID","==",key).ToList().Object;
        }

        public void PutEntity(object key, object entity)
        {
            if (EntityType.GetProperty("SYS_ID").GetValue(entity) != key)
            {
                throw new BadRequestException();
            }
            dynamicDb.Entry(entity).State = EntityState.Modified;
            dynamicDb.SaveChanges();
        }
    }
}
