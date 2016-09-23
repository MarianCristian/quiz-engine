using Qubiz.QuizEngine.Services.Option.Contract;

namespace Qubiz.QuizEngine.Services.Question.Contract
{
	public class QuestionDetail : QuestionDefinition
	{
		public OptionDefinition[] Options { get; set; }
	}
}