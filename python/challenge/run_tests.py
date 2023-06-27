import subprocess
import os
from glob import iglob


def get_folder_with_incorrect_tests():
    for f in iglob("**/test_incorrect_implementation", recursive=True):
        return f


def get_path_to_correct_tests():
    for f in iglob("**/test_correct_implementation.py", recursive=True):
        return f


def run_tests_for_correct_implementation():
    r = subprocess.run([f"pytest {get_path_to_correct_tests()}"], shell=True, stdout=subprocess.DEVNULL)
    result = "ПРОЙДЕНЫ" if not r.returncode else "НЕ ПРОЙДЕНЫ"
    print(f"Тесты на правильную имплементацию: {result}")


def run_tests_for_incorrect_implementation():
    total_count = 0
    correct_passed = 0
    tests_folder = get_folder_with_incorrect_tests()
    for filename in os.listdir(tests_folder):
        if filename.startswith("test_") and filename.endswith(".py"):
            r = subprocess.run(
                    [f"pytest {tests_folder}/{filename}"],
                    shell=True,
                    stdout=subprocess.DEVNULL
            )
            total_count += 1
            correct_passed += 1 if r.returncode else 0
    print(f"Тесты на неправильную имплементацию: КОРРЕКТНО {correct_passed} из {total_count}")


run_tests_for_correct_implementation()
run_tests_for_incorrect_implementation()
