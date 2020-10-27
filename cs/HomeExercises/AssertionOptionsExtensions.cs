using System;
using FluentAssertions.Equivalency;

namespace HomeExercises
{
    public static class AssertionOptionsExtensions
    {
	    public static EquivalencyAssertionOptions<T> ExcludingByName<T>(
		    this EquivalencyAssertionOptions<T> options,
		    Type fromType,
		    string name)
	    {
		    options.Excluding(instance =>
			    instance.SelectedMemberInfo.DeclaringType == fromType
			    && instance.SelectedMemberInfo.Name.EndsWith(name));
		    return options;
	    }
    }
}
