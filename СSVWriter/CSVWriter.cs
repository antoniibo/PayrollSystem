namespace СSVWriter
{
    public class CSVWriter
    {
        public static void WriteFile(string filename, string data)
        {
            File.WriteAllText(filename, data);
        }
    }
}