import os
import fb
import pytest
from glob import iglob
from your_name import AUTHORS
from io import StringIO
from contextlib import redirect_stdout


def get_folder_with_incorrect_tests():
    for f in iglob("**/test_incorrect_implementation", recursive=True):
        return f


def get_path_to_correct_tests():
    for f in iglob("**/test_correct_implementation.py", recursive=True):
        return f


def run_tests_for_correct_implementation():
    temp_stdout = StringIO()
    with redirect_stdout(temp_stdout):
        tests_result = pytest.main([get_path_to_correct_tests(),])
    result = "ПРОЙДЕНЫ" if not tests_result else "НЕ ПРОЙДЕНЫ"
    print(f"Тесты на правильную имплементацию: {result}")
    return bool(not tests_result)


def run_tests_for_incorrect_implementation():
    temp_stdout = StringIO()
    firebase_data = {}
    total_count = 0
    correct_passed = 0
    tests_folder = get_folder_with_incorrect_tests()
    for filename in os.listdir(tests_folder):
        if filename.startswith("test_") and filename.endswith(".py"):
            with redirect_stdout(temp_stdout):
                test_result = pytest.main([f"{tests_folder}/{filename}",])
            total_count += 1
            correct_passed += test_result
            firebase_data[fb.get_firebase_key_from_filename(filename)] = test_result
    print(f"Тесты на неправильную имплементацию: КОРРЕКТНО {correct_passed} из {total_count}")
    fb.send_to_firebase(AUTHORS, firebase_data)


if __name__ == "__main__":
    if not AUTHORS:
        raise ValueError("Укажите фамилии учеников в файл /python/challenge/your_name.py")
    if run_tests_for_correct_implementation():
        run_tests_for_incorrect_implementation()
