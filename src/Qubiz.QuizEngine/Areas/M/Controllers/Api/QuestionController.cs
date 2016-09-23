using Qubiz.QuizEngine.Areas.M.Models;
using Qubiz.QuizEngine.Infrastructure;
using Qubiz.QuizEngine.Services;
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
			Qubiz.QuizEngine.Services.Models.PagedResult<Qubiz.QuizEngine.Services.Models.QuestionListItem> questions = await questionService.GetQuestionsByPageAsync(pageNumber, itemsPerPage);
			if (questions.Items == null && questions.TotalCount == 0)
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
			Qubiz.QuizEngine.Services.Models.QuestionDetail question = await questionService.GetQuestionByID(id);

			return Ok(question.DeepCopyTo<Question>());
		}

		[HttpPut]
		public async Task<IHttpActionResult> Put(Question question)
		{
			ValidationError[] errors = await questionService.UpdateQuestionAsync(question.DeepCopyTo<Qubiz.QuizEngine.Services.Models.QuestionDetail>());
			string errorMessage = String.Empty;
			if (errors.Any())
			{
				if (errors.Length > 1)
				{
					foreach (ValidationError error in errors)
					{
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
			ValidationError[] errors = await questionService.AddQuestionAsync(question.DeepCopyTo<Qubiz.QuizEngine.Services.Models.QuestionDetail>());
			string errorMessage = String.Empty;
			if (errors.Any())
			{
				if (errors.Length > 1)
				{
					foreach (ValidationError error in errors)
					{
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