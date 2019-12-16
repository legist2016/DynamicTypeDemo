using DynamicTypeDemo;
using DynamicTypeDemo.Template;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace DynamicTableTemplateManager.Controllers
{
    public class templateController : ApiController
    {
        private Model2 db = new Model2();

        // GET: api/template
        public IHttpActionResult GetTableTemplate()
        {
            var list =  db.TableTemplates.Where(p=>p.IsDelete == false);
            return Ok(list);
        }

        // GET: api/template/5
        [ResponseType(typeof(TableTemplate))]
        public IHttpActionResult GetTableTemplate(int id)
        {
            TableTemplate tableTemplate = db.TableTemplates.Find(id);
            if (tableTemplate == null)
            {
                return NotFound();
            }

            return Ok(tableTemplate);
        }



        // PUT: api/template/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTableTemplate(int id, TableTemplate tableTemplate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tableTemplate.Id)
            {
                return BadRequest();
            }


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
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/template
        [ResponseType(typeof(TableTemplate))]
        public IHttpActionResult PostTableTemplate(TableTemplate tableTemplate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TableTemplates.Add(tableTemplate);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = tableTemplate.Id }, tableTemplate);
        }

        // DELETE: api/template/5
        [ResponseType(typeof(TableTemplate))]
        public IHttpActionResult DeleteTableTemplate(int id)
        {
            TableTemplate tableTemplate = db.TableTemplates.Find(id);
            if (tableTemplate == null)
            {
                return NotFound();
            }

            //db.TableTemplates.Remove(tableTemplate);
            tableTemplate.IsDelete = true;
            db.SaveChanges();

            return Ok(tableTemplate);
        }

        // GET: api/template/fields/5
        //[ResponseType(typeof(TableTemplate))]
        [ActionName("fields")]
        [HttpGet]
        public IHttpActionResult GetTableTemplateFields(int id)
        {
            TableTemplate tableTemplate = db.TableTemplates.Find(id);
            if (tableTemplate == null)
            {
                return NotFound();
            }

            return Ok(tableTemplate.Fields);
        }

        // PUT: api/template/fields/5
        //[ResponseType(typeof(TableTemplate))]
        [ActionName("fields")]
        [ResponseType(typeof(void))]
        [HttpPut]
        public IHttpActionResult PutTableTemplateFields(int id, List<TableTemplateField[]> fields)
        {
            using (var tran = db.Database.BeginTransaction())
            {
                try {
                    if (fields[2] != null)
                    {
                        fields[2].ToList().ForEach(p =>
                        {
                           
                            if (db.TableTemplateFields.Count(q => p.Id == q.Id && p.TableTemplateId == q.TableTemplateId)!=1)
                            {
                                throw new Exception("无效数据");
                            }

                            db.Entry(p).State = EntityState.Modified;
                        });
                    }
                    db.SaveChanges();
                    TableTemplate tableTemplate = db.TableTemplates.Find(id);
                    if (tableTemplate == null)
                    {
                        throw new Exception("无效数据");
                    }
                    if (fields[0] != null)
                    {
                        fields[0].ToList().ForEach(p =>
                        {
                            tableTemplate.Fields.Add(p);
                        });
                    }
                    if (fields[1] != null)
                    {
                        fields[1].ToList().ForEach(p =>
                        {
                            var field = tableTemplate.Fields.FirstOrDefault(q => q.Id == p.Id);
                            if (field == null)
                            {
                                throw new Exception("无效数据");
                            }
                            tableTemplate.Fields.Remove(field);
                        });
                    }
                    db.SaveChanges();
                    tran.Commit();
                    return StatusCode(HttpStatusCode.NoContent);
                }
                catch (Exception ex) {
                    tran.Rollback();
                    return BadRequest(ex.Message);
                }
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TableTemplateExists(int id)
        {
            return db.TableTemplates.Count(e => e.Id == id) > 0;
        }
    }
}