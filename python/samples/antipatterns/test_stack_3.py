from python.lib.stack import Stack


def test_stack():
    stack = Stack()

    assert len(stack) == 0

    stack.push(1)
    stack.pop()
    assert len(stack) == 0

    stack.push(1)
    stack.push(2)
    stack.push(3)
    assert len(stack) == 3

    stack.pop()
    stack.pop()
    stack.pop()
    assert len(stack) == 0

    for i in range(1001):
        stack.push(i)

    for i in range(1000, -1, -1):
        assert stack.pop() == i

    ## Антипаттерн Freeride

    # 1. Непонятна область его ответственности. Складывается впечатление, что он тестирует все, однако он это делает плохо.
    # Он дает ложное чувство, что все протестировано. Хотя, например, этот тест не проверяет много важных случаев.
    #
    # 2. Таким тестам как-правило невозможно придумать внятное название.
    #
    # 3. Если что-то упадет в середине теста, будет сложно разобраться что именно пошло не так и сложно отлаживать — нужно жонглировать точками останова.
    #
    # 4. Такой тест не работает как документация. По этому сценарию непросто восстановить требования к тестируемому объекту.

    ## Мораль

    # Каждый тест должен тестировать одно конкретное требование. Это требование должно отражаться в названии теста.
    # Если вы не можете придумать название теста, у вас Free Ride!
