using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qubiz.QuizEngine.Database.Repositories.Option;
using Qubiz.QuizEngine.Database;
using Qubiz.QuizEngine.Infrastructure;
using Qubiz.QuizEngine.Database.Repositories.Option.Contract;
using System.Linq;
using System.Threading.Tasks;

namespace Qubiz.QuizEngine.UnitTesting.Database
{
	[TestClass]
	public class OptionRepositoryTest
	{
		private OptionRepository optionRepository;
		private QuizEngineDataContext dbContext;

		[TestInitialize]
		public void TestInitialize()
		{
			IConfig config = new Config();
			this.dbContext = new QuizEngineDataContext(config.ConnectionString);
			this.optionRepository = new OptionRepository(dbContext);
		}

		[TestCleanup]
		public void TestCleanup()
		{
			dbContext.Database.ExecuteSqlCommand("DELETE FROM [dbo].[OptionDefinitions]");
		}

		[TestMethod]
		public async Task ListByQuestionIDAsync_WhenGettingOptionsByQuestionID_ThenReturnOptionsList()
		{
			Guid questionID = Guid.NewGuid();
			OptionDefinition[] options = new OptionDefinition[]
			{
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test", IsCorrectAnswer = true, Order = 1, QuestionID = questionID },
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 2", IsCorrectAnswer = false, Order = 2, QuestionID = questionID },
			};

			optionRepository.Upsert(options);
			dbContext.SaveChanges();

			OptionDefinition[] dbOptions = (await optionRepository.ListByQuestionIDAsync(questionID)).ToArray();

			AssertAreEqual(options[0], dbOptions[0]);
			AssertAreEqual(options[1], dbOptions[1]);
		}

		[TestMethod]
		public async Task ListByQuestionIDAsync_WhenGettingOptionsThatDoesNotMeetTheSearchCriteria_ThenReturnEmptyOptionsList()
		{
			Guid questionID = Guid.NewGuid();

			OptionDefinition[] dbOptions = (await optionRepository.ListByQuestionIDAsync(questionID)).ToArray();

			Assert.AreEqual(0, dbOptions.Length);
		}

		[TestMethod]
		public async Task Upsert_WhenUpdatingOptions_ThenUpdateOptions()
		{
			Guid questionID = Guid.NewGuid();
			OptionDefinition[] options = new OptionDefinition[]
			{
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test", IsCorrectAnswer = true, Order = 1, QuestionID = questionID },
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 2", IsCorrectAnswer = false, Order = 2, QuestionID = questionID },
			};

			optionRepository.Upsert(options);
			dbContext.SaveChanges();

			options[0].Answer = "This is a modified Text";
			options[1].IsCorrectAnswer = true;

			optionRepository.Upsert(options);
			dbContext.SaveChanges();

			OptionDefinition[] dbOptions = (await optionRepository.ListByQuestionIDAsync(questionID)).ToArray();

			Assert.AreEqual("This is a modified Text", dbOptions[0].Answer);
			Assert.AreEqual(true, dbOptions[1].IsCorrectAnswer);
		}

		[TestMethod]
		public async Task Upsert_WhenAddingOptions_ThenAddOptions()
		{
			Guid questionID = Guid.NewGuid();
			OptionDefinition[] options = new OptionDefinition[]
			{
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test", IsCorrectAnswer = true, Order = 1, QuestionID = questionID },
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 2", IsCorrectAnswer = false, Order = 2, QuestionID = questionID },
			};

			optionRepository.Upsert(options);
			dbContext.SaveChanges();

			OptionDefinition[] dbOptions = (await optionRepository.ListByQuestionIDAsync(questionID)).ToArray();

			AssertAreEqual(options[0], dbOptions[0]);
			AssertAreEqual(options[1], dbOptions[1]);
		}

		[TestMethod]
		public async Task Delete_WhenDeletingOptions_DeleteOptions()
		{
			Guid questionID = Guid.NewGuid();
			QuizEngine.Database.Entities.OptionDefinition[] options = new QuizEngine.Database.Entities.OptionDefinition[]
			{
				new QuizEngine.Database.Entities.OptionDefinition {ID = Guid.NewGuid(), Answer = "Test", IsCorrectAnswer = true, Order = 1, QuestionID = questionID },
				new QuizEngine.Database.Entities.OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 2", IsCorrectAnswer = false, Order = 2, QuestionID = questionID },
			};

			dbContext.Options.AddRange(options);
			dbContext.SaveChanges();

			optionRepository.Delete(options.DeepCopyTo<OptionDefinition[]>());
			dbContext.SaveChanges();

			OptionDefinition[] dbOptions = (await optionRepository.ListByQuestionIDAsync(questionID)).ToArray();
			Assert.AreEqual(0, dbOptions.Length);
		}

		private void AssertAreEqual(OptionDefinition expected, OptionDefinition actual)
		{
			Assert.AreEqual(expected.Answer, actual.Answer);
			Assert.AreEqual(expected.ID, actual.ID);
			Assert.AreEqual(expected.IsCorrectAnswer, actual.IsCorrectAnswer);
			Assert.AreEqual(expected.Order, actual.Order);
			Assert.AreEqual(expected.QuestionID, actual.QuestionID);
		}
	}
}
