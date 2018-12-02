using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DanceService.Models;
using System.IO;

namespace DanceService.DAL
{
    public class FileStorage : IStorageService
    {
        #region Fields and Consts

        private readonly string _path;

        private const int BufferSize = 1000;

        private const string MainPath = "airdance\\";
        private const string RescuePath = "rescue\\";

        private const string ResultsPath = "results\\";
        private const string JudgesPath = "judges\\";
        private const string SignaturesPath = "signatures\\";

        private const string NominationFileName = "nominations.txt";
        private const string GroupFileName = "gruppa.txt";
        private const string JudgeFileName = "judge.txt";
        private const string DanceFileName = "dance";
        private const string ShaFileName = "sha1.txt";

        #endregion

        public FileStorage(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            this._path = path;
        }

        public IEnumerable<NominationModel> GetNominations()
        {
            try
            {
                string fileContent = File.ReadAllText($"{this._path}{MainPath}{NominationFileName}");
                if (string.IsNullOrWhiteSpace(fileContent))
                    return null;

                string[] splited = Clear(fileContent).Split('\n');
                return splited.Select(s => new NominationModel
                {
                    Number = s.Substring(0, s.IndexOf(':')),
                    Name = s.Substring(s.IndexOf(':') + 1)
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return null;
            }
        }

        public GroupModel GetGroup(string number)
        {
            try
            {
                string fileContent = File.ReadAllText($"{this._path}{MainPath}{number}\\{GroupFileName}");
                if (string.IsNullOrWhiteSpace(fileContent))
                    return null;

                string[] splited = Clear(fileContent).Split(';');
                return new GroupModel()
                {
                    Name = splited[0].Trim('"'),
                    Pairs = int.Parse(splited[1]),
                    Advanced = int.Parse(splited[2]),
                    Tour = int.Parse(splited[3]),
                    DancesCount = int.Parse(splited[4]),
                    Dances = splited.Skip(5).Take(int.Parse(splited[4])).Select(s => s.Trim('"'))
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return null;
            }
        }

        public IEnumerable<JudgeModel> GetJudges(string nomination, int tour)
        {
            try
            {
                List<JudgeModel> result = new List<JudgeModel>();
                string fileContent = File.ReadAllText($"{this._path}{MainPath}{nomination}\\{JudgeFileName}");
                if (string.IsNullOrWhiteSpace(fileContent))
                    return null;

                IEnumerable<string> splited = Clear(fileContent).Split('\n').Where(s => !s.StartsWith("\"\"") && !string.IsNullOrWhiteSpace(s));

                int order = 1;
                foreach (string[] judgeSplitted in splited.Select(s => s.Split(';')))
                {
                    result.Add(new JudgeModel()
                    {
                        Number = order,
                        Name = judgeSplitted[0].Trim('"'),
                        Password = judgeSplitted.Length > 1 ? judgeSplitted[1].Trim('"') : null,
                        Locked = this.IsJudgeLocked(nomination, tour, order)
                    });
                    order++;
                }
                return result;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return null;
            }
        }

        public bool LockJudge(string nomination, int tour, int judge, bool isLock, bool isFinal, string content)
        {
            try
            {
                if (isFinal)
                {
                    File.Delete($"{this._path}{MainPath}{nomination}\\{JudgesPath}{judge:00}.lock");
                    File.WriteAllText($"{this._path}{MainPath}{nomination}\\{ResultsPath}{ReultsName(tour, judge)}.lock", content);
                }
                else if (isLock)
                {
                    File.Delete($"{this._path}{MainPath}{nomination}\\{ResultsPath}{ReultsName(tour, judge)}.lock");
                    File.WriteAllText($"{this._path}{MainPath}{nomination}\\{JudgesPath}{judge:00}.lock", content);
                }
                else
                {
                    File.Delete($"{this._path}{MainPath}{nomination}\\{JudgesPath}{judge:00}.lock");
                    File.Delete($"{this._path}{MainPath}{nomination}\\{ResultsPath}{ReultsName(tour, judge)}.lock");
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }

        }

        public bool IsJudgeLocked(string nomination, int tour, int num)
            => File.Exists($"{this._path}{MainPath}{nomination}\\{JudgesPath}{num:00}.lock")
            || File.Exists($"{this._path}{MainPath}{nomination}\\{ResultsPath}{ReultsName(tour, num)}.lock");

        public IEnumerable<DanceModel> GetDances(string nomination)
        {
            int tDance = 1;
            List<DanceModel> result = new List<DanceModel>();
            try
            {
                while (File.Exists($"{this._path}{MainPath}{nomination}\\{DanceFileName}{tDance:00}.txt"))
                {
                    tDance++;
                    string fileContent = File.ReadAllText($"{this._path}{MainPath}{nomination}\\{DanceFileName}{tDance:00}.txt");
                    if (string.IsNullOrWhiteSpace(fileContent))
                        continue;

                    string[] danceContent = Clear(fileContent).Split('\n');
                    IEnumerable<TurnModel> turns =
                        danceContent.Select(turnContent => turnContent.Split(';')).Select(pairs => new TurnModel()
                        {
                            Dance = pairs[0].Trim('"'),
                            Pairs = pairs.Skip(1).Select(int.Parse).Where(p => p != 0)
                        }).TakeWhile(t => t.Dance.Length > 0);

                    result.Add(new DanceModel()
                    {
                        Turns = turns
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return result.Any() ? result : null;
        }

        public DanceModel GetDance(string nomination, int num)
        {
            try
            {
                string fileContent = File.ReadAllText($"{this._path}{MainPath}{nomination}\\{DanceFileName}{num:00}.txt");
                if (string.IsNullOrWhiteSpace(fileContent))
                    return null;

                string[] danceContent = Clear(fileContent).Split('\n');
                IEnumerable<TurnModel> turns =
                    danceContent.Select(turnContent => turnContent.Split(';')).Select(pairs => new TurnModel()
                    {
                        Dance = pairs[0].Trim('"'),
                        Pairs = pairs.Skip(1).Select(int.Parse).Where(p => p != 0)
                    }).TakeWhile(t => t.Dance.Length > 0);

                return new DanceModel()
                {
                    Turns = turns
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return null;
            }
        }

        public IEnumerable<string> GetShaResults(string nomination, int tour, int judge)
        {
            try
            {
                if (!File.Exists($"{this._path}{MainPath}{nomination}\\{ShaFileName}"))
                    return null;

                string fileContent = File.ReadAllText($"{this._path}{MainPath}{nomination}\\{ShaFileName}");
                if (string.IsNullOrWhiteSpace(fileContent))
                    return null;

                IEnumerable<string> shaContent =
                    Clear(fileContent).Split('\n').Where(sha=>sha.Length>0).Select(sha => sha.Substring(6, sha.IndexOf('.') - 6));

                return (from dance in shaContent
                        where File.Exists($"{this._path}{MainPath}{nomination}\\{ResultsPath}{ReultsName(tour, judge, dance)}.txt")
                        select File.ReadAllText($"{this._path}{MainPath}{nomination}\\{ResultsPath}{ReultsName(tour, judge, dance)}.txt"));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return null;
            }
        }

        #region Private metods

        private static string Clear(string s)
        {
            string result = (s[0] == (char)65279) ? s.Remove(0, 1) : s;
            return result.Replace("\r", string.Empty);
        }


        private static string ReultsName(int tour, int judge, string dance = null)
            => $"t{tour:00}j{judge:00}{(dance == null ? string.Empty : "_" + dance)}";

        #endregion

    }
}