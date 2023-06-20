from python.lib.stack import Stack


def test_stack():
    stack = Stack()
    stack.push(1)
    stack.push(2)
    stack.push(3)

    while len(stack) > 0:
        print(stack.pop())

    ## Антипаттерн Loudmouth
    #
	# Тест не является автоматическим. Если он сломается, никто этого не заметит.
    #
	## Мораль
    #
	# Вместо вывода на консоль, используйте Assert-ы.
