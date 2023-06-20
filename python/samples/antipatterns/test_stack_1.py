from python.lib.stack import Stack


def test_stack():
    data = []
    with open("/Users/admin/Documents/data.txt") as f:
        for line in f.readlines():
            data.append(line.split())

    stack = Stack()
    for action, num in data:
        if action == "push":
            stack.push(int(num))
        elif action == "pop":
            assert stack.pop() == int(num)

    ## Антипаттерн Local Hero

    # Тест не будет работать на машине другого человека или на Build-сервере.
    # Да и у того же самого человека после Clean Solution / переустановки ОС / повторного Clone репозитория / ...
    #

    ## Решение
    #
    # Тест не должен зависеть от особенностей локальной среды.
    # Если нужна работа с файлами, то либо включите файл в проект и настройте в свойствах его копирование в OutputDir,
    # либо поместите его в ресурсы.
    #
    # можно указать относительный путь до файла или путь из корня проекта
    # with open("../data.txt") as f:
    #     ...
