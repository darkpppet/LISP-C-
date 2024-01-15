/*
 * Parser.cs 
 * 
 * Parser 클래스(partial)에서 함수메소드들 빼고
 *  - 멤버변수
 *   - private readonly Stack stack: lexeme들을 넣어둘 스택
 *   - private readonly Stack tempstack: 한 괄호에서 lexeme들을 넣어둘 스택
 *   - private readonly Hashtable symbols: 심볼들을 저장해둘 해시테이블; 심볼을 키로 사용
 *   - private readonly ReadOnlyCollection<string> reservedWords: 예약어들이 들어있는 리스트
 *  - 프로퍼티
 *   - public object Top: stack의 맨 위값을 get 할 수 있는 프로퍼티
 *  - 메소드
 *   - private bool IsReserved(object): 파라미터가 예약어인지를 확인하는 메소드
 *   - private object Evaluate(object): 파라미터(심볼)의 value를 구해서 반환하는 메소드
 *   - private void CallFunction(): Function메소드에서 lisp함수를 호출할 때 호출하는 메소드. lisp함수명을 분석하여 적절한 메소드를 호출한다.
 *   - private void Function(): Parse메소드에서 한 최소 괄호 단위마다(괄호가 닫힐 때 마다) 호출. 괄호 안의 식을 수행해서 그 결과를 다시 스택에 넣음
 *   - public void Parse(Queue<string>): 파라미터의 렉심들을 파싱하여 명령을 수행하는 메소드
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace PLProject
{
    partial class Parser
    {
        private readonly Stack stack = new Stack(); //스택
        private readonly Stack tempstack = new Stack(); //임시 스택; 한 괄호짜리
        private readonly Hashtable symbols = new Hashtable(); //심볼들 저장하는 해시테이블
        private readonly ReadOnlyCollection<string> reservedWords = new List<string> //reserved word들 리스트
        {
            "+", "-", "*", "/", ">", "<", ">=", "<=", "=",
            "SETQ", "LIST", "CAR", "CDR", "CADDR", "NTH", "CONS", "REVERSE", "APPEND", "LENGTH", "MEMBER", "ASSOC", "REMOVE", "SUBST",
            "ATOM", "NULL", "NUMBERP", "ZEROP", "MINUSP", "EQUAL", "STRINGP",
            "IF", "COND"
        }.AsReadOnly();

        //Top 프로퍼티
        public object Top => Evaluate(stack.Peek());

        //reserved word인지 확인하는 메소드
        private bool IsReserved(object lexeme)
        {
            if (lexeme is string s)
            {
                foreach (string word in reservedWords)
                {
                    if (s == word)
                        return true;
                }
            }
            return false;
        }

        //symbol을 evaluate하는 메소드
        private object Evaluate(object symbol)
        {
            return symbol switch
            {
                string s when (new Regex(@"'.*").IsMatch(s)) => double.TryParse(s[1..], out double numbervalue) ? numbervalue : s[1..], //'로 시작하면 '빼고 그자체가 value
                string s when (new Regex(@""".*""").IsMatch(s)) => s, //문자열은 그대로 리턴
                string s when (new Regex(@"#\\.").IsMatch(s)) => s, //문자는 그대로 리턴
                string s when (double.TryParse(s, out double numbervalue)) => numbervalue, //숫자면 그자체가 value
                string s when (s == "T") => true, //T는 true
                string s when (s == "NIL") => false, //NIL은 false
                string s when IsReserved(s) => throw new Exception("Error: 예약어"),
                string s when symbols.ContainsKey(s)=> symbols[s], //심볼에 값이 저장되어 있으면 저장된 value 찾아옴
                string s => throw new Exception($"Error: {s}에는 값이 없음"), //심볼에 값이 저장되어 있지 않으면 예외
                _ => symbol, //심볼(스트링)이 아니면 그대로 반환
            };
        }

        private void CallFunction()
        {
            switch (tempstack.Pop())
            {
                case "+":
                    Plus();
                    break;

                case "-":
                    Minus();
                    break;

                case "*":
                    Multiply();
                    break;

                case "/":
                    Division();
                    break;

                case ">":
                    IsGreaterThan();
                    break;

                case "<":
                    IsLessThan();
                    break;

                case ">=":
                    IsGreaterThanOrEqual();
                    break;

                case "<=":
                    IsLessThanOrEqual();
                    break;

                case "=":
                    IsEqual();
                    break;

                case "SETQ":
                    Setq();
                    break;

                case "LIST":
                    List();
                    break;

                case "CAR":
                    Car();
                    break;

                case "CDR":
                    Cdr();
                    break;

                case "CADDR":
                    Caddr();
                    break;

                case "NTH":
                    Nth();
                    break;

                case "CONS":
                    Cons();
                    break;

                case "REVERSE":
                    Reverse();
                    break;

                case "APPEND":
                    Append();
                    break;

                case "LENGTH":
                    Length();
                    break;

                case "MEMBER":
                    Member();
                    break;

                case "ASSOC":
                    Assoc();
                    break;

                case "REMOVE":
                    Remove();
                    break;

                case "SUBST":
                    Subst();
                    break;

                case "ATOM":
                    Atom();
                    break;

                case "NULL":
                    Null();
                    break;

                case "NUMBERP":
                    NumberP();
                    break;

                case "ZEROP":
                    ZeroP();
                    break;

                case "MINUSP":
                    MinusP();
                    break;

                case "EQUAL":
                    Equal();
                    break;

                case "STRINGP":
                    StringP();
                    break;

                case "IF":
                    If();
                    break;

                case "COND":
                    Cond();
                    break;

                case string symbol:
                    MiniList(symbol);
                    break;
            }
        }

        //Parse 메소드의 서브메소드; 한 괄호마다(닫는 괄호마다) 호출
        private void Function()
        {
            tempstack.Clear();

            //스택에서 여는괄호까지 빼서 임시스택에 푸시
            do
            {
                tempstack.Push(stack.Pop());
                if (tempstack.Peek() is string s)
                {
                    if (new Regex(@"['#]?\(").IsMatch(s)) //(, '(, #(
                        break;
                }
            } while (true);

            switch (tempstack.Pop())
            {
                case string s when s == @"(": //(: 식
                    CallFunction();
                    break;

                case string s when s == @"'(": //'(: 리스트
                    ArrayList list = new ArrayList();
                    while (tempstack.Count > 0)
                        list.Add(tempstack.Pop());
                    stack.Push(list);
                    break;

                case string s when s == @"#(": //#(: 배열
                    object[] array = new object[tempstack.Count];
                    for (int i = 0; tempstack.Count > 0; i++)
                        array[i] = tempstack.Pop();
                    stack.Push(array);
                    break;
            }
        }

        public void Parse(Queue<string> lexemes)
        {
            stack.Clear();

            //parsing
            while (lexemes.Count > 0)
            {
                string temp = lexemes.Dequeue();

                if (temp == ")") //닫는소괄호가 나오면 Function() 호출
                    Function();
                else //이외는 전부 스택에 푸시
                    stack.Push(temp);
            }

            if (stack.Count != 1)
                throw new Exception($"Error: 문법 오류");
        }
    }
}
