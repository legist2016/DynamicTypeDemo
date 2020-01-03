using DynamicTypeDemo.Entities;
using DynamicTypeDemo.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            Database.SetInitializer<DynamicEntityModel<T>>(null);
            Database.Log = (log) => { System.Diagnostics.Debug.WriteLine(log); };
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

    public class DynamicEntityService: IDisposable
    {
        private static Dictionary<int, TableTemplate> tempDict = new Dictionary<int, TableTemplate>();

        private TableTemplate template = null;
        private Type type = null;

        public void Init(int tid)
        {
            if (tempDict.ContainsKey(tid))
            {
                template = tempDict[tid];
            }
            else
            {
                using (TemplateModel db = new TemplateModel())
                {
                    template = db.TableTemplates.Find(tid);
                    template.Fields.ToList();
                }
                tempDict.Add(tid, template);
            }
            type = template.CreateType();            
        }

        private IDynamicEntityModel Db
        {
            get
            {
                return type.GetDynamicEntityModel();
            }
        }

        private object entity(string json)
        {
            return JsonConvert.DeserializeObject(json, type);
        }

        public TableTemplate GetTemplate()
        {
            return template;
        }

        public object GetEntities()
        {
            return Db.GetEntities();
        }

        public object GetEntities(string condition)
        {
            var c = DynamicCondition.Parse(condition);
            return Db.GetEntities(c);
        }

        public object GetEntity(object id)
        {
            return Db.GetEntity(id);
        }

        public void PutEntity(object id, string json )
        {
            Db.PutEntity(id, entity(json));
        }

        public object PostEntity(string json)
        {
            return Db.PostEntity(entity(json));
        }

        public object DeleteEntity(object key)
        {
            return Db.DeleteEntity(key);
        }

        public void Dispose()
        {
            
        }
    }
}
