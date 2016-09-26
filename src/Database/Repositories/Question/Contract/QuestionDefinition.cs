using Qubiz.QuizEngine.Database.Entities;
using System;

namespace Qubiz.QuizEngine.Database.Repositories.Question.Contract
{
	public class QuestionDefinition
	{
        public Guid ID { get; set; }
        
        public string QuestionText { get; set; }
        
        public Guid SectionID { get; set; }
        
        public byte Complexity { get; set; }
        
        public QuestionType Type { get; set; }
        
        public int Number { get; set; }
    }
}