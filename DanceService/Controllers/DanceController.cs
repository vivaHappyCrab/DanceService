using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DanceService.Models;
using DanceService.DAL;

namespace DanceService.Controllers
{
    public class DanceController : ApiController
    {
        private readonly IStorageService _storage;
        public DanceController()
        {
            this._storage =
                (IStorageService)
                    GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IStorageService));
        }

        // GET: api/Dance
        public IEnumerable<DanceModel> Get(string nomination)
        {
            return this._storage.GetDances(nomination);
        }

        // GET: api/Dance/5
        public DanceModel Get(string nomination, int num)
        {
            return this._storage.GetDance(nomination, num);
        }

        // POST: api/Dance
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Dance/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Dance/5
        public void Delete(int id)
        {
        }
    }
}
