using System;
using System.Linq.Expressions;
using FluentAssertions.Equivalency;
using FluentAssertions.Primitives;

namespace HomeExercises
{
	public static class FluentAssertionExtensions
	{
		public static void BeEquivalentToPersonIgnoringIdentifiers(this ObjectAssertions assertions, Person person)
		{
			assertions.BeAssignableTo<Person>()
				.And.BeEquivalentTo(person, ExcludingPersonIdentifiers);
		}

		private static EquivalencyAssertionOptions<Person> ExcludingPersonIdentifiers(
			this EquivalencyAssertionOptions<Person> assertionOptions)
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