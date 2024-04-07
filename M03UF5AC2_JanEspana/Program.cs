using System;
using System.Globalization;
using CsvHelper;
using System.Xml.Linq;
using System.Xml;
namespace M03UF5AC2_JanEspana
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Welcome to the register of water consume in Catalonia");
            if (XMLMethods.DetectIfXmlFileExists())
            {
                Console.WriteLine("The XML file already exists.");
            }
            else
            {
                XMLMethods.CreateXmlFileRoot();
                CSVMethods.ReadCsv();
            }
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. Identify the regions with a population greater than 200,000");
            Console.WriteLine("2. Calculate the average of consume of water");
            Console.WriteLine("3. Select the most and the least comarca in consume per capita");
            Console.WriteLine("4. Filter by name");
            Console.WriteLine("5. Filter by code");
            Console.WriteLine("6. Exit");
            string option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    XMLMethods.IdentifyPoblacioMoreThan200000();
                    break;
                case "2":
                    XMLMethods.CalculateAvgConsumOfWater();
                    break;
                case "3":
                    Console.WriteLine("Comarca with the most consume per capita: ");
                    XMLMethods.ShowHighestConsumPerCapita();
                    Console.WriteLine("Comarca with the least consume per capita: ");
                    XMLMethods.ShowLowestConsumPerCapita();
                    break;
                case "4":
                    Console.WriteLine("Enter the name of the comarca:");
                    string comarca = Console.ReadLine();
                    XMLMethods.FilterByComarca(comarca.ToUpper());
                    break;
                case "5":
                    Console.WriteLine("Enter the code of the comarca:");
                    XMLMethods.FilterByCodiComarca(int.Parse(Console.ReadLine()));
                    break;
                case "6":
                    Console.WriteLine("Goodbye");
                    break;
                default:
                    Console.WriteLine("Invalid option");
                    Main();
                    break;
            }
        } 
    }
}