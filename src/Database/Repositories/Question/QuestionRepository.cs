using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qubiz.QuizEngine.Infrastructure;
using Qubiz.QuizEngine.Database.Repositories.Question.Contract;
using System.Data.Entity;

namespace Qubiz.QuizEngine.Database.Repositories.Question
{
	public class QuestionRepository : IQuestionRepository
	{
		private DbSet<Entities.QuestionDefinition> dbSet;
		public QuestionRepository(QuizEngineDataContext context)
		{
			this.dbSet = context.Set<Entities.QuestionDefinition>();
		}

		public async Task<IEnumerable<QuestionDefinition>> List()
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

		public async Task<QuestionDefinition> GetByIDAsync(Guid id)
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

		public void Upsert(QuestionDefinition question)
		{
			Entities.QuestionDefinition dbQuestion = dbSet.Find(question.ID);
			if (dbQuestion == null)
			{
				dbSet.Add(question.DeepCopyTo<Entities.QuestionDefinition>());
			}
			Mapper.Map(question, dbQuestion);
		}

		public void Delete(Guid id)
		{
			Entities.QuestionDefinition question = dbSet.Find(id);
			dbSet.Remove(question);
		}
	}
}