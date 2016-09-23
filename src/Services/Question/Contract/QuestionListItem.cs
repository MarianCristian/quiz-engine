using System;

namespace Qubiz.QuizEngine.Services.Question.Contract
{
	public class QuestionListItem
    {
        public Guid ID { get; set; }
        public int Number { get; set; }
        public string Section { get; set; }
    }
}