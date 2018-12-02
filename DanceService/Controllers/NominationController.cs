using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DanceService.DAL;
using DanceService.Models;

namespace DanceService.Controllers
{
    public class NominationController : ApiController
    {
        private readonly IStorageService _storage;
        public NominationController()
        {
            this._storage =
                (IStorageService)
                    GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof (IStorageService));
        }

        // GET api/Nomination
        public IEnumerable<NominationModel> Get()
        {
            return this._storage.GetNominations();
        }

        // GET api/Nomination/5
        public GroupModel Get(string number)
        {
            return this._storage.GetGroup(number);
        }

        #region Unused

        // POST api/values
        private void Post([FromBody] string value) {}

        // PUT api/values/5
        private void Put(int id, [FromBody] string value) {}

        // DELETE api/values/5
        private void Delete(int id) {}

        #endregion

    }
}
