using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qubiz.QuizEngine.Database.Repositories.Question.Contract
{
	public interface IQuestionRepository
    {
        Task AddQuestionAsync(QuestionDefinition question);
        Task UpdateQuestionAsync(QuestionDefinition question);

        Task DeleteQuestionAsync(Guid id);

		Task<IEnumerable<QuestionDefinition>> GetQuestionsAsync();

        Task<QuestionDefinition> GetQuestionByIDAsync(Guid id);
	}
}