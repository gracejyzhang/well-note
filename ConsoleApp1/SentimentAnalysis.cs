using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class SentimentAnalysis
    {
        public BsonArray array;
        private int id = 1;

        public SentimentAnalysis() {
            array = new BsonArray();
        }

        public async Task Analyze(string endpoint, string key, string sentence)
        {
            var credentials = new ApiKeyServiceClientCredentials(key);
            var client = new TextAnalyticsClient(credentials)
            {
                Endpoint = endpoint
            };

            var result = client.Sentiment(sentence, "en");

            array.Add(new BsonDocument { { "id", id }, { "sentence", sentence }, { "score", result.Score} });
            id++;
            Console.WriteLine($"Sentiment Score: {result.Score:0.00}");

            //// The documents to be analyzed. Add the language of the document. The ID can be any value.
            //var inputDocuments = new MultiLanguageBatchInput(
            //    new List<MultiLanguageInput>
            //    {
            //        new MultiLanguageInput("1", "I had the best day of my life.", "en"),
            //        new MultiLanguageInput("2", "This was a waste of my time. The speaker put me to sleep.", "en"),
            //        new MultiLanguageInput("3", "No tengo dinero ni nada que dar...", "es"),
            //        new MultiLanguageInput("4", "L'hotel veneziano era meraviglioso. È un bellissimo pezzo di architettura.", "it"),
            //    });

            //var result = await client.SentimentBatchAsync(inputDocuments);

            //// Printing sentiment results
            //Console.WriteLine("===== Sentiment Analysis =====\n");

            //foreach (var document in result.Documents)
            //{
            //    Console.WriteLine($"Document ID: {document.Id} , Sentiment Score: {document.Score:0.00}");
            //}
            //Console.WriteLine();
        }
    }
}
