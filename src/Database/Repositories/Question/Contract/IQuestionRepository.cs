using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qubiz.QuizEngine.Database.Repositories.Question.Contract
{
	public interface IQuestionRepository
	{
		void Upsert(QuestionDefinition question);

		void Delete(Guid id);

		Task<IEnumerable<QuestionDefinition>> List();

		Task<QuestionDefinition> GetByIDAsync(Guid id);
	}
}