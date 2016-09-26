using Qubiz.QuizEngine.Areas.M.Models;
using Qubiz.QuizEngine.Infrastructure;
using Qubiz.QuizEngine.Services.Question.Contract;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Qubiz.QuizEngine.Areas.M.Controllers.Api
{
	public class QuestionController : ApiController
	{
		private readonly IQuestionService questionService;

		public QuestionController(IQuestionService questionService)
		{
			this.questionService = questionService;
		}

		[HttpGet]
		public async Task<IHttpActionResult> Get(int pageNumber, int itemsPerPage)
		{
			Qubiz.QuizEngine.Services.Common.Contract.PagedResult<Qubiz.QuizEngine.Services.Question.Contract.QuestionListItem> questions = await questionService.GetQuestionsByPageAsync(pageNumber, itemsPerPage);
			if (questions.Items.Length == 0 && questions.TotalCount == 0)
			{
				HttpResponseMessage message = new HttpResponseMessage();
				message.StatusCode = System.Net.HttpStatusCode.NoContent;
				return ResponseMessage(message);
			}
			return Ok(questions.DeepCopyTo<QuestionPaged>());
		}

		[HttpGet]
		public async Task<IHttpActionResult> Get(Guid id)
		{
			Qubiz.QuizEngine.Services.Question.Contract.QuestionDetail question = await questionService.GetQuestionByID(id);
			if (question == null)
			{
				HttpResponseMessage response = new HttpResponseMessage();
				response.StatusCode = System.Net.HttpStatusCode.NoContent;
				return ResponseMessage(response);
			}
			return Ok(question.DeepCopyTo<Question>());
		}

		[HttpPut]
		public async Task<IHttpActionResult> Put(Question question)
		{
			ValidationError[] errors = await questionService.UpdateQuestionAsync(question.DeepCopyTo<Qubiz.QuizEngine.Services.Question.Contract.QuestionDetail>());
			string errorMessage = String.Empty;
			if (errors.Any())
			{
				if (errors.Length > 1)
				{
					foreach (ValidationError error in errors)
					{
						if(errors.Last().Message.Equals(error.Message))
						{
							errorMessage += error.Message;
							continue;
						}
						errorMessage += error.Message + ", ";
					}
					return BadRequest(errorMessage);
				}

				return BadRequest(errors[0].Message);
			}

			return Ok();
		}

		[HttpPost]
		public async Task<IHttpActionResult> Post(Question question)
		{
			ValidationError[] errors = await questionService.AddQuestionAsync(question.DeepCopyTo<Qubiz.QuizEngine.Services.Question.Contract.QuestionDetail>());
			string errorMessage = String.Empty;
			if (errors.Any())
			{
				if (errors.Length > 1)
				{
					foreach (ValidationError error in errors)
					{
						if (errors.Last().Message.Equals(error.Message))
						{
							errorMessage += error.Message;
							continue;
						}
						errorMessage += error.Message + ", ";
					}
					return BadRequest(errorMessage);
				}

				return BadRequest(errors[0].Message);
			}

			return Ok();
		}

		[HttpDelete]
		public async Task<IHttpActionResult> Delete(Guid id)
		{
			await questionService.DeleteQuestionAsync(id);

			return Ok();
		}
	}
}