package ru.kontur.courses.samples;

import org.junit.jupiter.api.Test;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.util.Arrays;
import java.util.Stack;
import java.util.stream.Collectors;

import static org.junit.jupiter.api.Assertions.*;

public class AntiPatterns {

    /**
     * ## Антипаттерн Local Hero
     * <p>
     * Тест не будет работать на машине другого человека или на Build-сервере.
     * Да и у того же самого человека после сборки проекта / переустановки ОС / повторного Clone репозитория / ...
     * <p>
     * ## Решение
     * <p>
     * Тест не должен зависеть от особенностей локальной среды.
     * Если нужна работа с файлами, то либо включите файл в проект и настройте в свойствах его копирование в OutputDir,
     * либо поместите его в ресурсы.
     * <p>
     * var lines = File.ReadAllLines(@"data.txt")
     * var lines = Resources.data.Split(new []{"\r\n"}, StringSplitOptions.RemoveEmptyEntries)
     *
     * @throws IOException
     */
    @Test
    public void localHeroTest() throws IOException {
        var file = new File("C:\\work\\edu\\\\testing-course\\Patterns\\\\build\\data.txt");
        var stream = new FileInputStream(file);
        stream.close();
        var byteArray = stream.readAllBytes();
        var textFile = new String(byteArray);
        var lines = Arrays.stream(textFile.split("\n")).map(it -> it.split(" ")).toList();

        var stack = new Stack<>();
        for (String[] line : lines) {
            if (line[0].equals("push")) {
                stack.push(line[1]);
            } else {
                assertEquals(line[1], stack.pop());
            }
        }
    }

    /**
     * /*
     * ## Антипаттерн Loudmouth
     * <p>
     * Тест не является автоматическим. Если он сломается, никто этого не заметит.
     * <p>
     * ## Мораль
     * <p>
     * Вместо вывода на консоль, используйте Assert-ы.
     */
    @Test
    public void loudMouth() {
        var stack = new Stack<Integer>();
        stack.push(10);
        stack.push(20);
        stack.push(30);
        while (!stack.isEmpty())
            System.out.println(stack.pop());
    }

    /**
     * ## Антипаттерн Freeride
     * <p>
     * 1. Непонятна область его ответственности. Складывается впечатление, что он тестирует все, однако он это делает плохо.
     * Он дает ложное чувство, что все протестировано. Хотя, например, этот тест не проверяет много важных случаев.
     * <p>
     * 2. Таким тестам как-правило невозможно придумать внятное название.
     * <p>
     * 3. Если что-то упадет в середине теста, будет сложно разобраться что именно пошло не так и сложно отлаживать — нужно жонглировать точками останова.
     * <p>
     * 4. Такой тест не работает как документация. По этому сценарию непросто восстановить требования к тестируемому объекту.
     * <p>
     * ## Мораль
     * <p>
     * Каждый тест должен тестировать одно конкретное требование. Это требование должно отражаться в названии теста.
     * Если вы не можете придумать название теста, у вас Free Ride!
     */
    @Test
    public void freeRide() {
        var stack = new Stack<Integer>();
        assertTrue(stack.isEmpty());
        stack.push(1);
        stack.pop();
        assertTrue(stack.isEmpty());
        stack.push(1);
        stack.push(2);
        stack.push(3);
        assertEquals(3, stack.size());
        stack.pop();
        stack.pop();
        stack.pop();
        assertTrue(stack.isEmpty());
        for (var i = 0; i < 1000; i++) {
            stack.push(i);
        }
        for (var i = 1000; i > 0; i--) {
            assertEquals(i - 1, stack.pop());
        }
    }

    /**
     * ## Антипаттерн Overspecification
     * <p>
     * 1. Непонятна область ответственности. Сложно придумать название. Не так плохо, как FreeRide, но плохо.
     * <p>
     * 2. Изменение API роняет сразу много подобных тестов, создавая много рутинной работы по их починке.
     * <p>
     * 3. Если все тесты будут такими, то при появлении бага, падают они большой компанией.
     * <p>
     * <p>
     * ## Мораль
     * <p>
     * Сфокусируйтесь на проверке одного конкретного требования в каждом тесте.
     * Не старайтесь проверить "за одно" какое-то требование сразу во всех тестах — это может выйти боком.
     * <p>
     * Признак возможной проблемы — более одного Assert на метод.
     */
    @Test
    public void overSpecification() {
        var stack = new Stack<Integer>();
        stack.add(1);
        stack.add(2);
        stack.add(3);
        stack.add(4);
        stack.add(5);

        var result = stack.pop();
        assertEquals(5, result);
        assertFalse(stack.isEmpty());
        assertEquals(4, stack.size());
        assertEquals(4, stack.peek());
        assertTrue(Arrays.equals(new int[]{1, 2, 3, 4}, Arrays.stream(stack.toArray()).mapToInt(it -> (int)it).toArray()));
    }
}
