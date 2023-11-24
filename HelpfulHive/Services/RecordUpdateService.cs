

namespace HelpfulHive.Services
{
    public class RecordUpdateService
    {
        public event Func<Task> RecordsUpdated;

        public async Task OnRecordsUpdated()
        {
            if (RecordsUpdated != null)
            {
                await RecordsUpdated.Invoke();
            }
        }
    }
}
