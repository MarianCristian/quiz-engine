﻿using Qubiz.QuizEngine.Database.Repositories.Option.Contract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Qubiz.QuizEngine.Database.Repositories.Option
{
	public class OptionRepository : BaseRepository<Entities.OptionDefinition>, IOptionRepository
	{
		public OptionRepository(QuizEngineDataContext context, UnitOfWork unitOfWork)
			: base(context, unitOfWork)
		{ }

		public void DeleteOptionsAsync(OptionDefinition[] options)
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
				Delete(option);
			}
		}

		public async Task<IEnumerable<OptionDefinition>> GetOptionsByQuestionIDAsync(Guid id)
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

		public void UpdateOptionsAsync(OptionDefinition[] options)
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
				Upsert(option);
			}
		}

		public void AddOptionsAsync(OptionDefinition[] options)
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
				Create(option);
			}
		}
	}
}