from firebase import firebase
from datetime import date
import re


def firebase_init():
    firebase_url = f"https://testing-challenge.firebaseio.com"
    return firebase.FirebaseApplication(firebase_url)


def get_safe_username(username: str) -> str:
    username = re.sub(r"[^\w_)( -]", "", username.strip())
    username = re.sub(r"\s+", "_", username.strip())
    return username


def get_firebase_key_from_filename(filename: str) -> str:
    key = re.sub(r"\.py", "", filename.strip())
    return f"WordsStatistics{key.split('_')[-1]}"


def create_data_for_fb(data: dict) -> dict:
    return {
        "implementations": data,
        "lang": "py",
        "time": date.today().strftime("%Y%m%d")
    }


def send_to_firebase(user: str, data: dict) -> None:
    date_now = date.today().strftime("%Y%m%d")
    fb = firebase_init()
    r = fb.put(f"/word-statistics/{date_now}/", get_safe_username(user), create_data_for_fb(data))
