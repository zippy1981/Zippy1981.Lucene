using System.Collections.Generic;
using System.Threading.Tasks;
using Lucene.Net.Documents;
using Lucene.Net.Search;

namespace Zippy1981.Lucene.Context
{
    /// <remarks>This is mainly for mocking in unit tests.</remarks>>
    public interface ILuceneContext
    {

        /// <summary>Gets or sets the maximum results returned in a query.</summary>
        int MaxResults { get; set; }

        void AddDocument(Document document);
        void CommitWriter();
        Task<Document> GetDocumentByIdAsync(int id);
        Query GetQuery(IEnumerable<string> fields, string queryText);
        Task<TopDocs> ExecQueryAsync(Query query);
        void UpsertDocument(string name, string value, Document document);
    }
}