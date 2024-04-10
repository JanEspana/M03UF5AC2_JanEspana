using System;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using System.Xml.Linq;

namespace M03UF5AC2_JanEspana
{
    public class CSVMethods
    {
        public static void ReadCsv()
        {   
            using var reader = new System.IO.StreamReader(@"../../../Consum_d_aigua_a_Catalunya_per_comarques_20240402.csv");
            var options = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
            };
            using var csv = new CsvReader(reader, options);
            var records = csv.GetRecords<Consum>();
            foreach (var record in records)
            {
                XMLMethods.AddRecordToXml(record);
            }

            Console.WriteLine("CSV file read, XML completed");
        }
    }
    public class XMLMethods
    {
        public static bool DetectIfXmlFileExists()
        {
            return File.Exists("ConsumAiguaCatalunya.xml");
        }
        public static void CreateXmlFileRoot()
        {
            XDocument xmlDoc = new XDocument(
            new XElement("Consums"));
            string path = "ConsumAiguaCatalunya.xml";
            xmlDoc.Save(path);

            Console.WriteLine("XML file created. Transfeering CSV information to XML...");
        }
        public static void AddRecordToXml(Consum record)
        {
            XDocument xmlDoc = XDocument.Load("ConsumAiguaCatalunya.xml");
            XElement root = xmlDoc.Element("Consums");
            root.Add(new XElement("Consum",
            new XElement("Any", record.Any),
                new XElement("CodiComarca", record.CodiComarca),
                new XElement("Comarca", WriteComarca(record.Comarca)),
                new XElement("Poblacio", record.Poblacio),
                new XElement("DomesticXarxa", record.DomesticXarxa),
                new XElement("ActivitatsEconomiquesIFontsPropies", record.ActivitatsEconomiquesIFontsPropies),
                new XElement("Total", record.Total),
                new XElement("ConsumDomesticPerCapita", record.ConsumDomesticPerCapita)
                ));
            xmlDoc.Save("ConsumAiguaCatalunya.xml");
        }
        public static string WriteComarca(string comarca)
        {
            //dividir por coma
            string[] comarcaArray = comarca.Split(',');
            return comarcaArray[0];
        }
        public static void IdentifyPoblacioMoreThan200000()
        {

            XDocument xmlDoc = XDocument.Load("ConsumAiguaCatalunya.xml");
            var query = from x in xmlDoc.Descendants("Consum")
                        where (int)x.Element("Poblacio") > 200000
                        group x by (string)x.Element("Comarca") into g
                        select g.First();
            Console.WriteLine("Comarques amb població superior a 200000:");
            foreach (var item in query)
            {
                Console.WriteLine(item.Element("Comarca").Value);
            }
        }
        public static void CalculateAvgConsumOfWater()
        {
            XDocument xmlDoc = XDocument.Load("ConsumAiguaCatalunya.xml");
            var query = from x in xmlDoc.Descendants("Consum")
                        group x by new { Any = x.Element("Any").Value, Comarca = x.Element("Comarca").Value } into g
                        select new
                        {
                            Any = g.Key.Any,
                            Comarca = g.Key.Comarca,
                            AvgConsum = g.Average(x => (double)x.Element("ConsumDomesticPerCapita") * (int)x.Element("Poblacio"))
                        };
            foreach (var item in query)
            {
                Console.WriteLine(item.Any + " " + item.Comarca + " " + item.AvgConsum);
            }
        }
        public static void ShowHighestConsumPerCapita()
        {
            XDocument xmlDoc = XDocument.Load("ConsumAiguaCatalunya.xml");
            var query = (from x in xmlDoc.Descendants("Consum")
                         orderby (double)x.Element("ConsumDomesticPerCapita") descending select x).Take(5);
            foreach (var item in query)
            {
                Console.WriteLine(item.Element("Comarca").Value);
            }
        }
        public static void ShowLowestConsumPerCapita()
        {
            //selecciona l'any i la comarca amb el consum per capita més baix
            XDocument xmlDoc = XDocument.Load("ConsumAiguaCatalunya.xml");
            var query = (from x in xmlDoc.Descendants("Consum")
                         orderby (double)x.Element("ConsumDomesticPerCapita") ascending select x).Take(5);
            foreach (var item in query)
            {
                Console.WriteLine(item.Element("Comarca").Value);
            }
        }
        public static void FilterByCodiComarca(int codiComarca)
        {
            XDocument xmlDoc = XDocument.Load("ConsumAiguaCatalunya.xml");
            var query = from x in xmlDoc.Descendants("Consum")
                        where (int)x.Element("CodiComarca") == codiComarca
                        orderby (int)x.Element("Any") ascending
                        select x;
            foreach (var item in query)
            {
                Console.WriteLine(item.Element("Any").Value + " " + item.Element("CodiComarca").Value + " " + item.Element("Comarca").Value + " " + item.Element("Poblacio").Value + " " + item.Element("DomesticXarxa").Value + " " + item.Element("ActivitatsEconomiquesIFontsPropies").Value + " " + item.Element("Total").Value + " " + item.Element("ConsumDomesticPerCapita").Value);
            }
        }
        public static void FilterByComarca(string comarca)
        {
            XDocument xmlDoc = XDocument.Load("ConsumAiguaCatalunya.xml");
            var query = from x in xmlDoc.Descendants("Consum")
                        where x.Element("Comarca").Value.Contains(comarca)
                        orderby (int)x.Element("Any") ascending
                        select x;
            foreach (var item in query)
            {
                Console.WriteLine(item.Element("Any").Value + " " + item.Element("CodiComarca").Value + " " + item.Element("Comarca").Value + " " + item.Element("Poblacio").Value + " " + item.Element("DomesticXarxa").Value + " " + item.Element("ActivitatsEconomiquesIFontsPropies").Value + " " + item.Element("Total").Value + " " + item.Element("ConsumDomesticPerCapita").Value);
            }
        }
    }
}
