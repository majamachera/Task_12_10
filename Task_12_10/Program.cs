using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Task_12_10
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int count = 0;
            string date = DateTime.Now.ToString("yyyyMMddTHH_mm_ssZ");
            string path2 = @$"c:\oferts{date}.xml";
            string path3 = @$"c:\transactions{count}.xml";
            int value = 0;
            string instrument = "";
            int Watcher = 0;
            int consoleMenu = 0;
            int TransactionFilter = 0;
            List<string> instruments = new List<string>();
            List<StockOffer> oldOfferts;
            List<StockOffer> oldTransactions = null;
            List<StockOffer> transactions;
            List<StockOffer> AllTransactions = null;
            Instruments(instruments);
            var Offerts = RandomMaker(instruments);
            Console.WriteLine("Witamy na gieldzie:");
            string oldPathOfferts = Serialize<StockOffer>(Offerts, path2);
            bool consoleMode = true;
            bool WatcherMode = true;
            bool TransactionMode = false;

            while (consoleMode)
            {
                Writer();
                try
                {
                    consoleMenu = Convert.ToInt32(Console.ReadLine());


                    switch (consoleMenu)
                    {
                        case 1:
                            InstrumentsIteration(instruments);

                            break;
                        case 2:
                            oldOfferts = DeSerialized<StockOffer>(oldPathOfferts);
                            Console.WriteLine("Podaj przedzial czasowy wedlug wzoru: [data poczatkowa]-[data koncowa]");
                            Console.WriteLine("Twoj przedzial musi zawierac sie w przedziale liczbowym  1-1000, na przyklad: 20-100");
                            string fromUserDate = Console.ReadLine();
                            fromUserDate = fromUserDate.Trim(' ');
                            string[] fromUserDateArray = fromUserDate.Split('-');
                            int date1 = Convert.ToInt32(fromUserDateArray[0]);
                            int date2 = Convert.ToInt32(fromUserDateArray[1]);
                            if (date1 < 0 || date2 < 0 || date1 > 1000 || date2 > 1000)
                            {
                                Console.WriteLine("Przedzial nie zawiera sie w przedziale 1-1000");
                            }
                            else
                            {
                                if (date1 <= date2)
                                {
                                    OldOfferts(oldOfferts, date1, date2);
                                    Console.WriteLine();
                                }
                                else
                                {
                                    Console.WriteLine("Data poczatkowa musi byc mniejsza lub rowna koncowej");
                                }
                            }
                            break;
                        case 3:
                            if (WatcherMode)
                            {
                                Console.WriteLine("1.Czuwaj na oferty ponizej wartosci");
                                Console.WriteLine("2.Czuwaj na oferty konkretnego instrumentu");
                                Console.WriteLine("3.Czuwaj na oferty konkretnego instrumentu ponizej wartosci");
                                try
                                {
                                    Watcher = Convert.ToInt32(Console.ReadLine());

                                    switch (Watcher)
                                    {
                                        case 1:
                                            Console.WriteLine("Podaj wartosc");
                                            try
                                            {
                                                value = Convert.ToInt32(Console.ReadLine());

                                                WatcherMode = false;
                                                Console.WriteLine($"Czuwasz na wartosci ponizej: {value}");
                                            }
                                            catch (OverflowException)
                                            {
                                                Console.WriteLine("value is outside the range of the Int32 type.", value);
                                            }
                                            catch (FormatException)
                                            {
                                                Console.WriteLine("value is outside the range of the Int32 format.", Watcher);
                                            }
                                            break;
                                        case 2:
                                            Console.WriteLine("Podaj instrument");
                                            instrument = (Console.ReadLine()).ToUpper();
                                            if (instruments.Contains(instrument))
                                            {

                                                WatcherMode = false;
                                                Console.WriteLine($"Czuwasz na: {instrument}");
                                            }
                                            else
                                            {
                                                Console.WriteLine("Nie ma takiego instrumentu");
                                            }

                                            break;
                                        case 3:
                                            Console.WriteLine("Podaj instrument i wartosc wedlug wzoru: [instrument] [wartosc]");
                                            string fromUser = Console.ReadLine();
                                            string[] fromUserArray = fromUser.Split(" ");
                                            instrument = (fromUserArray[0]).ToUpper();
                                            try
                                            {
                                                value = Convert.ToInt32(fromUserArray[1]);

                                                if (instruments.Contains(instrument))
                                                {
                                                    WatcherMode = false;
                                                    Console.WriteLine($"Czuwasz na: {instrument} {value}");
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Nie ma takiego instrumentu");
                                                }

                                            }
                                            catch (OverflowException)
                                            {
                                                Console.WriteLine("value is outside the range of the Int32 type.", value);
                                            }
                                            catch (FormatException)
                                            {
                                                Console.WriteLine("value is outside the range of the Int32 format.", value);
                                            }
                                            break;

                                        default:
                                            Console.WriteLine("Nie ma takiej opcji");

                                            break;
                                    }
                                }
                                catch (OverflowException)
                                {
                                    Console.WriteLine("Watcher is outside the range of the Int32 type.", Watcher);
                                }
                                catch (FormatException)
                                {
                                    Console.WriteLine("Watcher is outside the range of the Int32 format.", Watcher);
                                }

                                break;
                            }
                            else
                            {
                                Console.WriteLine("Wacher jest juz ustawiony. Wpisz 1 jesli chcesz zresetowac opcje. Wpisz 2 jesli chcesz powrocic do menu glownego");
                                if (Console.ReadLine() == "1")
                                {
                                    value = 0;
                                    instrument = "";
                                    WatcherMode = true;
                                    Watcher = 0;
                                    continue;
                                }
                            }
                            break;


                        case 4:
                            if (!WatcherMode)
                            {
                                Offerts = RandomMaker(instruments);
                                Iteration(Offerts);
                                TransactionMode = true;

                                switch (Watcher)
                                {
                                    case 1:
                                        transactions = Value(value, Offerts);
                                        Iteration(transactions);
                                        count++;
                                        path3 = @$"c:\transactions{count}.xml";
                                        path3 = Serialize(transactions, path3);
                                        break;
                                    case 2:
                                        transactions = Instrument(instrument, Offerts);
                                        Iteration(transactions);
                                        count++;
                                        path3 = @$"c:\transactions{count}.xml";
                                        path3 = Serialize(transactions, path3);
                                        break;
                                    case 3:
                                        transactions = InstrumentandValue(instrument, value, Offerts);
                                        Iteration(transactions);
                                        count++;
                                        path3 = @$"c:\transactions{count}.xml";
                                        path3 = Serialize(transactions, path3);
                                        break;
                                    default:

                                        break;
                                }


                            }
                            else
                            {
                                Console.WriteLine("Ustaw czuwanie przed wejsciem na gielde");
                            }

                            break;
                        case 5:
                            if (TransactionMode)
                            {
                                if (count == 1)
                                {
                                    AllTransactions = DeSerialized<StockOffer>(@$"c:\transactions1.xml");
                                }

                                else
                                {
                                    for (int i = 1; i <= count; i++)
                                    {
                                        if (i == 1)
                                        {
                                            oldTransactions = DeSerialized<StockOffer>(@$"c:\transactions1.xml");
                                        }
                                        else
                                        {
                                            List<StockOffer> newTransactions = DeSerialized<StockOffer>(@$"c:\transactions{i}.xml");
                                            AllTransactions = oldTransactions.Concat(newTransactions).ToList();

                                        }


                                    }
                                }
                                OldTransactions(AllTransactions);
                                Console.WriteLine("1.Pokaz transakcje ponizej wartosci");
                                Console.WriteLine("2.Pokaz transakcje instrumentu");
                                Console.WriteLine("3.Pokaz transakcje instrumentu ponizej wartosci");
                                try
                                {
                                    TransactionFilter = Convert.ToInt32(Console.ReadLine());
                                    int valueT;
                                    string instrumentT;
                                    switch (TransactionFilter)
                                    {
                                        case 1:
                                            Console.WriteLine("Podaj wartosc");
                                            valueT = Convert.ToInt32(Console.ReadLine());
                                            AllTransactions = Value(valueT, AllTransactions);
                                            Iteration(AllTransactions);
                                            break;
                                        case 2:
                                            Console.WriteLine("Podaj instrument");
                                            instrumentT = (Console.ReadLine()).ToUpper();
                                            if (instruments.Contains(instrumentT))
                                            {
                                                AllTransactions = Instrument(instrumentT, AllTransactions);
                                                Iteration(AllTransactions);
                                            }
                                            else
                                            {
                                                Console.WriteLine("Nie ma takiego instrumentu");
                                            }

                                            break;
                                        case 3:
                                            Console.WriteLine("Podaj instrument i wartosc wedlug wzoru: [instrument] [wartosc]");
                                            string fromUser = Console.ReadLine();
                                            string[] fromUserArray = fromUser.Split(" ");
                                            instrumentT = (fromUserArray[0]).ToUpper();
                                            valueT = Convert.ToInt32(fromUserArray[1]);
                                            if (instruments.Contains(instrumentT))
                                            {
                                                AllTransactions = InstrumentandValue(instrumentT, valueT, AllTransactions);
                                                Iteration(AllTransactions);
                                            }
                                            else
                                            {
                                                Console.WriteLine("Nie ma takiego instrumentu");
                                            }


                                            break;

                                        default:
                                            Console.WriteLine("Nie ma takiej opcji");

                                            break;

                                    }
                                }
                                catch (OverflowException)
                                {
                                    Console.WriteLine("TransactionFilter is outside the range of the Int32 type.", TransactionFilter);
                                }
                                catch (FormatException)
                                {
                                    Console.WriteLine("TransactionFilter is outside the range of the Int32 format.", TransactionFilter);
                                }
                            }

                            else
                            {
                                Console.WriteLine("Nie masz jeszcze transakcji");
                            }
                            break;

                        case 6:
                            consoleMode = false;

                            break;
                        default:
                            Console.WriteLine("Nie ma takiej opcji");
                            break;
                    }

                }
                catch (OverflowException)
                {
                    Console.WriteLine("consoleMenu is outside the range of the Int32 type.", consoleMenu);
                }
                catch (FormatException)
                {
                    Console.WriteLine("consoleMenu is outside the range of the Int32 format.", consoleMenu);
                }

            }


        }
        static void Writer()
        {

            Console.WriteLine("1.Lista Instrumentow");
            Console.WriteLine("2.Przeszle oferty");
            Console.WriteLine("3.Ustawienie czuwania");
            Console.WriteLine("4.Wchodze na gielde");
            Console.WriteLine("5.Dokonane transakcje");
            Console.WriteLine("6.Koniec");
        }

        static void Instruments(List<string> instruments)
        {
            string path = @"c:\instruments.txt";
            using (FileStream fsSource = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fsSource))
                {
                    string line;
                    {
                        while (sr.Peek() >= 0)
                        {
                            line = sr.ReadLine();
                            instruments.Add(line);
                        }
                    }
                }
            }
        }
        public static List<StockOffer> RandomMaker(List<string> instruments)
        {
            var Offerts = new List<StockOffer>();
            Random rnd = new Random();
            for (int i = 1; i <= 1000; i++)
            {
                int mIndex = rnd.Next(instruments.Count);
                int value = rnd.Next(10, 100);
                var offer = new StockOffer() { Data = i, Instrument = instruments[mIndex], Value = value };
                Offerts.Add(offer);
            }
            return Offerts;

        }
        static string Serialize<T>(List<T> offerts, string path)
        {


            XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
            using (FileStream fsSources = new FileStream(path, FileMode.CreateNew))
            {
                using (StreamWriter swr = new StreamWriter(fsSources))
                {
                    serializer.Serialize(swr, offerts);
                    swr.Close();
                }
            }
            return path;

        }
        public static List<T> DeSerialized<T>(string path)
        {
            List<T> type;
            var serializer = new XmlSerializer(typeof(List<T>));
            using (var reader = XmlReader.Create(path))
            {
                type = (List<T>)serializer.Deserialize(reader);
            }
            return type;
        }
        public static List<StockOffer> Value(int value, List<StockOffer> Offerts)
        {

            var sortedOferts = Offerts.Where(x => x.Value < value).OrderBy(x => x.Value).ToList();
            return sortedOferts;
        }
        public static List<StockOffer> Instrument(string instrument, List<StockOffer> Offerts)
        {
            var sortedOferts = Offerts.Where(x => x.Instrument == instrument).OrderBy(x => x.Data).ToList();
            return sortedOferts;
        }
        public static List<StockOffer> InstrumentandValue(string instrument, int value, List<StockOffer> Offerts)
        {
            var sortedOferts = Offerts.Where(x => x.Instrument == instrument && x.Value < value).OrderBy(x => x.Data).ToList();
            return sortedOferts;
        }
        public static void OldOfferts(List<StockOffer> OldOfferts, int date1, int date2)
        {
            var filtredOldOfferts = OldOfferts.Where(x => x.Data >= date1 && x.Data <= date2).OrderByDescending(x => x.Data);
            foreach (var item in filtredOldOfferts)
            {
                Console.WriteLine($"{item.Data} {item.Instrument} {item.Value}");
            }
        }
        public static void OldTransactions(List<StockOffer> OldTrans)
        {
            var filtredTransactions = OldTrans.OrderBy(x => x.Value);
            foreach (var item in filtredTransactions)
            {
                Console.WriteLine($"{item.Data} {item.Instrument} {item.Value}");
            }
        }


        static void InstrumentsIteration(List<string> instruments)
        {

            foreach (var item in instruments)
            {
                Console.WriteLine(item);
            }
        }
        static void Iteration(List<StockOffer> list)
        {

            foreach (var item in list)
            {
                Console.WriteLine($"{item.Data} {item.Instrument} {item.Value}");
            }
        }

    }

}










