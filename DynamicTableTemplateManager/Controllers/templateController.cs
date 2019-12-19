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
using DynamicTypeDemo.Exceptions;

namespace DynamicTableTemplateManager.Controllers
{
    [RoutePrefix("api/template")]
    public class templateController : ApiController
    {
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
        [HttpPut]
        public IHttpActionResult PutTableTemplate(int id, TableTemplate tableTemplate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                service.PutTableTemplate(id, tableTemplate);
                return StatusCode(HttpStatusCode.NoContent);
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
        }

        [HttpPut]
        [Route("{id}/fields")]
        public IHttpActionResult PutTableTemplateFields(int id, List<TableTemplateField[]> fields)
        {
            try
            {
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


        // POST: api/template
        [ResponseType(typeof(TableTemplate))]
        public IHttpActionResult PostTableTemplate(TableTemplate tableTemplate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

        
        [Route("{id}/fields")]
        [HttpGet]
        public IHttpActionResult GetTableTemplateFields(int id)
        {
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


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                service.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}