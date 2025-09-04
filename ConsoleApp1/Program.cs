namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //== Message ==
            string name = "Bryan";
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            Console.WriteLine("Hello! this program was written by " + name + " on " +date+".\n" );

            //== Base Legend ==
            Console.WriteLine("Legend");
            Console.WriteLine("Decimal = base 10 (normal numbers).");
            Console.WriteLine("Binary = base 2 number system (0s and 1s), in 8-bit groupings.");
            Console.WriteLine("Hex = base 16 number system (0-9 followed by A-F), composed of 2 digits.");

            int[] numbers = { 0, 1, 2, 4, 8, 16, 32, 64, 100, 255 };

            foreach (int num in numbers)
            {
                NumberFormatsConverter(num);
            }

            Console.WriteLine(new String('-', 30));
            Console.WriteLine("Conversion completed! Goodbye!");
        }

        ///<summary>
        ///Prints given numbers in the format of Decimal, Binary and Hex formats.
        ///</summary>
        static void NumberFormatsConverter(int num)
        {
            string binary = Convert.ToString(num, 2).PadLeft(8, '0');
            string hex = num.ToString("X2");
            Console.WriteLine($"Decimal: {num,3} > Binary: {binary} > Hex: {hex}");
        }
    }
}
