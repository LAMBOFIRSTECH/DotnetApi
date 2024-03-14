namespace Tasks_WEB_API.Interfaces
{
    public interface IWriteTasksMethods
    {
        Task<Tache> CreateTask(Tache Tache);
		Task<Tache> UpdateTask(Tache Tache);
		Task DeleteTaskById(int id);
    }
}