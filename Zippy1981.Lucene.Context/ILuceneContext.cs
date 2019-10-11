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
        
        /// <summary>
        /// Commits the current <c>IndexWriter</c>
        /// </summary>
        void CommitWriter();
        
        /// <summary>
        /// Gets a document based on its id
        /// </summary>
        /// <param name="id">The document Id in the <c>Directory</c></param>
        /// <returns>The <see cref="Document"/> represented by <paramref name="id"/>.</returns>
        Task<Document> GetDocumentByIdAsync(int id);

        /// <summary>
        /// Gets a <see cref="Query"/> object.
        /// </summary>
        /// <param name="fields">The field names to query.</param>
        /// <param name="queryText">The query text.</param>
        /// <returns></returns>
        Query GetQuery(IEnumerable<string> fields, string queryText);
        
        /// <summary>
        /// Executes the <paramref name="query"/>
        /// </summary>
        /// <param name="query">The query to run.</param>
        /// <returns></returns>
        Task<TopDocs> ExecQueryAsync(Query query);

        /// <summary>
        /// Inerts or replaces a document based on a matched field name.
        /// </summary>
        /// <param name="name">The name of the field to search against.</param>
        /// <param name="value">The value of the <paramref name="name"/></param>
        /// <param name="document">The document to insert in place of the match or as a new document.</param>
        void UpsertDocument(string name, string value, Document document);
    }
}