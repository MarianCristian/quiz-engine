using System;
using System.Threading.Tasks;

namespace Qubiz.QuizEngine.Database.Repositories
{
    public interface ISectionRepository 
    {
        Task<Contract.Section[]> ListAsync();

        Task<Contract.Section> GetByNameAsync(string name);

        Task<Contract.Section> GetByIDAsync(Guid id);

        void Create(Contract.Section model);
        void Update(Contract.Section model);
        void Delete(Contract.Section model);
    }
}