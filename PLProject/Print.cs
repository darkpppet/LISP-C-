/*
 * Print.cs
 * 
 * Print 클래스(static)
 *  - 메소드
 *   - public static void PrintObject(object): object 형식에 따라 콘솔창에 출력하는 메소드
 * 
 */


using System;
using System.Collections;

namespace PLProject
{
    static class Print
    {
        public static void PrintObject(object o)
        {
            switch (o)
            {
                case ArrayList list: //리스트
                    Console.Write("(");
                    for (int i = 0; i < list.Count - 1; i++)
                        Console.Write(list[i] + " ");
                    Console.WriteLine(list[^1] + ")");
                    break;

                case object[] array: //배열
                    Console.Write("#(");
                    for (int i = 0; i < array.Length - 1; i++)
                        Console.Write(array[i] + " ");
                    Console.WriteLine(array[^1] + ")");
                    break;

                case bool: //T나 NIL
                    if ((bool)o == true)
                        Console.WriteLine("T");
                    else //if ((bool)o == false)
                        Console.WriteLine("NIL");
                    break;

                default:
                    Console.WriteLine(o);
                    break;
            }
        }
    }
}
