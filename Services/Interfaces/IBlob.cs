namespace judo_backend.Services.Interfaces
{
    public interface IBlob
    {
        List<string> Get(string path);

        MemoryStream Get(string path, string filename);

        void Change(string path, string fileName1, string fileName2);

        void Upload(string path, string filename, MemoryStream ms);

        void Delete(string path, string filename);
    }
}
