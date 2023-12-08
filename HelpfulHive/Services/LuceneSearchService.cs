using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Lucene.Net.Analysis.Ru; 
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpfulHive.Models; 


namespace HelpfulHive.Services
{
    public class LuceneSearchService
    {
        private readonly RAMDirectory _directory;
        private readonly Lucene.Net.Analysis.Analyzer _analyzer;
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public LuceneSearchService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
            _directory = new RAMDirectory();
            _analyzer = new RussianAnalyzer(LuceneVersion.LUCENE_48);
        }

        public async Task StartIndexingProcess()
        {
            using var context = _contextFactory.CreateDbContext();
            var rawDataList = await context.RawDatas.ToListAsync();

            var config = new IndexWriterConfig(LuceneVersion.LUCENE_48, _analyzer);
            using var writer = new IndexWriter(_directory, config);

            foreach (var rawData in rawDataList)
            {
                var doc = new Document
            {
                new TextField("Comments", rawData.Comments ?? "", Field.Store.YES),
                new TextField("Subject", rawData.Subject ?? "", Field.Store.YES),
                new TextField("Inquiry", rawData.Inquiry ?? "", Field.Store.YES),
                new TextField("Response", rawData.Response ?? "", Field.Store.YES),
                new StringField("RequestNumber", rawData.RequestNumber ?? "", Field.Store.YES)
            };
                writer.AddDocument(doc);
            }
        }

        public async Task<List<RawData>> GetRecordsByRequestNumbers(List<string> requestNumbers)
        {
            using var context = _contextFactory.CreateDbContext();
            var rawDataList = await context.RawDatas
                                           .Where(r => requestNumbers.Contains(r.RequestNumber))
                                           .ToListAsync();

            // Сортировка результатов в соответствии с порядком в requestNumbers
            return requestNumbers.Select(requestNumber =>
                rawDataList.FirstOrDefault(r => r.RequestNumber == requestNumber)).ToList();
        }


        public List<string> Search(string searchText)
        {
            using var reader = DirectoryReader.Open(_directory);
            var searcher = new IndexSearcher(reader);

            var parser = new MultiFieldQueryParser(LuceneVersion.LUCENE_48,
                new[] { "Comments", "Subject", "Inquiry", "Response" }, _analyzer);
            var query = parser.Parse(searchText);

            var hits = searcher.Search(query, 10).ScoreDocs;
            return hits.Select(hit => searcher.Doc(hit.Doc).Get("RequestNumber")).ToList();
        }


   

    }
}
