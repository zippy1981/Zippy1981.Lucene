using Lucene.Net.Analysis.Standard;
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
    }
}