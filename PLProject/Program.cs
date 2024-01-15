/*
 * Program.cs 
 * 
 * Main메소드
 * 
 */

using System;

namespace PLProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Lexer lexer = new Lexer(); //렉서
            Parser parser = new Parser(); //파서

            string input; //콘솔창에서 입력받는거

            while (true)
            {
                Console.Write("> ");
                input = Console.ReadLine();

                try
                {
                    lexer.Lex(input); //lexing
                    parser.Parse(lexer.Lexemes); //parsing
                    Print.PrintObject(parser.Top); //print
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
