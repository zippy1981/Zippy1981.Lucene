using System;
using System.Threading.Tasks;
using global::Lucene.Net.Documents;
using Octokit;

namespace Zippy1981.Lucene.Document.Octokit
{
    /// <summary>
    /// Lucene related extension methods for the <see cref="Octokit"/> namespace.
    /// </summary>
    public static class OctokitExtensions
    {
        public static global::Lucene.Net.Documents.Document ToDocument(this Repository repo)
        {
            var document = new global::Lucene.Net.Documents.Document();
            document.AddStoredField(nameof(repo.Id), repo.Id);
            document.AddStoredField(nameof(repo.DefaultBranch), repo.DefaultBranch);
            if (repo.Homepage != null)
            {
                document.AddStoredField(nameof(repo.Homepage), repo.Homepage);
            }

            if (repo.License?.Name != null)
            {
                document.AddStoredField($"{nameof(repo.License)}.{nameof(repo.License.Name)}", repo.License.Name);
            }

            document.AddStoredField(nameof(repo.ForksCount), repo.ForksCount);
            document.AddStoredField(nameof(repo.SubscribersCount), repo.SubscribersCount);
            document.AddTextField(nameof(repo.FullName), repo.FullName, Field.Store.YES);
            if (repo.Description != null)
            {
                document.AddTextField(nameof(repo.Description), repo.Description, Field.Store.YES);
            }

            return document;
        }

        public static async Task<global::Lucene.Net.Documents.Document> AddReadMeAsync(this global::Lucene.Net.Documents.Document document, Readme readme)
        {
            if (readme?.Content != null)
            {
                document.AddStoredField(OctokitLuceneUtils.ReadmeFileNameField, readme.Name);
                document.AddTextField(OctokitLuceneUtils.ReadmeField, readme.Content, Field.Store.NO);
                document.AddStoredField(OctokitLuceneUtils.ReadmeHtmlField, (await readme.GetHtmlContent()) ?? readme.Content);
            }

            return document;
        }
    }
}
