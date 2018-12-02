using System.Collections.Generic;
using System.Web.Http;
using DanceService.DAL;
using DanceService.Models;

namespace DanceService.Controllers
{
    public class JudgeController : ApiController
    {
        private readonly IStorageService _storage;
        public JudgeController()
        {
            this._storage =
                (IStorageService)
                    GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IStorageService));
        }

        // GET: api/Judge
        public IEnumerable<JudgeModel> Get(string nomination, int tour)
        {
            return this._storage.GetJudges(nomination, tour);
        }

        // POST: api/Judge
        public bool Post(string nomination, int tour, int judge, bool isLock, bool isFinal,[FromBody]string content)
        {
            return this._storage.LockJudge(nomination, tour, judge, isLock, isFinal, content);
        }

        #region Unused

        // GET: api/Judge/5
        private string Get(int id)
        {
            return "value";
        }

        // PUT: api/Judge/5
        private void Put(int id, [FromBody] string value) { }

        // DELETE: api/Judge/5
        private void Delete(int id) { }

        #endregion

    }
}
