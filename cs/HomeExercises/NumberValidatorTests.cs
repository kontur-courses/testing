using System;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		
		[Test]
		[TestCase(0,
			2, 
			true,
			"precision must be a positive number",
			TestName="throws when precision not a positive number")
		]
		[TestCase(23,
			24, 
			true,
			"precision must be a non-negative number less or equal than precision",
			TestName="throws when precision less than scale")
		]
		[TestCase(24,
			24, 
			true,
			"precision must be a non-negative number less or equal than precision",
			TestName="throws when precision equals scale")
		]
		public void NumberValidator_ThrowsException
		(
			int precision,
			int scale,
			bool onlyPositive,
			string exceptionMessage
		)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);
			
			action
				.Should()
				.Throw<ArgumentException>()
				.WithMessage(exceptionMessage);
		}


		[Test]
		[TestCase
			(
				3,
				2,
				true,
				"a.sd",
				false,
				TestName = "false when chars in arguments")
		]
		[TestCase
			(
				17,
				2,
				true,
				"0.000",
				false,
				TestName = "false when scale too big")
		]
		[TestCase
			(
				17,
				5,
				true,
				"-0.000",
				false,
				TestName = "false when minus in positive only NumbersValidator")
		]
		[TestCase
			(
				3,
				2,
				true,
				"00.00",
				false,
				TestName = "false when precision in arguments too big")
		]
		[TestCase
			(
				3,
				2,
				true,
				"0",
				true,
				TestName = "true when scale is zero and argument without floating point")
		]
		[TestCase
			(
				3,
				2,
				true,
				"+0.00",
				false,
				TestName = "false when signed number is too long")
		]
		[TestCase
			(
				3,
				2,
				true,
				"",
				false,
				TestName = "false when empty string")
		]
		[TestCase
			(
				3,
				2,
				true,
				null,
				false,
				TestName = "false when null")
		]
		public void IsValidNumber_GivesResult
			(
				int precision,
				int scale,
				bool onlyPositive,
				string value,
				bool expected
			)
         		{
         			var v = new NumberValidator(precision, scale, onlyPositive);
         			var result = v.IsValidNumber(value); 
         			result.Should().Be(expected);	
         		}
	}
}