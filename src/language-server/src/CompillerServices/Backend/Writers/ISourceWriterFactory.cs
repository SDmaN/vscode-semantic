namespace CompillerServices.Backend.Writers
{
    public interface ISourceWriterFactory
    {
        ISourceWriter Create(string inputModuleFileName, string outputPath);
    }
}