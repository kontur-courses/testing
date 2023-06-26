package ru.kontur.courses.samples;

import org.junit.jupiter.api.Test;

import java.util.EmptyStackException;
import java.util.Stack;

import static org.junit.jupiter.api.Assertions.*;

public class SpecificationTest {

    @Test
    public void constructorCreateEmptyStack() {
        var stack = new Stack<>();

        assertEquals(0, stack.size());
    }

    @Test
    public void toArrayReturnsItemsInNotPopOrder() {
        var stack = new Stack<Integer>();
        stack.add(1);
        stack.add(2);
        stack.add(3);

        assertArrayEquals(new Integer[]{1, 2, 3}, stack.toArray(Integer[]::new));
    }

    @Test
    public void pushAddsItemToStackTop() {
        var stack = new Stack<Integer>();
        stack.add(1);
        stack.add(2);
        stack.add(3);

        stack.push(42);

        assertArrayEquals(new Integer[]{1, 2, 3, 42}, stack.toArray(Integer[]::new));
    }

    @Test
    public void popOnEmptyStackFails() {
        var stack = new Stack<Integer>();

        assertThrows(EmptyStackException.class, () -> {
            stack.pop();
        });
    }

    @Test
    public void popReturnLastPushedItem() {
        var stack = new Stack<Integer>();
        stack.add(1);
        stack.add(2);
        stack.add(3);

        stack.push(42);

        assertEquals(42, stack.pop());
    }
}
