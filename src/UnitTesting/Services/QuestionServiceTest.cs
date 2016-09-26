using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using Qubiz.QuizEngine.Infrastructure;
using Qubiz.QuizEngine.Database;
using Qubiz.QuizEngine.Database.Repositories;
using Qubiz.QuizEngine.Services.Question;
using Qubiz.QuizEngine.Services.Option.Contract;
using Qubiz.QuizEngine.Services.Question.Contract;
using System.Collections.Generic;
using Qubiz.QuizEngine.Database.Repositories.Section.Contract;
using Qubiz.QuizEngine.Services.Common.Contract;
using System.Linq;

namespace Qubiz.QuizEngine.UnitTesting.Services
{
	[TestClass]
	public class QuestionServiceTest
	{
		Mock<IUnitOfWorkFactory> unitOfWorkFactoryMock;
		Mock<IUnitOfWork> unitOfWorkMock;
		Mock<IQuestionRepository> questionRepositoryMock;
		Mock<IOptionRepository> optionRepositoryMock;
		Mock<ISectionRepository> sectionRepositoryMock;
		QuestionService questionService;

		[TestInitialize]
		public void TestInitialize()
		{
			unitOfWorkFactoryMock = new Mock<IUnitOfWorkFactory>(MockBehavior.Strict);
			unitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
			questionRepositoryMock = new Mock<IQuestionRepository>(MockBehavior.Strict);
			optionRepositoryMock = new Mock<IOptionRepository>(MockBehavior.Strict);
			sectionRepositoryMock = new Mock<ISectionRepository>(MockBehavior.Strict);
			questionService = new QuestionService(unitOfWorkFactoryMock.Object);


			unitOfWorkFactoryMock.Setup(x => x.Create()).Returns(unitOfWorkMock.Object);

			unitOfWorkMock.Setup(x => x.Dispose());
		}

		[TestCleanup]
		public void TestCleanup()
		{
			unitOfWorkFactoryMock.VerifyAll();
			unitOfWorkMock.VerifyAll();
			questionRepositoryMock.VerifyAll();
			optionRepositoryMock.VerifyAll();
			sectionRepositoryMock.VerifyAll();
		}

		[TestMethod]
		public async Task DeleteQuestionAsync_WhenDeletingExistingQuestion_ThenDeleteQuestion()
		{
			Guid questionID = Guid.NewGuid();

			IEnumerable<QuizEngine.Database.Models.OptionDefinition> options = new QuizEngine.Database.Models.OptionDefinition[]
			{
				new QuizEngine.Database.Models.OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 1", IsCorrectAnswer = true, Order = 1, QuestionID = questionID },
				new QuizEngine.Database.Models.OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 2", IsCorrectAnswer = false, Order = 1, QuestionID = questionID },
			};

			unitOfWorkMock.Setup(x => x.QuestionRepository).Returns(questionRepositoryMock.Object);
			questionRepositoryMock.Setup(x => x.DeleteQuestionAsync(questionID)).Returns(Task.CompletedTask);

			unitOfWorkMock.Setup(x => x.OptionRepository).Returns(optionRepositoryMock.Object);
			optionRepositoryMock.Setup(x => x.GetOptionsByQuestionIDAsync(questionID)).Returns(Task.FromResult(options));
			optionRepositoryMock.Setup(x => x.DeleteOptionsAsync(It.Is<QuizEngine.Database.Models.OptionDefinition[]>(s => (HaveEqualState(s, options.DeepCopyTo<OptionDefinition[]>())))));

			unitOfWorkMock.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask);

			await questionService.DeleteQuestionAsync(questionID);
		}

		[TestMethod]
		public async Task DeleteQuestionAsync_WhenDeletetingQuestionThatDoesNotExist_ThenQuestionIsDeleted()
		{
			Guid questionID = Guid.NewGuid();

			IEnumerable<OptionDefinition> options = new OptionDefinition[0];

			unitOfWorkMock.Setup(x => x.QuestionRepository).Returns(questionRepositoryMock.Object);
			questionRepositoryMock.Setup(x => x.DeleteQuestionAsync(questionID)).Returns(Task.CompletedTask);

			unitOfWorkMock.Setup(x => x.OptionRepository).Returns(optionRepositoryMock.Object);
			optionRepositoryMock.Setup(x => x.GetOptionsByQuestionIDAsync(questionID)).Returns(Task.FromResult(options.DeepCopyTo<IEnumerable<Qubiz.QuizEngine.Database.Models.OptionDefinition>>()));
			optionRepositoryMock.Setup(x => x.DeleteOptionsAsync(It.Is<Qubiz.QuizEngine.Database.Models.OptionDefinition[]>(s => (HaveEqualState(s, options.ToArray())))));

			unitOfWorkMock.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask);

			await questionService.DeleteQuestionAsync(questionID);

		}

		[TestMethod]
		public async Task GetQuestionByID_WhenGettingExistingQuestionByID_ThenReturnQuestion()
		{
			Guid questionID = Guid.NewGuid();

			IEnumerable<Qubiz.QuizEngine.Database.Models.OptionDefinition> options = new Qubiz.QuizEngine.Database.Models.OptionDefinition[]
			{
				new Qubiz.QuizEngine.Database.Models.OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 1", IsCorrectAnswer = true, Order = 1, QuestionID = questionID },
				new Qubiz.QuizEngine.Database.Models.OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 2", IsCorrectAnswer = false, Order = 1, QuestionID = questionID },
			};

			QuestionDetail question = new QuestionDetail { ID = questionID, Complexity = 0, Number = 1, QuestionText = "This is a test", SectionID = Guid.NewGuid(), Type = QuestionType.SingleSelect, Options = options.ToArray().DeepCopyTo<OptionDefinition[]>() };

			unitOfWorkMock.Setup(x => x.QuestionRepository).Returns(questionRepositoryMock.Object);
			questionRepositoryMock.Setup(x => x.GetQuestionByIDAsync(question.ID)).Returns(Task.FromResult(question.DeepCopyTo<Qubiz.QuizEngine.Database.Models.QuestionDefinition>()));

			unitOfWorkMock.Setup(x => x.OptionRepository).Returns(optionRepositoryMock.Object);
			optionRepositoryMock.Setup(x => x.GetOptionsByQuestionIDAsync(question.ID)).Returns(Task.FromResult(options));

			QuestionDetail serviceQuestion = await questionService.GetQuestionByID(question.ID);

			AssertAreEqual(question, serviceQuestion);
			CollectionAreEqual(serviceQuestion.Options[0], serviceQuestion.Options[0]);
			CollectionAreEqual(serviceQuestion.Options[1], serviceQuestion.Options[1]);
		}

		[TestMethod]
		public async Task GetQuestionByID_WhenNoQuestionMatchTheSearchedID_ThenReturnNull()
		{
			Guid questionID = Guid.NewGuid();

			IEnumerable<Qubiz.QuizEngine.Database.Models.OptionDefinition> options = new Qubiz.QuizEngine.Database.Models.OptionDefinition[0];

			unitOfWorkMock.Setup(x => x.QuestionRepository).Returns(questionRepositoryMock.Object);
			questionRepositoryMock.Setup(x => x.GetQuestionByIDAsync(questionID)).Returns(Task.FromResult((Qubiz.QuizEngine.Database.Models.QuestionDefinition)null));

			unitOfWorkMock.Setup(x => x.OptionRepository).Returns(optionRepositoryMock.Object);
			optionRepositoryMock.Setup(x => x.GetOptionsByQuestionIDAsync(questionID)).Returns(Task.FromResult(options));

			QuestionDetail returnedQuestion = await questionService.GetQuestionByID(questionID);

			Assert.AreEqual(null, returnedQuestion);
		}

		[TestMethod]
		public async Task UpdateQuestionAsync_WhenUpdatingExistingQuestion_ThenReturnNoValidationError()
		{
			Guid questionID = Guid.NewGuid();

			IEnumerable<Qubiz.QuizEngine.Database.Models.OptionDefinition> options = new Qubiz.QuizEngine.Database.Models.OptionDefinition[]
			{
				new Qubiz.QuizEngine.Database.Models.OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 1", IsCorrectAnswer = true, Order = 1, QuestionID = questionID },
				new Qubiz.QuizEngine.Database.Models.OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 2", IsCorrectAnswer = false, Order = 1, QuestionID = questionID },
				new Qubiz.QuizEngine.Database.Models.OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 3", IsCorrectAnswer = false, Order = 1, QuestionID = questionID },
			};

			QuestionDetail question = new QuestionDetail { ID = questionID, Complexity = 0, Number = 1, QuestionText = "Test", SectionID = Guid.NewGuid(), Type = QuestionType.SingleSelect, Options = options.DeepCopyTo<IEnumerable<OptionDefinition>>().ToArray() };

			unitOfWorkMock.Setup(x => x.QuestionRepository).Returns(questionRepositoryMock.Object);
			questionRepositoryMock.Setup(x => x.UpdateQuestionAsync(It.Is<Qubiz.QuizEngine.Database.Models.QuestionDefinition>(s => (HaveEqualState(s, question))))).Returns(Task.CompletedTask);

			unitOfWorkMock.Setup(x => x.OptionRepository).Returns(optionRepositoryMock.Object);
			optionRepositoryMock.Setup(x => x.GetOptionsByQuestionIDAsync(questionID)).Returns(Task.FromResult(options));
			optionRepositoryMock.Setup(x => x.DeleteOptionsAsync(It.Is<Qubiz.QuizEngine.Database.Models.OptionDefinition[]>(s => (HaveEqualState(s, options.DeepCopyTo<OptionDefinition[]>())))));
			optionRepositoryMock.Setup(x => x.AddOptionsAsync(It.Is<Qubiz.QuizEngine.Database.Models.OptionDefinition[]>(s => (HaveEqualState(s, question.Options)))));

			unitOfWorkMock.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask);

			ValidationError[] errors = await questionService.UpdateQuestionAsync(question);
			Assert.AreEqual(0, errors.Length);
		}

		[TestMethod]
		public async Task UpdateQuestionAsync_WhenQuestionTextIsEmpty_ThenReturnValidationError()
		{
			Guid questionID = Guid.NewGuid();

			OptionDefinition[] options = new OptionDefinition[]
			{
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 1", IsCorrectAnswer = true, Order = 1, QuestionID = questionID },
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 2", IsCorrectAnswer = false, Order = 1, QuestionID = questionID },
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 3", IsCorrectAnswer = false, Order = 1, QuestionID = questionID },
			};

			QuestionDetail question = new QuestionDetail { ID = Guid.NewGuid(), Complexity = 0, Number = 1, Options = options, QuestionText = String.Empty, SectionID = Guid.NewGuid(), Type = QuestionType.SingleSelect };

			ValidationError[] errors = await questionService.UpdateQuestionAsync(question);

			Assert.AreEqual(1, errors.Length);
			Assert.AreEqual(errors[0].Message, "Question text can't be empty !");
		}

		[TestMethod]
		public async Task UpdateQuestionAsync_WhenThereAreLessThanThreeOptionsProvided_ThenReturnValidationError()
		{
			Guid questionID = Guid.NewGuid();

			OptionDefinition[] options = new OptionDefinition[]
			{
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 1", IsCorrectAnswer = true, Order = 1, QuestionID = questionID },
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 2", IsCorrectAnswer = false, Order = 1, QuestionID = questionID },
			};

			QuestionDetail question = new QuestionDetail { ID = Guid.NewGuid(), Complexity = 0, Number = 1, Options = options, QuestionText = "This is a test", SectionID = Guid.NewGuid(), Type = QuestionType.SingleSelect };

			ValidationError[] errors = await questionService.UpdateQuestionAsync(question);

			Assert.AreEqual(1, errors.Length);
			Assert.AreEqual(errors[0].Message, "Question must have at last 3 options !");
		}

		[TestMethod]
		public async Task UpdateQuestionAsync_WhenThereIsNoSectionAsigned_ThenReturnValidationError()
		{
			Guid questionID = Guid.NewGuid();

			OptionDefinition[] options = new OptionDefinition[]
			{
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 1", IsCorrectAnswer = true, Order = 1, QuestionID = questionID },
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 2", IsCorrectAnswer = false, Order = 1, QuestionID = questionID },
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 3", IsCorrectAnswer = false, Order = 1, QuestionID = questionID },
			};

			QuestionDetail question = new QuestionDetail { ID = Guid.NewGuid(), Complexity = 0, Number = 1, Options = options, QuestionText = "This is a test", SectionID = Guid.Empty, Type = QuestionType.SingleSelect };

			ValidationError[] errors = await questionService.UpdateQuestionAsync(question);

			Assert.AreEqual(1, errors.Length);
			Assert.AreEqual(errors[0].Message, "Question must have a section assigned !");
		}

		[TestMethod]
		public async Task UpdateQuestionAsync_WhenThereAreOptionsWhichHaveEmptyAnswers_ThenReturnValidationError()
		{
			Guid questionID = Guid.NewGuid();

			OptionDefinition[] options = new OptionDefinition[]
			{
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 1", IsCorrectAnswer = true, Order = 1, QuestionID = questionID },
				new OptionDefinition {ID = Guid.NewGuid(), Answer = String.Empty, IsCorrectAnswer = false, Order = 1, QuestionID = questionID },
				new OptionDefinition {ID = Guid.NewGuid(), Answer = String.Empty, IsCorrectAnswer = false, Order = 1, QuestionID = questionID },
			};

			QuestionDetail question = new QuestionDetail { ID = Guid.NewGuid(), Complexity = 0, Number = 1, Options = options, QuestionText = "This is a test", SectionID = Guid.NewGuid(), Type = QuestionType.SingleSelect };

			ValidationError[] errors = await questionService.UpdateQuestionAsync(question);

			Assert.AreEqual(1, errors.Length);
			Assert.AreEqual(errors[0].Message, "Option answers can't be empty !");
		}

		[TestMethod]
		public async Task UpdateQuestionAsync_WhenThereIsNoCorrectAnswer_ThenReturnValidationError()
		{
			Guid questionID = Guid.NewGuid();

			OptionDefinition[] options = new OptionDefinition[]
			{
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 1", IsCorrectAnswer = false, Order = 1, QuestionID = questionID },
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 2", IsCorrectAnswer = false, Order = 1, QuestionID = questionID },
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 3", IsCorrectAnswer = false, Order = 1, QuestionID = questionID },
			};

			QuestionDetail question = new QuestionDetail { ID = Guid.NewGuid(), Complexity = 0, Number = 1, Options = options, QuestionText = "This is a test", SectionID = Guid.NewGuid(), Type = QuestionType.SingleSelect };

			ValidationError[] errors = await questionService.UpdateQuestionAsync(question);

			Assert.AreEqual(1, errors.Length);
			Assert.AreEqual(errors[0].Message, "Options must have at last 1 correct answer !");
		}

		[TestMethod]
		public async Task AddQuestionAsync_WhenAddingAddingQuestion_ThenReturnNoValdiationError()
		{
			Guid questionID = Guid.NewGuid();

			OptionDefinition[] options = new OptionDefinition[]
			{
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test Answer", IsCorrectAnswer = true, Order = 1, QuestionID = questionID },
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test Answer", IsCorrectAnswer = true, Order = 2, QuestionID = questionID },
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test Answer", IsCorrectAnswer = false, Order = 3, QuestionID = questionID },

			};
			QuestionDetail question = new QuestionDetail { ID = questionID, Complexity = 0, Number = 1, QuestionText = "This is a test", SectionID = Guid.NewGuid(), Type = QuestionType.SingleSelect, Options = options };

			unitOfWorkMock.Setup(x => x.QuestionRepository).Returns(questionRepositoryMock.Object);
			questionRepositoryMock.Setup(x => x.AddQuestionAsync(It.Is<Qubiz.QuizEngine.Database.Models.QuestionDefinition>(s => (HaveEqualState(s, question))))).Returns(Task.CompletedTask);

			unitOfWorkMock.Setup(x => x.OptionRepository).Returns(optionRepositoryMock.Object);
			optionRepositoryMock.Setup(x => x.AddOptionsAsync(It.Is<Qubiz.QuizEngine.Database.Models.OptionDefinition[]>(s => (HaveEqualState(s, question.Options)))));

			unitOfWorkMock.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask);

			ValidationError[] errors = await questionService.AddQuestionAsync(question);

		}

		[TestMethod]
		public async Task AddQuestionAsync_WhenAddingAddingQuestionWithEmptyQuestionText_ThenReturnNoValdiationError()
		{
			Guid questionID = Guid.NewGuid();

			OptionDefinition[] options = new OptionDefinition[]
			{
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 1", IsCorrectAnswer = true, Order = 1, QuestionID = questionID },
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 2", IsCorrectAnswer = false, Order = 1, QuestionID = questionID },
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 3", IsCorrectAnswer = false, Order = 1, QuestionID = questionID },
			};

			QuestionDetail question = new QuestionDetail { ID = Guid.NewGuid(), Complexity = 0, Number = 1, Options = options, QuestionText = String.Empty, SectionID = Guid.NewGuid(), Type = QuestionType.SingleSelect };

			ValidationError[] errors = await questionService.AddQuestionAsync(question);

			Assert.AreEqual(1, errors.Length);
			Assert.AreEqual(errors[0].Message, "Question text can't be empty !");
		}

		[TestMethod]
		public async Task AddQuestionAsync_WhenAddingAddingQuestionWithLessThanThreeOptions_ThenReturnNoValdiationError()
		{
			Guid questionID = Guid.NewGuid();

			OptionDefinition[] options = new OptionDefinition[]
			{
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 1", IsCorrectAnswer = true, Order = 1, QuestionID = questionID },
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 2", IsCorrectAnswer = false, Order = 1, QuestionID = questionID },
			};

			QuestionDetail question = new QuestionDetail { ID = Guid.NewGuid(), Complexity = 0, Number = 1, Options = options, QuestionText = "This is a test", SectionID = Guid.NewGuid(), Type = QuestionType.SingleSelect };

			ValidationError[] errors = await questionService.AddQuestionAsync(question);

			Assert.AreEqual(1, errors.Length);
			Assert.AreEqual(errors[0].Message, "Question must have at last 3 options !");
		}

		[TestMethod]
		public async Task AddQuestionAsync_WhenAddingAddingQuestionWithoutASectionAssigned_ThenReturnNoValdiationError()
		{
			Guid questionID = Guid.NewGuid();

			OptionDefinition[] options = new OptionDefinition[]
			{
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 1", IsCorrectAnswer = true, Order = 1, QuestionID = questionID },
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 2", IsCorrectAnswer = false, Order = 1, QuestionID = questionID },
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 3", IsCorrectAnswer = false, Order = 1, QuestionID = questionID },
			};

			QuestionDetail question = new QuestionDetail { ID = Guid.NewGuid(), Complexity = 0, Number = 1, Options = options, QuestionText = "This is a test", SectionID = Guid.Empty, Type = QuestionType.SingleSelect };

			ValidationError[] errors = await questionService.AddQuestionAsync(question);

			Assert.AreEqual(1, errors.Length);
			Assert.AreEqual(errors[0].Message, "Question must have a section assigned !");
		}

		[TestMethod]
		public async Task AddQuestionAsync_WhenAddingAddingQuestionWithEmptyOptionAnswers_ThenReturnNoValdiationError()
		{
			Guid questionID = Guid.NewGuid();

			OptionDefinition[] options = new OptionDefinition[]
			{
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 1", IsCorrectAnswer = true, Order = 1, QuestionID = questionID },
				new OptionDefinition {ID = Guid.NewGuid(), Answer = String.Empty, IsCorrectAnswer = false, Order = 1, QuestionID = questionID },
				new OptionDefinition {ID = Guid.NewGuid(), Answer = String.Empty, IsCorrectAnswer = false, Order = 1, QuestionID = questionID },
			};

			QuestionDetail question = new QuestionDetail { ID = Guid.NewGuid(), Complexity = 0, Number = 1, Options = options, QuestionText = "This is a test", SectionID = Guid.NewGuid(), Type = QuestionType.SingleSelect };

			ValidationError[] errors = await questionService.AddQuestionAsync(question);

			Assert.AreEqual(1, errors.Length);
			Assert.AreEqual(errors[0].Message, "Option answers can't be empty !");
		}

		[TestMethod]
		public async Task AddQuestionAsync_WhenAddingAddingQuestionWithoutCorrectAnswer_ThenReturnNoValdiationError()
		{
			Guid questionID = Guid.NewGuid();

			OptionDefinition[] options = new OptionDefinition[]
			{
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 1", IsCorrectAnswer = false, Order = 1, QuestionID = questionID },
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 2", IsCorrectAnswer = false, Order = 1, QuestionID = questionID },
				new OptionDefinition {ID = Guid.NewGuid(), Answer = "Test 3", IsCorrectAnswer = false, Order = 1, QuestionID = questionID },
			};

			QuestionDetail question = new QuestionDetail { ID = Guid.NewGuid(), Complexity = 0, Number = 1, Options = options, QuestionText = "This is a test", SectionID = Guid.NewGuid(), Type = QuestionType.SingleSelect };

			ValidationError[] errors = await questionService.AddQuestionAsync(question);

			Assert.AreEqual(1, errors.Length);
			Assert.AreEqual(errors[0].Message, "Options must have at last 1 correct answer !");
		}

		[TestMethod]
		public async Task GetQuestionsByPageAsync_WhenGettingPagedQuestions_ThenReturnQuestionsPaged()
		{
			Section[] sections = new Section[]{
				new Section { ID = Guid.NewGuid(), Name = "This is a test 1" },
				new Section { ID = Guid.NewGuid(), Name = "This is a test 2" }

			};

			IEnumerable<Qubiz.QuizEngine.Database.Models.QuestionDefinition> questions = new Qubiz.QuizEngine.Database.Models.QuestionDefinition[]
			{
				new Qubiz.QuizEngine.Database.Models.QuestionDefinition { ID = Guid.NewGuid(), Complexity = 0, Number = 1,  QuestionText = "This is a test", SectionID = sections[0].ID, Type = Qubiz.QuizEngine.Database.Entities.QuestionType.SingleSelect },
				new Qubiz.QuizEngine.Database.Models.QuestionDefinition { ID = Guid.NewGuid(), Complexity = 0, Number = 2,  QuestionText = "This is a test 2", SectionID = sections[1].ID, Type = Qubiz.QuizEngine.Database.Entities.QuestionType.SingleSelect }
			};

			unitOfWorkMock.Setup(x => x.QuestionRepository).Returns(questionRepositoryMock.Object);
			questionRepositoryMock.Setup(x => x.GetQuestionsAsync()).Returns(Task.FromResult(questions));

			unitOfWorkMock.Setup(x => x.SectionRepository).Returns(sectionRepositoryMock.Object);
			sectionRepositoryMock.Setup(x => x.ListAsync()).Returns(Task.FromResult(sections));

			PagedResult<QuestionListItem> pagedQuestions = await questionService.GetQuestionsByPageAsync(0, 2);

			Assert.AreEqual(questions.Count(), pagedQuestions.Items.Length);
			Assert.AreEqual(questions.Count(), pagedQuestions.TotalCount);
		}

		[TestMethod]
		public async Task GetQuestionsByPageAsync_WhenGettingEmptyPagedQuestions_ThenReturnEmptyPagedQuestions()
		{
			Section[] sections = new Section[]{
			};

			IEnumerable<Qubiz.QuizEngine.Database.Models.QuestionDefinition> questions = new Qubiz.QuizEngine.Database.Models.QuestionDefinition[]
			{
			};

			unitOfWorkMock.Setup(x => x.QuestionRepository).Returns(questionRepositoryMock.Object);
			questionRepositoryMock.Setup(x => x.GetQuestionsAsync()).Returns(Task.FromResult(questions));

			unitOfWorkMock.Setup(x => x.SectionRepository).Returns(sectionRepositoryMock.Object);
			sectionRepositoryMock.Setup(x => x.ListAsync()).Returns(Task.FromResult(sections));

			PagedResult<QuestionListItem> pagedQuestions = await questionService.GetQuestionsByPageAsync(0, 2);

			Assert.AreEqual(0, pagedQuestions.Items.Length);
			Assert.AreEqual(0, pagedQuestions.TotalCount);
		}

		private void AssertAreEqual(QuestionDetail expected, QuestionDetail actual)
		{
			Assert.AreEqual(expected.ID, actual.ID);
			Assert.AreEqual(expected.Number, actual.Number);
			Assert.AreEqual(expected.QuestionText, actual.QuestionText);
			Assert.AreEqual(expected.SectionID, actual.SectionID);
			Assert.AreEqual(expected.Type.ToString(), actual.Type.ToString());
			Assert.AreEqual(expected.Complexity, actual.Complexity);
		}
		private void CollectionAreEqual(OptionDefinition expected, OptionDefinition actual)
		{
			Assert.AreEqual(expected.ID, actual.ID);
			Assert.AreEqual(expected.IsCorrectAnswer, actual.IsCorrectAnswer);
			Assert.AreEqual(expected.Order, actual.Order);
			Assert.AreEqual(expected.QuestionID, actual.QuestionID);
			Assert.AreEqual(expected.Answer, actual.Answer);
		}

		private void CollectionAreEqual(QuestionDetail expected, QuestionListItem actual, Section[] sections)
		{
			Assert.AreEqual(expected.ID, actual.ID);
			Assert.AreEqual(expected.Number, actual.Number);
			Assert.AreEqual(expected.SectionID, sections.ToArray().First(x => x.Name.Equals(actual.Section)).ID);
		}

		private bool CompareTwoOptions(Qubiz.QuizEngine.Database.Models.OptionDefinition expected, OptionDefinition actual)
		{
			return expected.ID == actual.ID
				&& expected.IsCorrectAnswer == actual.IsCorrectAnswer
				&& expected.Answer == actual.Answer
				&& expected.IsCorrectAnswer == actual.IsCorrectAnswer
				&& expected.Order == actual.Order
				&& expected.QuestionID == actual.QuestionID;
		}

		private bool HaveEqualState(Qubiz.QuizEngine.Database.Models.QuestionDefinition expected, QuestionDetail actual)
		{
			return expected.Complexity == actual.Complexity
				&& expected.ID == actual.ID
				&& expected.Number == actual.Number
				&& expected.QuestionText == actual.QuestionText
				&& expected.SectionID == actual.SectionID
				&& expected.Type.ToString() == actual.Type.ToString();
		}

		private bool HaveEqualState(Qubiz.QuizEngine.Database.Models.OptionDefinition[] expected, OptionDefinition[] actual)
		{
			bool areEqual = false;
			if (expected.Length == 0 && actual.Length == 0)
			{
				areEqual = true;
			}

			for (int i = 0; i < expected.Length; i++)
			{
				if (CompareTwoOptions(expected[i], actual[i]))
				{
					areEqual = true;
					continue;
				}
				areEqual = false;
			}
			return areEqual;
		}
	}
}