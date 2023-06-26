package ru.kontur.courses.homework;

import java.util.regex.Pattern;

public class NumberValidator {
    private final Pattern numberRegex;
    private final Boolean onlyPositive;
    private final int precision;
    private final int scale;

    public NumberValidator(int precision, int scale, Boolean onlyPositive) {
        this.precision = precision;
        this.scale = scale;
        this.onlyPositive = onlyPositive;
        if (precision <= 0)
            throw new IllegalArgumentException("precision must be a positive number");
        if (scale < 0 || scale >= precision)
            throw new IllegalArgumentException("precision must be a non-negative number less or equal than precision");
        numberRegex = Pattern.compile("^([+-]?)(\\d+)([.,](\\d+))?$");
    }

    public Boolean isValidNumber(String value) {
        // Проверяем соответствие входного значения формату N(m,k), в соответствии с правилом,
        // описанным в Формате описи документов, направляемых в налоговый орган в электронном виде по телекоммуникационным каналам связи:
        // Формат числового значения указывается в виде N(m.к), где m – максимальное количество знаков в числе, включая знак (для отрицательного числа),
        // целую и дробную часть числа без разделяющей десятичной точки, k – максимальное число знаков дробной части числа.
        // Если число знаков дробной части числа равно 0 (т.е. число целое), то формат числового значения имеет вид N(m).

        if (value == null || value.isBlank())
            return false;

        var matcher = numberRegex.matcher(value);
        var match = matcher.matches();

        if (!match)
            return false;

        // Знак и целая часть
        var intPart = matcher.group(1).length() + matcher.group(2).length();
        // Дробная часть
        var fracGroup = matcher.group(4);

        var fracPart = fracGroup == null ? 0 : fracGroup.length();

        if (intPart + fracPart > precision || fracPart > scale)
            return false;

        return !onlyPositive || !matcher.group(1).equals("-");
    }
}
