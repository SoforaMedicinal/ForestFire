using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ForestFireModel.ForestModel;

namespace ForestFireModel
{
    class Program
    {
        public static float f;
        public static float p;
        public static int height;
        public static int width;
        public static int interval;

        private static Timer timer;
        private Encoding ascii = Encoding.ASCII;
        static void Main(string[] args)
        {
            var parser = ParserValue();
            if (!parser.Succeeded)
            {
                Console.WriteLine(String.Join(Environment.NewLine, parser.Errors));
                Console.ReadLine();
                return;
            }

            ForestModel model;
            var createResult = ForestModel.Create(f, p, height, width);
            if (!createResult.Succeeded)
            {
                Console.WriteLine(string.Join(Environment.NewLine, createResult.Errors));
            }
            else
            {
                Console.OutputEncoding = System.Text.Encoding.GetEncoding(1252);
                model = createResult.Value;
                timer = new Timer(new TimerCallback(Drawing), model, 0, interval);
            }

            Console.ReadLine();
        }

        public static Result<bool> ParserValue()
        {
            IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };

            var errors = new List<string>();

            if (!float.TryParse(ConfigurationManager.AppSettings["WoodFireRisk"], NumberStyles.Float, formatter, out f)) { errors.Add("Поле WoodFireRisk не удалось преобразовать в float"); }
            if (!float.TryParse(ConfigurationManager.AppSettings["FillingWithWood"],NumberStyles.Float,formatter,out p)) { errors.Add("Поле FillingWithWood  не удалось преобразовать в float"); }
            if (!int.TryParse(ConfigurationManager.AppSettings["HeightMap"],out height)) { errors.Add("Поле HeightMap не удалось преобразовать в int"); }
            if (!int.TryParse(ConfigurationManager.AppSettings["WidthMap"], out width)) { errors.Add("Поле WidthMap не удалось преобразовать в int"); }
            if (!int.TryParse(ConfigurationManager.AppSettings["Interval"], out interval)) { errors.Add("Поле Interval не удалось преобразовать в int"); }

            if (errors.Any())
            {
                return Result<bool>.Fail(errors);
            }

            return Result<bool>.Success(true);
        }

        public static void Drawing(Object model)
        {
            ForestModel forest = (ForestModel)model;
            Console.Clear();

            for (int y = 0; y < forest.HeightMap; y++)
            {
                Console.WriteLine("");
                for (int x = 0; x < forest.WidthMap; x++)
                {
                    switch (forest.GetState()[y, x])
                    {
                        //Пустое поле
                        case Landscape.Empty:
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.Write($"   ");
                            break;

                        //Поле с деревом
                        case Landscape.Tree:
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write($"•  ");
                            break;

                        //Поле с горящим деревом
                        case Landscape.Fire:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write($"+  ");
                            break;
                    }
                }
            }

            forest.Step();

        }
    }
}
