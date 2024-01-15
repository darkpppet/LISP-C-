/*
 * Lexer.cs 
 * 
 * Lexer 클래스
 *  - 프로퍼티
 *   - public Queue<string> Lexemes: Lex메소드로 lexing한 스트링들을 저장하는 리스트(큐)를 get 할 수 있는 프로퍼티
 *  - 메소드 
 *   - public void Lex(string): string을 lexing하여 객체 내부의 리스트에 저장하는 메소드
 * 
 */

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PLProject
{
    class Lexer
    {
        public Queue<string> Lexemes { get; } = new Queue<string>();

        public void Lex(string input)
        {
            int index = 0; //인풋 스트링의 인덱스로 사용할 것
            string lexeme = ""; //렉심

            Lexemes.Clear();
            input += " "; //범위 예외 방지하기 위해 input string 뒤에 공백 추가

            //lexing
            while (index < input.Length - 1)
            {
                //구분자: 공백, (, ), ', "
                switch (input[index])
                {
                    //(, )은 그냥 하나의 lexeme
                    case '(': case ')':
                        lexeme += input[index++].ToString();
                        Lexemes.Enqueue(lexeme);
                        lexeme = "";
                        break;

                    //'와 #은 다음 렉심과 이어서 저장
                    case '\'': case '#':
                        lexeme += input[index++].ToString();
                        break;

                    //"은 다음 "까지 한 lexeme으로 저장
                    case '\"': //문자열
                        lexeme += input[index++].ToString();
                        while (input[index] != '\"')
                            lexeme += input[index++].ToString();
                        lexeme += input[index++].ToString();
                        Lexemes.Enqueue(lexeme);
                        lexeme = "";
                        break;

                    //공백은 무시
                    case ' ':
                        index++;
                        break;

                    //; 뒤에는 전부 주석(무시)
                    case ';':
                        return;

                    //다음 구분자 전까지 하나의 lexeme으로 저장
                    default:
                        lexeme += input[index++].ToString().ToUpper();
                        while (!new Regex(@"[ #()'""]").IsMatch(input[index].ToString()))
                            lexeme += input[index++].ToString().ToUpper();
                        Lexemes.Enqueue(lexeme);
                        lexeme = "";
                        break;
                }
            }
        }
    }
}
