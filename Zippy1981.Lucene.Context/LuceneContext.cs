using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Zippy1981.Lucene.Context
{
    public class LuceneContext
    {
        private readonly ILogger _logger;

        /// <summary>
        /// <see cref="SemaphoreSlim"/> used to lock <see cref="GetReaderAsync"/>.
        /// </summary>
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        private readonly Analyzer _analyzer;

        private DirectoryReader _indexReader;

        private readonly LuceneVersion _version;

        private readonly IndexWriter _writer;

        /// <summary>
        /// Gets or sets the maximum results returned in a query.
        /// </summary>
        public int MaxResults { get; set; } = 50;

        public LuceneContext(Directory directory, Analyzer analyzer, ILogger<LuceneContext> logger = null)
        {
            _version = LuceneVersion.LUCENE_48;
            _analyzer = analyzer;
            _logger = logger ?? NullLoggerProvider.Instance.CreateLogger(typeof(LuceneContext).FullName);

            var iwc = new IndexWriterConfig(_version, _analyzer)
            {
                OpenMode = OpenMode.CREATE_OR_APPEND,
            };
            _writer = new IndexWriter(directory, iwc);
            _writer.Commit();
            _indexReader = DirectoryReader.Open(directory);
        }

        public void AddDocument(Document document)
        {
            _writer.AddDocument(document);
        }

        public void CommitWriter() => _writer.Commit();

        public async Task<Document> GetDocumentByIdAsync(int id) => (await GetReaderAsync()).Document(id);

        public Query GetQuery(IEnumerable<string> fields, string queryText)
            => GetMultiFieldQueryParser(fields).Parse(queryText);

        public async Task<TopDocs> ExecQueryAsync(Query query) => (await GetIndexSearcherAsync()).Search(query, MaxResults);

        private async Task<IndexSearcher> GetIndexSearcherAsync() => new IndexSearcher(await GetReaderAsync());

        private async Task<IndexReader> GetReaderAsync()
        {
            try
            {
                await _semaphoreSlim.WaitAsync();
                var newReader = DirectoryReader.OpenIfChanged(_indexReader);
                if (newReader != null)
                {
                    _indexReader.Dispose();
                    _indexReader = newReader;
                }

                return _indexReader;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private QueryParser GetMultiFieldQueryParser(IEnumerable<string> fields) 
            => new MultiFieldQueryParser(_version, fields.ToArray(), _analyzer);

        public void UpsertDocument(string name, string value, Document document)
        {
            _writer.UpdateDocument(new Term(name, value), document);
        }
    }
}
