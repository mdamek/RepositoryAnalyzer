using System;
using System.Collections.Generic;
using System.Linq;
using AnalyzeManager.Models;
using LibGit2Sharp;

namespace AnalyzeManager
{
    public class GitManager
    {
        private readonly string _pathToLocalRepository;

        public GitManager(string pathToRepository)
        {
            _pathToLocalRepository = pathToRepository;
        }

        public List<FileCodeStatistics> AddCommitsNumbersToFiles(List<FileCodeStatistics> filesContainer)
        {
            using (var repo = new Repository(_pathToLocalRepository))
            {
                var allCommits = repo.Commits;
                foreach (var commit in allCommits)
                {
                    if (commit.Parents.Any())
                    {
                        var old = commit.Parents.First().Tree;
                        var changes = repo.Diff.Compare<TreeChanges>(old, commit.Tree);
                        foreach (var change in changes)
                        {
                            var path = change.Path.Split('/').Last();
                            if(filesContainer.Any(e => e.FileFullName.Contains(path)))
                            {
                                var index = filesContainer.FindIndex(e => e.FileFullName.Contains(path));
                                filesContainer[index].AllCommitsNumber += 1;
                            }
                        }
                    }
                    else
                    {
                        
                    }
                   
                }
;
            }

            return filesContainer;
        }

    }
}
