package ru.kontur.courses;

import org.junit.jupiter.api.DynamicTest;
import org.junit.jupiter.api.TestFactory;
import org.junit.platform.launcher.LauncherSession;
import org.junit.platform.launcher.core.LauncherDiscoveryRequestBuilder;
import org.junit.platform.launcher.core.LauncherFactory;
import org.junit.platform.launcher.listeners.SummaryGeneratingListener;
import ru.kontur.courses.donotopen.*;
import java.lang.reflect.InvocationTargetException;
import java.util.stream.Stream;

import static org.junit.platform.engine.discovery.DiscoverySelectors.selectClass;

public class IncorrectImplementationTest {
    @TestFactory
    Stream<DynamicTest> stream() {
        var request = LauncherDiscoveryRequestBuilder.request()
                .selectors(selectClass(WordStatisticsTest.class)).build();

        var listener = new SummaryGeneratingListener();

        return Stream.of(
                new WordStatistics01(),
                new WordStatistics02(),
                new WordStatistics03(),
                new WordStatistics04(),
                new WordStatistics123(),
                new WordStatistics998(),
                new WordStatistics999(),
                new WordStatisticsC(),
                new WordStatisticsCR(),
                new WordStatisticsE(),
                new WordStatisticsE2(),
                new WordStatisticsE3(),
                new WordStatisticsE4(),
                new WordStatisticsEN1(),
                new WordStatisticsEN2(),
                new WordStatisticsL2(),
                new WordStatisticsL3(),
                new WordStatisticsL4(),
                new WordStatisticsQWE(),
                new WordStatisticsSTA()
        ).map(it -> DynamicTest.dynamicTest(it.getClass().getSimpleName(), () -> {
            WordStatisticsTest.wordStatisticFactory = () -> {
                try {
                    return it.getClass().getConstructor().newInstance();
                } catch (InstantiationException e) {
                    throw new RuntimeException(e);
                } catch (IllegalAccessException e) {
                    throw new RuntimeException(e);
                } catch (InvocationTargetException e) {
                    throw new RuntimeException(e);
                } catch (NoSuchMethodException e) {
                    throw new RuntimeException(e);
                }
            };

            try (LauncherSession session = LauncherFactory.openSession()) {
                var launcher = session.getLauncher();

                launcher.registerTestExecutionListeners(listener);

                launcher.execute(request);
                var summary = listener.getSummary();
                if (summary.getTestsFailedCount() == 0) {
                    throw new RuntimeException("Некорректная имплементация прошла, не хватает тестов");
                }
            }
        }));
    }
}
