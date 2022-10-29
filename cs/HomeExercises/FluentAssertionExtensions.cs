using System;
using System.Linq.Expressions;
using FluentAssertions.Equivalency;
using FluentAssertions.Primitives;

namespace HomeExercises
{
	public static class FluentAssertionExtensions
	{
		public static EquivalencyAssertionOptions<T> ExcludingPersonIdentifiers<T>(
			this EquivalencyAssertionOptions<T> assertionOptions)
		{
			Expression<Func<IMemberInfo, bool>> excludingExpression = memberInfo =>
				CheckPersonIdMember(memberInfo);
			return assertionOptions.Excluding(excludingExpression);
		}

		private static bool CheckPersonIdMember(IMemberInfo memberInfo)
		{
			return memberInfo.SelectedMemberInfo != null
				&& memberInfo.SelectedMemberInfo.DeclaringType == typeof(Person)
				&& memberInfo.SelectedMemberInfo.Name == nameof(Person.Id)
				&& memberInfo.SelectedMemberInfo.MemberType == typeof(int);
		}
	}
}