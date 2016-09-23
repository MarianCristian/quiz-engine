using Qubiz.QuizEngine.Infrastructure;
using Qubiz.QuizEngine.Services.Common.Contract;
using System;
using System.Threading.Tasks;

namespace Qubiz.QuizEngine.Services.Question.Contract
{
	public interface IQuestionService
    {
        Task DeleteQuestionAsync(Guid id);
        Task<QuestionDetail> GetQuestionByID(Guid id);
        Task<ValidationError[]> AddQuestionAsync(QuestionDetail question);
        Task<ValidationError[]> UpdateQuestionAsync(QuestionDetail question);
        Task<PagedResult<QuestionListItem>> GetQuestionsByPageAsync(int pageNumber, int itemsPerPage);
    }
}