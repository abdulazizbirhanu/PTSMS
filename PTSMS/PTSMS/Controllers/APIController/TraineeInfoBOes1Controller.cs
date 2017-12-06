using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using System.Xml.Serialization;
using PTSMSDAL.APIModels.Models;
using PTSMSDAL.Context;

namespace PTSMS.Controllers.APIController
{
    public class TraineeInfoBOes1Controller : ApiController
    {
        private EAA_API_Context db = new EAA_API_Context();

        // GET: api/TraineeInfoBOes1
        public IQueryable<TraineeInfoBO> GetTraineeInfobo()
        {
            return db.TraineeInfobo;
        }

        // GET: api/TraineeInfoBOes1/5
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

        // PUT: api/TraineeInfoBOes1/5
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

        // POST: api/TraineeInfoBOes1
        [ResponseType(typeof(TraineeInfoBO))]
        public IHttpActionResult PostTraineeInfoBO(TraineeInfoBO trainee)
        {
            try
            {


                string response;
                using (var stream = new StreamReader(Request.Content.ReadAsStreamAsync().Result))
                {
                    stream.BaseStream.Position = 0;
                    response = stream.ReadToEnd();
                }
                TraineeInfoBO traineeInfoBO = new TraineeInfoBO();
                traineeInfoBO = Deserialize(response);
                db.TraineeInfobo.Add(traineeInfoBO);
                db.SaveChanges();

                return CreatedAtRoute("DefaultApi", new { id = traineeInfoBO.id }, traineeInfoBO);
            }
            catch (Exception e)
            {
                return CreatedAtRoute("DefaultApi", null, "Refuse to save");
            }
        }
        [HttpPost]
        public IHttpActionResult PostTraineeInfoBO11(int id)
        {
            return CreatedAtRoute("DefaultApi", new { id = "ok" }, true);
        }
        public TraineeInfoBO Deserialize(string input)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(TraineeInfoBO));

                using (StringReader sr = new StringReader(input))
                {
                    return (TraineeInfoBO)ser.Deserialize(sr);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        // DELETE: api/TraineeInfoBOes1/5
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