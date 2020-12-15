function dogFactory(name, age, breed) {
  return {
    name: name,
    age: age,
    breed: breed,
    bark()
    {
      console.log("Wuff");
    }
  }
};

let abc = {
  obj1: dogFactory("a", "b", "c"),
  obj2: dogFactory("a", "b", "c")
};
console.log(abc);
abc.obj1.bark();
