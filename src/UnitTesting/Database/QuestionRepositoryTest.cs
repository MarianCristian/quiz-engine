using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qubiz.QuizEngine.Database.Repositories.Question;
using Qubiz.QuizEngine.Database;
using Qubiz.QuizEngine.Infrastructure;
using System.Threading.Tasks;
using Qubiz.QuizEngine.Database.Repositories.Question.Contract;
using System.Collections.Generic;
using System.Linq;

namespace Qubiz.QuizEngine.UnitTesting.Database
{
	[TestClass]
	public class QuestionRepositoryTest
	{
		private QuestionRepository questionRepository;
		private QuizEngineDataContext dbContext;

		[TestInitialize]
		public void TestInitialize()
		{
			IConfig config = new Config();
			this.dbContext = new QuizEngineDataContext(config.ConnectionString);
			this.questionRepository = new QuestionRepository(dbContext);
		}

		[TestCleanup]
		public void TestCleanup()
		{
			dbContext.Database.ExecuteSqlCommand("DELETE FROM [dbo].[QuestionDefinitions]");
			dbContext.Database.ExecuteSqlCommand("DBCC CHECKIDENT('QuestionDefinitions', RESEED, 0)");
		}

		[TestMethod]
		public async Task List_WhenGettingAllQuestions_ThenReturnListOfQuestions()
		{
			QuestionDefinition[] questions = new QuestionDefinition[]
			{
				new QuestionDefinition {ID = Guid.NewGuid(), Complexity = 0 , QuestionText = "This is a test", SectionID = Guid.NewGuid(), Type = QuizEngine.Database.Entities.QuestionType.SingleSelect},
				new QuestionDefinition {ID = Guid.NewGuid(), Complexity = 0 , QuestionText = "This is a test 2", SectionID = Guid.NewGuid(), Type = QuizEngine.Database.Entities.QuestionType.MultiSelect}
			};

			questionRepository.Upsert(questions[0]);
			questionRepository.Upsert(questions[1]);
			dbContext.SaveChanges();

			IEnumerable<QuestionDefinition> dbQuestions = await questionRepository.List();

			AssertAreEqual(questions[0], dbQuestions.First(x => x.ID == questions[0].ID));
			AssertAreEqual(questions[1], dbQuestions.First(x => x.ID == questions[1].ID));
			Assert.AreEqual(questions.Length, dbQuestions.Count());
		}

		[TestMethod]
		public async Task List_WhenNoQuestionsMatchTheSearch_ThenReturnEmptyList()
		{
			IEnumerable<QuestionDefinition> dbQuestions = await questionRepository.List();

			Assert.AreEqual(0, dbQuestions.Count());
		}

		[TestMethod]
		public async Task GetByIDAsync_WhenGettingQuestionById_ThenReturnQuestion()
		{
			QuestionDefinition question = new QuestionDefinition { ID = Guid.NewGuid(), Complexity = 0, Number = 1, QuestionText = "This is a test", SectionID = Guid.NewGuid(), Type = QuizEngine.Database.Entities.QuestionType.SingleSelect };

			questionRepository.Upsert(question);
			dbContext.SaveChanges();

			QuestionDefinition dbQuestion = await questionRepository.GetByIDAsync(question.ID);

			AssertAreEqual(question, dbQuestion);
		}

		[TestMethod]
		public async Task GetByIDAsync_WhenNoQuestionMatchTheSearchedID_ThenReturnNull()
		{
			QuestionDefinition dbQuestion = await questionRepository.GetByIDAsync(Guid.NewGuid());

			Assert.IsNull(dbQuestion);
		}

		[TestMethod]
		public async Task Upsert_WhenAddingQuestion_ThenAddQuestion()
		{
			QuestionDefinition question = new QuestionDefinition { ID = Guid.NewGuid(), Complexity = 0, Number = 1, QuestionText = "This is a test", SectionID = Guid.NewGuid(), Type = QuizEngine.Database.Entities.QuestionType.SingleSelect };

			questionRepository.Upsert(question);
			dbContext.SaveChanges();

			QuestionDefinition dbQuestion = await questionRepository.GetByIDAsync(question.ID);

			AssertAreEqual(question, dbQuestion);
		}

		[TestMethod]
		public async Task Upsert_WhenUpdatingQuestion_ThenUpdateQuestion()
		{
			QuestionDefinition question = new QuestionDefinition { ID = Guid.NewGuid(), Complexity = 0, Number = 1, QuestionText = "This is a test", SectionID = Guid.NewGuid(), Type = QuizEngine.Database.Entities.QuestionType.SingleSelect };

			questionRepository.Upsert(question);
			dbContext.SaveChanges();

			question.QuestionText = "This is another test !";
			question.Type = QuizEngine.Database.Entities.QuestionType.MultiSelect;
			questionRepository.Upsert(question);
			dbContext.SaveChanges();

			QuestionDefinition dbQuestion = await questionRepository.GetByIDAsync(question.ID);

			Assert.AreEqual("This is another test !", dbQuestion.QuestionText);
			Assert.AreEqual(QuizEngine.Database.Entities.QuestionType.MultiSelect, dbQuestion.Type);
		}

		[TestMethod]
		public async Task Delete_WhenDeletingQuestion_ThenDeleteQuestion()
		{
			QuestionDefinition question = new QuestionDefinition { ID = Guid.NewGuid(), Complexity = 0, Number = 1, QuestionText = "This is a test", SectionID = Guid.NewGuid(), Type = QuizEngine.Database.Entities.QuestionType.SingleSelect };

			questionRepository.Upsert(question);
			dbContext.SaveChanges();

			questionRepository.Delete(question.ID);
			dbContext.SaveChanges();

			QuestionDefinition dbQuestion = await questionRepository.GetByIDAsync(question.ID);

			Assert.IsNull(dbQuestion);

		}

		private void AssertAreEqual(QuestionDefinition expected, QuestionDefinition actual)
		{
			Assert.AreEqual(expected.ID, actual.ID);
			Assert.AreEqual(expected.QuestionText, actual.QuestionText);
			Assert.AreEqual(expected.SectionID, actual.SectionID);
			Assert.AreEqual(expected.Type, actual.Type);
		}
	}
}
