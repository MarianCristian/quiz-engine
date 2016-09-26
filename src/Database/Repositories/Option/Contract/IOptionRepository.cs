using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qubiz.QuizEngine.Database.Repositories.Option.Contract
{
	public interface IOptionRepository
    {
        void AddOptionsAsync(OptionDefinition[] options);
        void UpdateOptionsAsync(OptionDefinition[] options);
        void DeleteOptionsAsync(OptionDefinition[] options);
        Task<IEnumerable<OptionDefinition>> GetOptionsByQuestionIDAsync(Guid id);
	}
}