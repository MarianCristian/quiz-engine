﻿using Qubiz.QuizEngine.Services.Option.Contract;
using System;

namespace Qubiz.QuizEngine.Services.Question.Contract
{
	public class QuestionDetail
	{
        public Guid ID { get; set; }
        
        public string QuestionText { get; set; }
        
        public Guid SectionID { get; set; }
        
        public byte Complexity { get; set; }
        
        public QuestionType Type { get; set; }
        
        public int Number { get; set; }

		public OptionDefinition[] Options { get; set; }
	}

    public enum QuestionType
    {
        SingleSelect, MultiSelect
    }

}