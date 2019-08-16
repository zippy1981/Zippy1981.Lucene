using System.Linq;
using System.Threading.Tasks;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Store;
using Lucene.Net.Util;
using NUnit.Framework;

namespace Zippy1981.Lucene.Context
{
    public class TestLuceneContext
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void TestNullLogger()
        {
            var actual = new LuceneContext(new RAMDirectory(), new StandardAnalyzer(LuceneVersion.LUCENE_48));
            Assert.AreEqual(50, actual.MaxResults);
        }

        [Test]
        public void TestMaxResults()
        {
            var actual = new LuceneContext(new RAMDirectory(), new StandardAnalyzer(LuceneVersion.LUCENE_48));
            Assert.AreEqual(50, actual.MaxResults);
            actual.MaxResults = 25;
            Assert.AreEqual(25, actual.MaxResults);
        }

        [Test]
        public async Task TestAddAndUpdateDocument()
        {
            var actual = new LuceneContext(new RAMDirectory(), new StandardAnalyzer(LuceneVersion.LUCENE_48));
            var document = new Document();
            document.AddStringField("key", "value", Field.Store.YES);
            actual.AddDocument(document);
            actual.CommitWriter();
            var query = actual.GetQuery(new[] {"key"}, "value");
            var results = await actual.ExecQueryAsync(query);
            Assert.AreEqual(1, results.TotalHits);
            var doc = await actual.GetDocumentByIdAsync(results.ScoreDocs.First().Doc);
            Assert.AreEqual(1, doc.Fields.Count);
            Assert.AreEqual("value", doc.GetField("key").GetStringValue());
        }
    }
}