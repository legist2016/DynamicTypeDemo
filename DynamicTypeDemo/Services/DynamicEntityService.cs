using DynamicTypeDemo.Entities;
using DynamicTypeDemo.Exceptions;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace DynamicTypeDemo.Services
{
    public interface IDynamicEntityModel
    {
        //object GetSet();
        object GetEntities();
        object GetEntities(DynamicCondition condition);
        object GetEntity(object key);
        void PutEntity(object key, object entity);
        object PostEntity(object entity);
        object DeleteEntity(object key);
    }
    public partial class DynamicEntityModel<T> : DbContext, IDynamicEntityModel
        where T:class//,IT_GeneralTable
    {

        public DynamicEntityModel()
            : base("name=Model1")
        {
            Database.SetInitializer<DynamicEntityModel>(null);
            //Database.Log = (log) => { System.Diagnostics.Debug.WriteLine(log); };
        }

        public virtual DbSet<T> Entities { get; set; }

        public object GetEntity(object key)
        {
            return this.Entities.Find(key);
        }

        public object GetEntities()
        {
            return this.Entities;
        }

        public object PostEntity(object entity)
        {
            Entities.Add(entity as T);
            SaveChanges();
            return entity;
        }

        public void PutEntity(object key, object obj)
        {
            T entity = obj as T;
            
            if (entity.GetKey() != key)
            {
                throw new BadRequestException();
            }
            this.Entry(entity).State = EntityState.Modified;
            SaveChanges();
        }
        public object DeleteEntity(object key)
        {
            T entity = Entities.Find(key);
            if (entity == null)
            {
                throw new NotFoundException();
            }
            Entities.Remove(entity);
            SaveChanges();
            return entity;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.RegisterEntityType(T);
        }

        public object GetEntities(DynamicCondition condition)
        {
            return this.Entities;//.Where(p=>p.SYS_ID.c);
        }

        static public Expression<Func<T, bool>> GenerateExpression(DynamicCondition condition)
        {
            throw new NotImplementedException();
        }
    }

    public partial class DynamicEntityModel : DbContext//, IDynamicEntityModel
    {
        public Type Type { get; set; }

        public DynamicEntityModel(Type type)
            : base("name=Model1")
        {
            Database.SetInitializer<DynamicEntityModel>(null);
            //Database.Log = (log) => { System.Diagnostics.Debug.WriteLine(log); };
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
        public static TableTemplate GetTemplate(int templateId)
        {
            using (Model2 db = new Model2())
            {
                var template = db.TableTemplates.Find(templateId);
                template.Fields.ToList();
                return template;
            }
        }

        public static object GetEntities(int templateId)
        {
            var db = GetTemplate(templateId).CreateType().GetDynamicEntityModel();
            return db.GetEntities();
        }

        public static object GetEntities(int templateId, string condition)
        {
            var db = GetTemplate(templateId).CreateType().GetDynamicEntityModel();
            var c = DynamicCondition.Parse(condition);
            return db.GetEntities(c);
        }

        public static object GetEntity(int templateId, object id)
        {
            var db = GetTemplate(templateId).CreateType().GetDynamicEntityModel();
            return db.GetEntity(id);
        }
    }
}
