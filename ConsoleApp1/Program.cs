using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        private static readonly string key = Values.getKey();
        private static readonly string endpoint = Values.getEndpoint();

        static void Main(string[] args)
        {
            string filename = @"C:\Users\grace\source\repos\ConsoleApp1\ConsoleApp1\whatstheweatherlike.wav";

            if (args.Length > 0)
            {
                filename = args[0];
            }

            var db_client = new MongoClient(Values.GetDatabase());
            var db = db_client.GetDatabase("test");
            var collection = db.GetCollection<BsonDocument>("audio");

            AudioToText audio = new AudioToText();
            audio.RecognizeSpeech(filename).Wait();
            List<string> sentences = audio.sentences;

            SentimentAnalysis analyzer = new SentimentAnalysis();
            foreach (string sentence in sentences)
            {
                analyzer.Analyze(endpoint, key, sentence).Wait();
            }

            KeyPhraseExtraction phrases = new KeyPhraseExtraction();
            phrases.Extract(endpoint, key, sentences).Wait();

            var document = new BsonDocument { { "date", 10000 }, { "sentences", analyzer.array }, { "key_phrases", phrases.array } };

            //SentimentAnalysisExample(client);
            //KeyPhraseExtractionExample(client);

            collection.InsertOne(document);

            Console.Write("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
