using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

       public bool CreateBool(Person person)
        {
          return  string.IsNullOrEmpty(person.Name);
        }
    }
}
