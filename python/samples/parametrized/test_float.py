import pytest


@pytest.fixture(params=[(12.0, 3.0, 4), (12.0, 2.0, 6), (12.0, 4.0, 3)])
def divide_test_cases(request):
    return request.param


@pytest.mark.parametrize("input_str, expected",
                         [("123", 123), ("1.1", 1.1), ("1.1e1", 1.1e1), ("1.1e-1", 1.1e-1), ("-0.1", -0.1)])
def test_parse_with_invariant_culture(input_str, expected):
    assert pytest.approx(float(input_str), 0.0001) == expected


def test_divide(divide_test_cases):
    a, b, expected = divide_test_cases
    assert pytest.approx(a / b, 0.0001) == expected
