using Asana.Library.Models; // Or whatever namespace you defined in ToDo.cs
using System;

namespace Asana
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ToDo MyFirstToDo = new ToDo();
            Console.WriteLine(MyFirstToDo.Name?.Length);
        }
    }
}