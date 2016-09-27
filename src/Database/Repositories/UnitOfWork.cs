using Qubiz.QuizEngine.Database.Repositories.Option;
using Qubiz.QuizEngine.Database.Repositories.Option.Contract;
using Qubiz.QuizEngine.Database.Repositories.Question;
using Qubiz.QuizEngine.Database.Repositories.Question.Contract;
using Qubiz.QuizEngine.Database.Repositories.Section;
using Qubiz.QuizEngine.Database.Repositories.Section.Contract;
using Qubiz.QuizEngine.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Qubiz.QuizEngine.Database.Repositories
{
	public class UnitOfWork : IUnitOfWork, IDisposable
	{
		private readonly QuizEngineDataContext dbContext;

		private IFeatureFlagRepository featureFlagRepository;
		private IQuestionRepository questionRepository;
		private IOptionRepository optionRepository;
		private ISectionRepository sectionRepository;
		private IAdminRepository adminsRepository;

		public UnitOfWork(IConfig config)
		{
			dbContext = new QuizEngineDataContext(config.ConnectionString);
		}

		public IFeatureFlagRepository FeatureFlagRepository
		{
			get
			{
				if (featureFlagRepository == null)
				{
					featureFlagRepository = new FeatureFlagRepository(dbContext, this);
				}

				return featureFlagRepository;
			}
		}

		public IQuestionRepository QuestionRepository
		{
			get
			{
				if (questionRepository == null)
				{
					questionRepository = new QuestionRepository(dbContext);
				}

				return questionRepository;
			}
		}

		public IOptionRepository OptionRepository
		{
			get
			{
				if (optionRepository == null)
				{
					optionRepository = new OptionRepository(dbContext);
				}

				return optionRepository;
			}
		}

		public ISectionRepository SectionRepository
		{
			get
			{
				if (sectionRepository == null)
				{
					sectionRepository = new SectionRepository(dbContext);
				}

				return sectionRepository;
			}
		}

		public IAdminRepository AdminRepository
		{
			get
			{
				if (adminsRepository == null)
				{
					adminsRepository = new AdminRepository(dbContext, this);
				}

				return adminsRepository;
			}
		}

		public async Task SaveAsync()
		{
			await dbContext.SaveChangesAsync();
		}

		public void Dispose()
		{
			if (dbContext != null)
			{
				dbContext.Dispose();
			}
		}
	}
}
