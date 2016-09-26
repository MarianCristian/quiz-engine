using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qubiz.QuizEngine.Database.Repositories.Question.Contract
{
	public interface IQuestionRepository
    {
        void AddQuestionAsync(QuestionDefinition question);
        void UpdateQuestionAsync(QuestionDefinition question);

        Task DeleteQuestionAsync(Guid id);

		Task<IEnumerable<QuestionDefinition>> GetQuestionsAsync();

        Task<QuestionDefinition> GetQuestionByIDAsync(Guid id);
	}
}