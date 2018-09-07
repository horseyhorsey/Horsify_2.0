using Horsesoft.Music.Data.Model.Horsify;
using System;
using System.Collections.Generic;

namespace Horsesoft.Music.Horsify.SongServiceClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Push Q and enter to quit");
            Console.WriteLine("");            

            PrintInitialHelp();

            RunProgram();
            
        }

        private static void RunProgram()
        {
            var line = string.Empty;
            while ((line = Console.ReadLine()) != "q")
            {
                try
                {
                    var searchType = GetUserColumnType(line);
                    var amount = GetRandomTerm();
                    var searchTerm = GetSearchTerm();
                    var client = new HorsifyService.HorsifySongServiceClient("BasicHttpBinding_IHorsifySongService");

                    var filter = new SearchFilter()
                    {
                        Filters = new List<HorsifyFilter>()
                        {
                            new HorsifyFilter
                            {
                                SearchType = searchType,
                                Filters = new List<string> { $"{searchTerm}"}
                            }                            
                        }
                    };

                    System.Net.ServicePointManager.Expect100Continue = false;
                    var songs = client.SearchLikeFilters(filter, amount);
                    if (songs != null)
                    {
                        foreach (var item in songs)
                        {
                            Console.WriteLine(item.FileLocation);
                        }
                    }

                    PrintInitialHelp();
                }
                catch(Exception ex) { throw; }
            }
        }

        private static void PrintInitialHelp()
        {
            Console.WriteLine();
            Console.WriteLine("Enter menu number to search:");
            Console.WriteLine("0 - All");
            Console.WriteLine("1 - Artist");
            Console.WriteLine("2 - Label");
            Console.WriteLine("3 - Year");
        }

        private static string GetSearchTerm()
        {
            Console.WriteLine("Enter your search term...");
            return Console.ReadLine();
        }

        private static short GetRandomTerm()
        {
            Console.WriteLine("Enter random amount....0 for all");

            short amount = 0;
            while (!short.TryParse(Console.ReadLine(), out amount))
            {

            }
            return amount;
        }

        private static SearchType GetUserColumnType(string line)
        {
            int type = 0;
            int.TryParse(line, out type);
            if (type == 0)
                return SearchType.All;

            switch (type)
            {
                case 0:
                    return SearchType.All;
                case 1:
                    return SearchType.Artist;
                case 2:
                    return SearchType.Label;
                case 3:
                    return SearchType.Year;
                default:
                    return SearchType.All;
            }
        }

        static void PrintOptions()
        {
            Console.WriteLine("1:Artist");
            Console.WriteLine("2:Label");
        }
    }
}
