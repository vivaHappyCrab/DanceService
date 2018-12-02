using DanceService.Models;
using System.Collections.Generic;

namespace DanceService.DAL
{
    public interface IStorageService
    {
        IEnumerable<NominationModel> GetNominations();

        GroupModel GetGroup(string number);

        IEnumerable<JudgeModel> GetJudges(string nomination, int tour);

        bool LockJudge(string nomination, int tour, int judge, bool isLock, bool isFinal,string content);

        IEnumerable<DanceModel> GetDances(string nomination);

        DanceModel GetDance(string nomination, int num);

        IEnumerable<string> GetShaResults(string nomination, int tour, int judge);
    }
}