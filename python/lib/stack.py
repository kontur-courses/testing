from typing import Optional, Any, List


class StackIsEmptyException(Exception):
    pass


class Stack:

    def __init__(self, values: Optional[List[Any]] = None) -> None:
        self.stack = values or []

    def push(self, value: Any) -> None:
        self.stack.append(value)

    def pop(self) -> Any:
        if len(self.stack) <= 0:
            raise StackIsEmptyException
        return self.stack.pop()

    def top(self) -> List[Any]:
        if len(self.stack) <= 0:
            raise StackIsEmptyException
        return self.stack[-1]

    def to_list(self) -> List[Any]:
        return list(reversed(self.stack))

    def __len__(self) -> int:
        return len(self.stack)
