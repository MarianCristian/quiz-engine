using Qubiz.QuizEngine.Database.Repositories.Option.Contract;
using Qubiz.QuizEngine.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Qubiz.QuizEngine.Database.Repositories.Option
{
	public class OptionRepository : IOptionRepository
	{
		private DbSet<Entities.OptionDefinition> dbSet;
		public OptionRepository(QuizEngineDataContext context)
		{
			this.dbSet = context.Set<Entities.OptionDefinition>();
		}


		public void Delete(OptionDefinition[] options)
		{
			Entities.OptionDefinition[] entityOptions = options.Select(o => new Entities.OptionDefinition
			{
				Answer = o.Answer,
				ID = o.ID,
				IsCorrectAnswer = o.IsCorrectAnswer,
				Order = o.Order,
				QuestionID = o.QuestionID
			}).ToArray();
			foreach (var option in entityOptions)
			{
				dbSet.Attach(option);
				dbSet.Remove(option);
			}
		}

		public async Task<IEnumerable<OptionDefinition>> ListByQuestionIDAsync(Guid id)
		{
			return await dbSet.Where(o => o.QuestionID == id).OrderBy(o => o.Order).Select(o => new OptionDefinition
			{
				Answer = o.Answer,
				ID = o.ID,
				IsCorrectAnswer = o.IsCorrectAnswer,
				Order = o.Order,
				QuestionID = o.QuestionID
			}).ToListAsync();
		}

		public void Update(OptionDefinition[] options)
		{
			Entities.OptionDefinition[] entityOptions = options.Select(o => new Entities.OptionDefinition
			{
				Answer = o.Answer,
				ID = o.ID,
				IsCorrectAnswer = o.IsCorrectAnswer,
				Order = o.Order,
				QuestionID = o.QuestionID
			}).ToArray();
			foreach (var option in entityOptions)
			{
				Entities.OptionDefinition existingOption = dbSet.Find(option.ID);
				Mapper.Map(existingOption, option);
			}
		}

		public void Add(OptionDefinition[] options)
		{
			Entities.OptionDefinition[] entityOptions = options.Select(o => new Entities.OptionDefinition
			{
				Answer = o.Answer,
				ID = o.ID,
				IsCorrectAnswer = o.IsCorrectAnswer,
				Order = o.Order,
				QuestionID = o.QuestionID
			}).ToArray();
			foreach (var option in entityOptions)
			{
				dbSet.Add(option);
			}
		}
	}
}