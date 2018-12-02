using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DanceService.DAL;
using System.Security.Cryptography;
using System.Text;

namespace DanceService.Controllers
{
    public class ShaController : ApiController
    {
        private readonly IStorageService _storage;
        public ShaController()
        {
            this._storage =
                (IStorageService)
                    GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IStorageService));
        }
        // GET: api/Sha
        public string Get(string nomination, int tour, int judge)
        {
            IEnumerable<string> results = this._storage.GetShaResults(nomination, tour, judge);
            if (results == null)
                return null;

            if (tour > 1)
            {
                results = results.Select(Order);
            }

            byte[][] hashes = results.Select(HashString).ToArray();
            byte[] sumHash=new byte[20* hashes.Count()];

            for(int i=0;i< hashes.Count();++i)
            Buffer.BlockCopy(hashes[i], 0, sumHash, 20*i, 20);

            string hash16= HashBytes(sumHash);

            return (Convert.ToInt32(hash16, 16) % 10000).ToString();

        }

        #region Unused

        // GET: api/Sha/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Sha
        public void Post([FromBody] string value) { }

        // PUT: api/Sha/5
        public void Put(int id, [FromBody] string value) { }

        // DELETE: api/Sha/5
        public void Delete(int id) { }

        #endregion

        private static byte[] HashString(string input)
        {
            return (new SHA1Managed()).ComputeHash(Encoding.UTF8.GetBytes(input));
        }

        private static string  HashBytes(byte[] input)
        {
            byte[] hash= (new SHA1Managed()).ComputeHash(input);
            return string.Join(string.Empty, hash.Skip(18).Select(b => b.ToString("x2")));
        }

        private static string Order(string s)
        {
            return string.Join(string.Empty, s.Split('\n').Where(res=>res.Length>0).Select(int.Parse).OrderBy(res => res).Select(res => res.ToString()));
        }
    }
}
