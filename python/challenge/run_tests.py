import subprocess
import os
import fb
from glob import iglob
from your_name import AUTHORS


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
    return True if not r.returncode else False


def run_tests_for_incorrect_implementation():
    firebase_data = {}
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
            passed = 1 if r.returncode else 0
            correct_passed += passed
            firebase_data[fb.get_firebase_key_from_filename(filename)] = passed
    print(f"Тесты на неправильную имплементацию: КОРРЕКТНО {correct_passed} из {total_count}")
    fb.send_to_firebase(AUTHORS, firebase_data)


if __name__ == "__main__":
    if not AUTHORS:
        raise ValueError("Укажите фамилии учеников в файл /python/challenge/your_name.py")
    if run_tests_for_correct_implementation():
        run_tests_for_incorrect_implementation()
