package ru.kontur.courses.samples;

import org.junit.jupiter.api.Test;

import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;
import java.util.stream.IntStream;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;

public class AAATest {
    @Test
    public void giveResultOfSameSizeOnEqualSizeArrays() {
        var arr = new int[]{1};
        var arr2 = new int[]{2};

        var result = zipJava9(arr, arr2);

        assertEquals(1, result.get(0).getKey());
        assertEquals(2, result.get(0).getValue());
    }

    @Test
    public void beEmptyWhenBothInputsAreEmpty() {
        var arr = new int[0];
        var arr2 = new int[0];

        var result = zipJava9(arr, arr2);

        assertTrue(result.isEmpty());
    }

    @Test
    public void beEmptyWhenFirstIsEmpty() {
        var arr = new int[0];
        var arr2 = new int[]{1, 2};

        var result = zipJava9(arr, arr2);

        assertTrue(result.isEmpty());
    }

    @Test
    public void beEmptyWhenSecondIsEmpty() {
        var arr = new int[]{1, 2};
        var arr2 = new int[0];

        var result = zipJava9(arr, arr2);

        assertTrue(result.isEmpty());
    }

    @Test
    public void haveLengthOfSecondWhenFirstContainsMoreElements() {
        var arr = new int[]{1, 3, 5, 7};
        var arr2 = new int[]{2, 4};

        var result = zipJava9(arr, arr2);

        assertEquals(1, result.get(0).getKey());
        assertEquals(2, result.get(0).getValue());
        assertEquals(3, result.get(1).getKey());
        assertEquals(4, result.get(1).getValue());
    }

    @Test
    public void haveLengthOfFirstWhenSecondContainsMoreElements() {
        var arr1 = new int[]{1, 3};
        var arr2 = new int[]{2, 4, 6, 8};

        var result = zipJava9(arr1, arr2);

        assertEquals(1, result.get(0).getKey());
        assertEquals(2, result.get(0).getValue());
        assertEquals(3, result.get(1).getKey());
        assertEquals(4, result.get(1).getValue());
    }

    public static List<Map.Entry<Integer, Integer>> zipJava9(int[] as, int[] bs) {
        return IntStream.range(0, Math.min(as.length, bs.length))
                .mapToObj(i -> Map.entry(as[i], bs[i])).collect(Collectors.toList());
    }
}
