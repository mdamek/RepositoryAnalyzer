using System.Collections.Generic;
using System.Linq;
using AnalyzeManager.Models;
using LibGit2Sharp;

namespace AnalyzeManager
{
    public class GitProvider
    {
        private readonly string _pathToLocalRepository;

        public GitProvider(string pathToRepository)
        {
            _pathToLocalRepository = pathToRepository;
        }

        public List<MetricsModel> AddCommitsMetrics(List<MetricsModel> filesContainer)
        {
            using (var repo = new Repository(_pathToLocalRepository))
            {
                var allCommits = repo.Commits.QueryBy(new CommitFilter {SortBy = CommitSortStrategies.Time});
                foreach (var commit in allCommits)
                {
                    if (commit.Parents.Any())
                    {
                        var old = commit.Parents.First().Tree;
                        var changes = repo.Diff.Compare<TreeChanges>(old, commit.Tree);
                        foreach (var change in changes)
                        {
                            AddChangeToFile(change.Path.Split('/').Last(), filesContainer);
                        }
                    }
                    else
                    {
                        foreach (var tree in commit.Tree)
                        {
                            LocateChangedFiles(tree, filesContainer);
                        }
                    }
                }
            }

            return filesContainer;
        }

        private void AddChangeToFile(string changedFileName, List<MetricsModel> filesContainer)
        {
            if (filesContainer.Any(e => e.FileFullName.Split("\\").Last() == changedFileName))
            {
                var changedFileIndex =
                    filesContainer.FindIndex(e => e.FileFullName.Split("\\").Last() == changedFileName);
                filesContainer[changedFileIndex].AllCommitsNumber += 1;
            }
        }

        private void LocateChangedFiles(TreeEntry treeEntry, List<MetricsModel> filesContainer)
        {
            if (treeEntry.Mode == Mode.Directory)
            {
                foreach (var child in ((Tree) treeEntry.Target))
                {
                    LocateChangedFiles(child, filesContainer);
                }
            }
            else
            {
                AddChangeToFile(treeEntry.Path, filesContainer);
            }
                
        }
    }
}

