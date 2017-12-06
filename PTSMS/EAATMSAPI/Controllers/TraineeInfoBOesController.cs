using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using EAATMSAPI.Context;
using EAATMSAPI.Models;

namespace EAATMSAPI.Controllers
{
    public class TraineeInfoBOesController : ApiController
    {
        private EAA_API_Context db = new EAA_API_Context();

        // GET: api/TraineeInfoBOes
        public IQueryable<TraineeInfoBO> GetTraineeInfobo()
        {
            return db.TraineeInfobo;
        }

        // GET: api/TraineeInfoBOes/5
        [ResponseType(typeof(TraineeInfoBO))]
        public IHttpActionResult GetTraineeInfoBO(int id)
        {
            TraineeInfoBO traineeInfoBO = db.TraineeInfobo.Find(id);
            if (traineeInfoBO == null)
            {
                return NotFound();
            }

            return Ok(traineeInfoBO);
        }

        // PUT: api/TraineeInfoBOes/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTraineeInfoBO(int id, TraineeInfoBO traineeInfoBO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != traineeInfoBO.id)
            {
                return BadRequest();
            }

            db.Entry(traineeInfoBO).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TraineeInfoBOExists(id))
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

        // POST: api/TraineeInfoBOes
        [ResponseType(typeof(TraineeInfoBO))]
        public IHttpActionResult PostTraineeInfoBO(TraineeInfoBO traineeInfoBO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TraineeInfobo.Add(traineeInfoBO);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = traineeInfoBO.id }, traineeInfoBO);
        }

        // DELETE: api/TraineeInfoBOes/5
        [ResponseType(typeof(TraineeInfoBO))]
        public IHttpActionResult DeleteTraineeInfoBO(int id)
        {
            TraineeInfoBO traineeInfoBO = db.TraineeInfobo.Find(id);
            if (traineeInfoBO == null)
            {
                return NotFound();
            }

            db.TraineeInfobo.Remove(traineeInfoBO);
            db.SaveChanges();

            return Ok(traineeInfoBO);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TraineeInfoBOExists(int id)
        {
            return db.TraineeInfobo.Count(e => e.id == id) > 0;
        }
    }
}