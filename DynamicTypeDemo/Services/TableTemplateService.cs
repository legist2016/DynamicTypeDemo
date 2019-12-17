using DynamicTypeDemo.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicTypeDemo.Services
{
    public class NotFoundException : Exception { }
    public class BadRequestException : Exception {
        public BadRequestException():base() { }
        public BadRequestException(string message):base(message) { }
    }

    public class TableTemplateService: IDisposable
    {
        private Model2 db = new Model2();

        public IEnumerable<TableTemplate> GetTableTemplate()
        {
            var list = db.TableTemplates.Where(p => p.IsDelete == false);
            return list;
        }

        public TableTemplate GetTableTemplate(int id)
        {
            TableTemplate tableTemplate = db.TableTemplates.Find(id);
            return tableTemplate;
        }



        /// <exception cref="BadRequestException"></exception>
        public void PutTableTemplate(int id, TableTemplate tableTemplate) 
             
        {

            db.Entry(tableTemplate).State = EntityState.Modified;
            db.Entry(tableTemplate).Property("IsDelete").IsModified = false;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TableTemplateExists(id))
                {
                    throw new BadRequestException();
                }
                else
                {
                    throw;
                }
            }
        }
        
        public void PostTableTemplate(TableTemplate tableTemplate)
        {
            db.TableTemplates.Add(tableTemplate);
            db.SaveChanges();            
        }

        /// <exception cref="BadRequestException"></exception>
        /// <exception cref="NotFoundException"></exception>
        public TableTemplate DeleteTableTemplate(int id)
        {
            TableTemplate tableTemplate = db.TableTemplates.Find(id);
            if (tableTemplate == null)
            {
                throw new NotFoundException();
            }
            if (tableTemplate.Fields.Count() > 0)
            {
                throw new BadRequestException("非空无法删除。");
            }
            //db.TableTemplates.Remove(tableTemplate);
            tableTemplate.IsDelete = true;
            db.SaveChanges();

            return tableTemplate;
        }

        /// <exception cref="NotFoundException"></exception>
        public IEnumerable<TableTemplateField> GetTableTemplateFields(int id)
        {
            TableTemplate tableTemplate = db.TableTemplates.Find(id);
            if (tableTemplate == null)
            {
                throw new NotFoundException();
            }

            return tableTemplate.Fields;
        }

        /// <exception cref="BadRequestException"></exception>
        public void PutTableTemplateFields(int id, List<TableTemplateField[]> fields)
        {
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    TableTemplate tableTemplate = db.TableTemplates.Find(id);
                    if (tableTemplate == null)
                    {
                        throw new BadRequestException("无效数据");
                    }
                    if (fields[0] != null)
                    {
                        fields[0].ToList().ForEach(p =>
                        {
                            tableTemplate.Fields.Add(p);
                        });
                    }
                    db.SaveChanges();
                    if (fields[1] != null)
                    {
                        fields[1].ToList().ForEach(p =>
                        {
                            var field = tableTemplate.Fields.FirstOrDefault(q => q.Id == p.Id);
                            if (field == null)
                            {
                                throw new BadRequestException(string.Format("无效删除数据:{0},{1}", p.Name, p.Title));
                            }
                            tableTemplate.Fields.Remove(field);
                            db.TableTemplateFields.Remove(field);
                        });
                    }
                    db.SaveChanges();
                    if (fields[2] != null)
                    {
                        fields[2].ToList().ForEach(p =>
                        {
                            var field = tableTemplate.Fields.FirstOrDefault(q => q.Id == p.Id);
                            if (field == null)
                            {
                                throw new BadRequestException("无效数据");
                            }

                            db.Entry(field).State = EntityState.Detached;
                            db.Entry(p).State = EntityState.Modified;
                        });
                    }
                    db.SaveChanges();
                    tran.Commit();
                }
                catch (Exception)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        public void Dispose()
        {
            db.Dispose();
        }

        private bool TableTemplateExists(int id)
        {
            return db.TableTemplates.Count(e => e.Id == id) > 0;
        }
    }
}
