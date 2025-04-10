# Тестирование

Это блок о написании правильных и полезных тестов.

Пройдя блок, ты:

- Узнаешь паттерны создания тестов:
    - каноническую структуру теста AAA
    - правила именования тестов, чтобы они работали как спецификация
- Познакомишься с антипаттернами, которые приводят к хрупкости, сложности и трудночитаемости
- Получишь опыт тестирования "чёрного ящика" и "белого ящика"
- Поймёшь, когда лучше работают тесты, а когда code review
- Почувствуешь пользу от написания тестов


## Необходимые знания

Понадобится знание C#, JS, Java или Python.


## Самостоятельная подготовка

### C#
1. Познакомься с NUnit, если ещё не знаком, научись подключать его к проекту через NuGet
2. Изучи возможности синтаксиса NUnit по этому [примеру](https://github.com/nunit/nunit-csharp-samples/blob/master/syntax/AssertSyntaxTests.cs) или по [документации](https://github.com/nunit/docs/wiki/NUnit-Documentation)
3. Научись запускать тесты из Visual Studio с помощью ReSharper по [инструкции](https://www.jetbrains.com/resharper/features/unit_testing.html)
4. Изучи возможности синтаксиса [FluentAssertions](https://fluentassertions.com/introduction)
5. Установи .NET Framework 4.8

### JS
1. Познакомься с Mocha, если ещё не знаком, научись подключать его через npm (Yarn)
2. Изучи возможности синтаксиса [Mocha](https://mochajs.org/), [ChaiJS](https://www.chaijs.com/api/bdd/)
3. Научись запускать тесты в терминале (`npm test` или `yarn test`), из WebStorm по [инструкции](https://www.jetbrains.com/help/webstorm/testing.html) или другой любимой JavaScript IDE
4. Если пока плохо знаком с Node.js и ES6, то начни с Шага 1 этого [туториала](https://github.com/kontur-courses/frontend-starter-tutorial)

### Java
1. Познакомься с JUnit, если ещё не знаком, научись его подключать через Gradle
2. Изучи возможности синтаксиса, [документация](https://junit.org/junit5/docs/5.0.1/api/org/junit/jupiter/api/Assertions.html)
3. Научись запускать тесты JUnit5 в IDE
4. Изучи возможности синтаксиса [AssertJ](https://assertj.github.io/doc/)

### Python
1. Познакомься c pytest, если ещё не знаком ([документация](https://docs.pytest.org/en/7.3.x/))
2. Научись запускать тесты из PyCharm по [инструкции](https://www.jetbrains.com/help/pycharm/performing-tests.html#run-tests-in-parallel)
3. Почитай о том, что такое фикстуры в pytest (например, на [хабре](https://habr.com/ru/articles/448786/))
4. Установи с помощью pip в окружение библиотеки из python/requirements.txt - используй команду `pip install -r python/requirements.txt`

## Очная встреча

~ 3 часа.


## Закрепление материала

1. Спецзадание __Ретротестирование__  
Вспомни одну-две решенные задачи. Какие тесты пригодились бы, если бы решение надо было дополнить или переписать?
2. Спецзадание __Test infection__  
Решив задачу по программированию, напиши на нее модульные тесты.
