package ru.kontur.courses.homework;

public class Person {
    public static int idCounter = 0;
    public int age, height, weight;
    public String name;
    public Person parent;
    public int id;

    public Person(String name, int age, int height, int weight, Person parent) {
        id = idCounter++;
        this.name = name;
        this.age = age;
        this.height = height;
        this.weight = weight;
        this.parent = parent;
    }
}
