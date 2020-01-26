using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        private static readonly string key = Values.getKey();
        private static readonly string endpoint = Values.getEndpoint();

        static void Main(string[] args)
        {
            var db_client = new MongoClient(Values.GetDatabase());
            var db = db_client.GetDatabase("wellnote-entries");
            var res_collection = db.GetCollection<BsonDocument>("analysis");
            var audio_collection = db.GetCollection<BsonDocument>("audio");
            var links = audio_collection.Find(new BsonDocument()).ToList();

            using (var client = new WebClient())
            {
                foreach (BsonDocument link in links)
                {
                    string filename = @"C:\Users\grace\source\repos\ConsoleApp1\ConsoleApp1\" + link.GetValue("date").ToString() + ".wav";
                    client.DownloadFile(link.GetValue("audio").ToString(), filename);

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

                    var document = new BsonDocument { { "date", link.GetValue("date") }, { "sentences", analyzer.array }, { "key_phrases", phrases.array } };

                    res_collection.InsertOne(document);
                }
            }

            Console.Write("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
