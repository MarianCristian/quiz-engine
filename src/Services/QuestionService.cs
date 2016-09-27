using Qubiz.QuizEngine.Database;
using Qubiz.QuizEngine.Database.Repositories;
using Qubiz.QuizEngine.Infrastructure;
using Qubiz.QuizEngine.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Qubiz.QuizEngine.Services
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
				unitOfWork.QuestionRepository.Delete(id);
				IEnumerable<Database.Repositories.Option.Contract.OptionDefinition> options = await unitOfWork.OptionRepository.ListByQuestionIDAsync(id);
				unitOfWork.OptionRepository.Delete(options.ToArray());
				await unitOfWork.SaveAsync();
			}
		}

		public async Task<QuestionDetail> GetQuestionByID(Guid id)
		{
			using (IUnitOfWork unitOfWork = unitOfWorkFactory.Create())
			{
				//List<Database.Models.QuestionDefinition> list = new List<Database.Models.QuestionDefinition>();
				QuestionDetail question = (await unitOfWork.QuestionRepository.GetByIDAsync(id)).DeepCopyTo<QuestionDetail>();
				IEnumerable<Database.Repositories.Option.Contract.OptionDefinition> options = await unitOfWork.OptionRepository.ListByQuestionIDAsync(id);
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
		}

		public async Task UpdateQuestionAsync(QuestionDetail question)
		{
			using (IUnitOfWork unitOfWork = unitOfWorkFactory.Create())
			{
				unitOfWork.QuestionRepository.Upsert(question.DeepCopyTo<Database.Repositories.Question.Contract.QuestionDefinition>());
				unitOfWork.OptionRepository.Delete((await unitOfWork.OptionRepository.ListByQuestionIDAsync(question.ID)).ToArray());
				unitOfWork.OptionRepository.Add(question.Options.Select(o => new Database.Repositories.Option.Contract.OptionDefinition
				{
					Answer = o.Answer,
					ID = o.ID,
					IsCorrectAnswer = o.IsCorrectAnswer,
					Order = o.Order,
					QuestionID = o.QuestionID
				}).ToArray());
				await unitOfWork.SaveAsync();
			}
		}

		public async Task AddQuestionAsync(QuestionDetail question)
		{
			using (IUnitOfWork unitOfWork = unitOfWorkFactory.Create())
			{
				unitOfWork.QuestionRepository.Upsert(question.DeepCopyTo<Database.Repositories.Question.Contract.QuestionDefinition>());
				unitOfWork.OptionRepository.Add(question.Options.Select(o => new Database.Repositories.Option.Contract.OptionDefinition
				{
					Answer = o.Answer,
					ID = o.ID,
					IsCorrectAnswer = o.IsCorrectAnswer,
					Order = o.Order,
					QuestionID = o.QuestionID
				}).ToArray());
				await unitOfWork.SaveAsync();
			}
		}

		public async Task<PagedResult<QuestionListItem>> GetQuestionsByPageAsync(int pageNumber, int itemsPerPage)
		{
			using (IUnitOfWork unitOfWork = unitOfWorkFactory.Create())
			{
				IEnumerable<Database.Repositories.Question.Contract.QuestionDefinition> questions = await unitOfWork.QuestionRepository.List();

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