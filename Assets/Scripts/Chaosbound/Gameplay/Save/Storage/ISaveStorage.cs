namespace Chaosbound.Gameplay.Save
{
    public interface ISaveStorage
    {
        void Save(string data);

        bool TryLoad(
            out string data);

        bool Exists();

        void Delete();
    }
}