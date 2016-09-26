using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qubiz.QuizEngine.Infrastructure;
using Qubiz.QuizEngine.Database.Repositories.Question.Contract;
using System.Data.Entity;

namespace Qubiz.QuizEngine.Database.Repositories.Question
{
	public class QuestionRepository : BaseRepository<Entities.QuestionDefinition>, IQuestionRepository
	{
		public QuestionRepository(QuizEngineDataContext context, UnitOfWork unitOfWork)
			: base(context, unitOfWork)
		{ }

		public async Task<IEnumerable<QuestionDefinition>> GetQuestionsAsync()
		{
			return await dbSet.Select(q => new QuestionDefinition
			{
				ID = q.ID,
				Complexity = q.Complexity,
				Number = q.Number,
				QuestionText = q.QuestionText,
				SectionID = q.SectionID,
				Type = q.Type
			}).ToListAsync();
		}

		public async Task<QuestionDefinition> GetQuestionByIDAsync(Guid id)
		{
			return await dbSet.Where(q => q.ID == id).Select(q => new QuestionDefinition
			{
				ID = q.ID,
				Complexity = q.Complexity,
				Number = q.Number,
				QuestionText = q.QuestionText,
				SectionID = q.SectionID,
				Type = q.Type
			}).FirstOrDefaultAsync();
		}

		public void UpdateQuestionAsync(QuestionDefinition question)
		{
			Upsert(question.DeepCopyTo<Entities.QuestionDefinition>());
		}

		public void AddQuestionAsync(QuestionDefinition question)
		{
			Create(question.DeepCopyTo<Entities.QuestionDefinition>());
		}

		public async Task DeleteQuestionAsync(Guid id)
		{
			Entities.QuestionDefinition question = await dbSet.Where(i => i.ID == id).FirstOrDefaultAsync();
			Delete(question);
		}
	}
}