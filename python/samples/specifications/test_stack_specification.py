import pytest

from python.lib.stack import Stack, StackIsEmptyException


class TestStackSpecification:

    def test_create_empty_stack(self) -> None:
        stack = Stack()

        assert len(stack) == 0

    def test_push_items_to_empty_stack(self) -> None:
        stack = Stack([1, 2, 3])

        assert len(stack) == 3
        assert stack.pop() == 3
        assert stack.pop() == 2
        assert stack.pop() == 1
        assert len(stack) == 0

    def test_to_array_returns_items_in_pop_order(self) -> None:
        stack = Stack([1, 2, 3])

        assert stack.to_list() == [3, 2, 1]

    def test_add_items_on_stack_top(self) -> None:
        stack = Stack([1, 2, 3])
        stack.push(42)

        assert stack.to_list() == [42, 3, 2, 1]

    def test_pop_on_empty_stack_fails(self) -> None:
        stack = Stack()

        with pytest.raises(StackIsEmptyException):
            stack.pop()

    def test_pop_returns_last_pushed_item(self) -> None:
        stack = Stack([1, 2, 3])
        stack.push(42)

        assert stack.pop() == 42
