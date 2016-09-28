using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qubiz.QuizEngine.Database.Repositories.Option.Contract
{
	public interface IOptionRepository
    {
        void Upsert(OptionDefinition[] options);
        void Delete(OptionDefinition[] options);
        Task<IEnumerable<OptionDefinition>> ListByQuestionIDAsync(Guid id);
	}
}