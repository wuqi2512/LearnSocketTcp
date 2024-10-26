namespace ProtocalTool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string configPath = "E:\\Github\\LearnSocketTcp\\ProtocalTool\\Config.xml";
            string outputPath = "E:\\Github\\LearnSocketTcp\\Common\\Protocal\\";
            GeneratorCSharp generatorCSharp = new GeneratorCSharp();
            generatorCSharp.Generate(configPath, outputPath);
        }
    }
}
