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

        [Test]
        public async Task TestUpsertDocument_KeyValue_InsertNew()
        {
            var actual = new LuceneContext(new RAMDirectory(), new StandardAnalyzer(LuceneVersion.LUCENE_48));
            var document = new Document();
            document.AddStringField("key", "value", Field.Store.YES);
            actual.UpsertDocument("key", "value", document);
            actual.CommitWriter();
            var query = actual.GetQuery(new[] { "key" }, "value");
            var results = await actual.ExecQueryAsync(query);
            Assert.AreEqual(1, results.TotalHits);
            var doc = await actual.GetDocumentByIdAsync(results.ScoreDocs.First().Doc);
            Assert.AreEqual(1, doc.Fields.Count);
            Assert.AreEqual("value", doc.GetField("key").GetStringValue());
        }

        [Test]
        public async Task TestUpsertDocument_Value_Replace()
        {
            var actual = new LuceneContext(new RAMDirectory(), new StandardAnalyzer(LuceneVersion.LUCENE_48));
            var document = new Document();
            document.AddStringField("key", "value", Field.Store.YES);
            document.AddStringField("key2", "other value", Field.Store.YES);
            actual.AddDocument(document);
            actual.CommitWriter();

            var replacementDocument = new Document();
            replacementDocument.AddStringField("key", "value", Field.Store.YES);
            replacementDocument.AddStringField("key2", "yet another value", Field.Store.YES);
            actual.UpsertDocument("key", replacementDocument);
            actual.CommitWriter();

            var query = actual.GetQuery(new[] { "key" }, "value");
            var results = await actual.ExecQueryAsync(query);
            Assert.AreEqual(1, results.TotalHits);
            var doc = await actual.GetDocumentByIdAsync(results.ScoreDocs.First().Doc);
            Assert.AreEqual(2, doc.Fields.Count);
            Assert.AreEqual("value", doc.GetField("key").GetStringValue());
            Assert.AreEqual("yet another value", doc.GetField("key2").GetStringValue());
        }

        [Test]
        public async Task TestUpsertDocument_Key_InsertNew()
        {
            var actual = new LuceneContext(new RAMDirectory(), new StandardAnalyzer(LuceneVersion.LUCENE_48));
            var document = new Document();
            document.AddStringField("key", "value", Field.Store.YES);
            actual.UpsertDocument("key", document);
            actual.CommitWriter();
            var query = actual.GetQuery(new[] { "key" }, "value");
            var results = await actual.ExecQueryAsync(query);
            Assert.AreEqual(1, results.TotalHits);
            var doc = await actual.GetDocumentByIdAsync(results.ScoreDocs.First().Doc);
            Assert.AreEqual(1, doc.Fields.Count);
            Assert.AreEqual("value", doc.GetField("key").GetStringValue());
        }

        [Test]
        public async Task TestUpsertDocument_KeyValue_Replace()
        {
            var actual = new LuceneContext(new RAMDirectory(), new StandardAnalyzer(LuceneVersion.LUCENE_48));
            var document = new Document();
            document.AddStringField("key", "value", Field.Store.YES);
            document.AddStringField("key2", "other value", Field.Store.YES);
            actual.AddDocument(document);
            actual.CommitWriter();

            var replacementDocument = new Document();
            replacementDocument.AddStringField("key", "value", Field.Store.YES);
            replacementDocument.AddStringField("key2", "yet another value", Field.Store.YES);
            actual.UpsertDocument("key", "value", replacementDocument);
            actual.CommitWriter();

            var query = actual.GetQuery(new[] { "key" }, "value");
            var results = await actual.ExecQueryAsync(query);
            Assert.AreEqual(1, results.TotalHits);
            var doc = await actual.GetDocumentByIdAsync(results.ScoreDocs.First().Doc);
            Assert.AreEqual(2, doc.Fields.Count);
            Assert.AreEqual("value", doc.GetField("key").GetStringValue());
            Assert.AreEqual("yet another value", doc.GetField("key2").GetStringValue());
        }
    }
}