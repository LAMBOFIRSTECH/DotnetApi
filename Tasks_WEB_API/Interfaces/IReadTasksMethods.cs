namespace Tasks_WEB_API.Interfaces
{
    public interface IReadTasksMethods
    {
        Task<List<Tache>> GetTaches();
        Task<Tache> GetTaskById(int? matricule);
    }
}