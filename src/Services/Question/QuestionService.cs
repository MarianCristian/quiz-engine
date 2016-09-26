﻿using Qubiz.QuizEngine.Database;
using Qubiz.QuizEngine.Database.Repositories;
using Qubiz.QuizEngine.Infrastructure;
using Qubiz.QuizEngine.Services.Common.Contract;
using Qubiz.QuizEngine.Services.Option.Contract;
using Qubiz.QuizEngine.Services.Question.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Qubiz.QuizEngine.Services.Question
{
	public class QuestionService : IQuestionService
	{
		private readonly IUnitOfWorkFactory unitOfWorkFactory;

		public QuestionService(IUnitOfWorkFactory unitOfWorkFactory)
		{
			this.unitOfWorkFactory = unitOfWorkFactory;
		}

		public async Task DeleteQuestionAsync(Guid id)
		{
			using (IUnitOfWork unitOfWork = unitOfWorkFactory.Create())
			{
				await unitOfWork.QuestionRepository.DeleteQuestionAsync(id);
				IEnumerable<Qubiz.QuizEngine.Database.Models.OptionDefinition> options = await unitOfWork.OptionRepository.GetOptionsByQuestionIDAsync(id);
				unitOfWork.OptionRepository.DeleteOptionsAsync(options.ToArray());
				await unitOfWork.SaveAsync();
			}
		}

		public async Task<QuestionDetail> GetQuestionByID(Guid id)
		{
			using (IUnitOfWork unitOfWork = unitOfWorkFactory.Create())
			{
				QuestionDetail question = (await unitOfWork.QuestionRepository.GetQuestionByIDAsync(id)).DeepCopyTo<QuestionDetail>();

				IEnumerable<OptionDefinition> options = (await unitOfWork.OptionRepository.GetOptionsByQuestionIDAsync(id)).DeepCopyTo<OptionDefinition[]>();

				if (question != null)
				{
					question.Options = options.Select(o => new OptionDefinition
					{
						Answer = o.Answer,
						ID = o.ID,
						IsCorrectAnswer = o.IsCorrectAnswer,
						Order = o.Order,
						QuestionID = o.QuestionID
					}).ToArray();
					return question;
				}
				return null;
			}
		}

		public async Task<ValidationError[]> UpdateQuestionAsync(QuestionDetail question)
		{
			using (IUnitOfWork unitOfWork = unitOfWorkFactory.Create())
			{
				if (question.QuestionText.Equals(String.Empty))
					return new ValidationError[1] { new ValidationError { Message = "Question text can't be empty !" } };

				if (question.Options.Length < 3)
					return new ValidationError[1] { new ValidationError { Message = "Question must have at last 3 options !" } };

				if (question.SectionID == Guid.Empty)
					return new ValidationError[1] { new ValidationError { Message = "Question must have a section assigned !" } };

				foreach (OptionDefinition option in question.Options)
				{
					if (option.Answer.Equals(String.Empty))
					{
						return new ValidationError[1] { new ValidationError { Message = "Option answers can't be empty !" } };
					}
				}

				if (!question.Options.Any(x => x.IsCorrectAnswer == true))
					return new ValidationError[1] { new ValidationError { Message = "Options must have at last 1 correct answer !" } };

				await unitOfWork.QuestionRepository.UpdateQuestionAsync(question.DeepCopyTo<Qubiz.QuizEngine.Database.Models.QuestionDefinition>());

				unitOfWork.OptionRepository.DeleteOptionsAsync((await unitOfWork.OptionRepository.GetOptionsByQuestionIDAsync(question.ID)).ToArray());

				unitOfWork.OptionRepository.AddOptionsAsync(question.Options.Select(o => new Qubiz.QuizEngine.Database.Models.OptionDefinition
				{
					Answer = o.Answer,
					ID = o.ID,
					IsCorrectAnswer = o.IsCorrectAnswer,
					Order = o.Order,
					QuestionID = o.QuestionID
				}).ToArray());
				await unitOfWork.SaveAsync();

				return new ValidationError[0];
			}
		}

		public async Task<ValidationError[]> AddQuestionAsync(QuestionDetail question)
		{
			using (IUnitOfWork unitOfWork = unitOfWorkFactory.Create())
			{
				if (question.QuestionText.Equals(String.Empty))
					return new ValidationError[1] { new ValidationError { Message = "Question text can't be empty !" } };

				if (question.Options.Length < 3)
					return new ValidationError[1] { new ValidationError { Message = "Question must have at last 3 options !" } };

				if (question.SectionID == Guid.Empty)
					return new ValidationError[1] { new ValidationError { Message = "Question must have a section assigned !" } };

				foreach (OptionDefinition option in question.Options)
				{
					if (option.Answer.Equals(String.Empty))
						return new ValidationError[1] { new ValidationError { Message = "Option answers can't be empty !" } };

				}

				if (!question.Options.Any(x => x.IsCorrectAnswer == true))
					return new ValidationError[1] { new ValidationError { Message = "Options must have at last 1 correct answer !" } };

				await unitOfWork.QuestionRepository.AddQuestionAsync(question.DeepCopyTo<Qubiz.QuizEngine.Database.Models.QuestionDefinition>());
				unitOfWork.OptionRepository.AddOptionsAsync(question.Options.Select(o => new Qubiz.QuizEngine.Database.Models.OptionDefinition
				{
					Answer = o.Answer,
					ID = o.ID,
					IsCorrectAnswer = o.IsCorrectAnswer,
					Order = o.Order,
					QuestionID = o.QuestionID
				}).ToArray());
				await unitOfWork.SaveAsync();
				return new ValidationError[0];
			}
		}

		public async Task<PagedResult<QuestionListItem>> GetQuestionsByPageAsync(int pageNumber, int itemsPerPage)
		{
			using (IUnitOfWork unitOfWork = unitOfWorkFactory.Create())
			{
				IEnumerable<QuestionDetail> questions = (await unitOfWork.QuestionRepository.GetQuestionsAsync()).DeepCopyTo<QuestionDetail[]>().ToArray();

				if (pageNumber > questions.ToList().Count / itemsPerPage)
				{
					pageNumber = questions.ToList().Count / itemsPerPage;
				}

				if (pageNumber < 0)
				{
					pageNumber = 0;
				}

				var questionsFiltered = questions.Select(q => new { ID = q.ID, Number = q.Number, SectionID = q.SectionID }).ToArray();

				IEnumerable<Database.Repositories.Section.Contract.Section> sections = await unitOfWork.SectionRepository.ListAsync();

				return new PagedResult<QuestionListItem>
				{
					Items = questionsFiltered.OrderBy(q => q.Number).Skip(pageNumber * itemsPerPage).Take(itemsPerPage).Select(q => new QuestionListItem
					{
						ID = q.ID,
						Number = q.Number,
						Section = (sections.SingleOrDefault(s => s.ID == q.SectionID) ?? new Database.Repositories.Section.Contract.Section { Name = string.Empty }).Name
					}).ToArray(),
					TotalCount = questionsFiltered.Count()
				};
			}
		}
	}
}