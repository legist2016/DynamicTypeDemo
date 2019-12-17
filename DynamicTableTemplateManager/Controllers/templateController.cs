using DynamicTypeDemo.Services;
using DynamicTypeDemo.Entities;
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
        //private Model2 db = new Model2();
        private TableTemplateService service = new TableTemplateService();

        // GET: api/template
        public IHttpActionResult GetTableTemplate()
        {
            var list = service.GetTableTemplate(); // db.TableTemplates.Where(p=>p.IsDelete == false);
            return Ok(list);
        }

        // GET: api/template/5
        [ResponseType(typeof(TableTemplate))]
        public IHttpActionResult GetTableTemplate(int id)
        {
            TableTemplate tableTemplate = service.GetTableTemplate(id);// db.TableTemplates.Find(id);
            if (tableTemplate == null)
            {
                return NotFound();
            }

            return Ok(tableTemplate);
        }



        // PUT: api/template/5
        [ResponseType(typeof(void))]
        [NonAction]
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


            //db.Entry(tableTemplate).State = EntityState.Modified;
            //db.Entry(tableTemplate).Property("IsDelete").IsModified = false;

            try
            {
                //db.SaveChanges();
                service.PutTableTemplate(id, tableTemplate);
            }
            catch (NotFoundException)
            {
                    return NotFound();
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                throw;
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

            //db.TableTemplates.Add(tableTemplate);
            //db.SaveChanges();
            service.PostTableTemplate(tableTemplate);

            return CreatedAtRoute("DefaultApi", new { id = tableTemplate.Id }, tableTemplate);
        }

        // DELETE: api/template/5
        [ResponseType(typeof(TableTemplate))]
        public IHttpActionResult DeleteTableTemplate(int id)
        {
            try
            {
                TableTemplate tableTemplate = service.DeleteTableTemplate(id);
                /*db.TableTemplates.Find(id);
            if (tableTemplate == null)
            {
                return NotFound();
            }
            if (tableTemplate.Fields.Count() > 0)
            {
                return BadRequest("非空无法删除。");
            }
            //db.TableTemplates.Remove(tableTemplate);
            tableTemplate.IsDelete = true;
            db.SaveChanges();
            */
                return Ok(tableTemplate);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [ActionName("fields")]
        [HttpGet]
        public IHttpActionResult GetTableTemplateFields(int id)
        {
            /*TableTemplate tableTemplate = db.TableTemplates.Find(id);
            if (tableTemplate == null)
            {
                return NotFound();
            }*/
            try
            {
                var fields = service.GetTableTemplateFields(id);
                return Ok(fields);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        // PUT: api/template/fields/5
        //[ResponseType(typeof(TableTemplate))]
        [ActionName("fields")]
        [ResponseType(typeof(void))]
        [HttpPut]
        public IHttpActionResult PutTableTemplateFields(int id, List<TableTemplateField[]> fields)
        {
            /*using (var tran = db.Database.BeginTransaction())
            {
                try {
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
                    db.SaveChanges();
                    if (fields[1] != null)
                    {
                        fields[1].ToList().ForEach(p =>
                        {
                            var field = tableTemplate.Fields.FirstOrDefault(q => q.Id == p.Id);
                            if (field == null)
                            {
                                throw new Exception(string.Format("无效删除数据:{0},{1}",p.Name,p.Title));
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
                                throw new Exception("无效数据");
                            }

                            db.Entry(field).State = EntityState.Detached;
                            db.Entry(p).State = EntityState.Modified;
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
            }*/
            try
            {
                //db.SaveChanges();
                service.PutTableTemplateFields(id, fields);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);

        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                service.Dispose();
            }
            base.Dispose(disposing);
        }

        /*private bool TableTemplateExists(int id)
        {
            return db.TableTemplates.Count(e => e.Id == id) > 0;
        }*/
    }
}