using System.Collections.Generic;
using Octokit;

namespace Zippy1981.Lucene.Document.Octokit
{
    public static class OctokitLuceneUtils
    {
        public const string ReadmeFileNameField = "README_FILE_NAME";
        public const string ReadmeField = "README";
        public const string ReadmeHtmlField = "README_HTML";

        public static IEnumerable<string> GetRepositorySearchFields() => new[]
        {
            nameof(Repository.FullName),
            nameof(Repository.Description),
            nameof(ReadmeField),
        };
    }
}