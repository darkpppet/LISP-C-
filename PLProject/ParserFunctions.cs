/*
 * ParserFunctions.cs 
 * 
 * Parser 클래스(partial)에서 함수메소드들.
 *  - 메소드: 전부 private, 반환형 void, 파라미터 없음
 *   
 */

using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace PLProject
{
    partial class Parser
    {
        private void Plus() //+
        {
            double result = (double)Evaluate(tempstack.Pop());

            while (tempstack.Count > 0)
                result += (double)Evaluate(tempstack.Pop());

            stack.Push(result);
        }

        private void Minus() //-
        {
            double result = (double)Evaluate(tempstack.Pop());

            while (tempstack.Count > 0)
                result -= (double)Evaluate(tempstack.Pop());

            stack.Push(result);
        }

        private void Multiply() //*
        {
            double result = (double)Evaluate(tempstack.Pop());

            while (tempstack.Count > 0)
                result *= (double)Evaluate(tempstack.Pop());

            stack.Push(result);
        }

        private void Division() ///
        {
            double result = (double)Evaluate(tempstack.Pop());

            while (tempstack.Count > 0)
            {
                double num = (double)Evaluate(tempstack.Pop()); 
                if (num == 0) //0으로 나누기 시도하면 익셉션
                    throw new Exception("Error: 0으로 나눌 수 없음");
                result /= num;
            }

            stack.Push(result);
        }

        private void Setq()
        {
            string symbol = (string)tempstack.Pop();
            var value = Evaluate(tempstack.Pop());

            if (tempstack.Count > 0)
                throw new Exception("Error: 문법 오류");

            if (IsReserved(symbol))
                throw new Exception("Error: 예약어에는 저장 불가");

            symbols[symbol] = value;

            stack.Push(symbol);
        }

        private void List()
        {
            ArrayList newlist = new ArrayList();

            while (tempstack.Count > 0)
                newlist.Add(Evaluate(tempstack.Pop()));

            stack.Push(newlist);
        }

        private void Car()
        {
            ArrayList originlist = (ArrayList)Evaluate(tempstack.Pop());

            if (tempstack.Count > 0)
                throw new Exception("Error: 문법 오류");

            stack.Push(originlist[0]);
        }

        private void Cdr()
        {
            ArrayList originlist = (ArrayList)Evaluate(tempstack.Pop());

            if (tempstack.Count > 0)
                throw new Exception("Error: 문법 오류");

            ArrayList newlist = (ArrayList)originlist.Clone();
            newlist.RemoveAt(0);

            stack.Push(newlist);
        }

        private void Caddr()
        {
            ArrayList originlist = (ArrayList)Evaluate(tempstack.Pop());

            if (tempstack.Count > 0)
                throw new Exception("Error: 문법 오류");

            if (2 >= originlist.Count)
                stack.Push(false);
            else
                stack.Push(originlist[2]);
        }

        private void Nth()
        {
            int index = Convert.ToInt32((double)Evaluate(tempstack.Pop()));
            ArrayList originlist = (ArrayList)Evaluate(tempstack.Pop());

            if (tempstack.Count > 0)
                throw new Exception("Error: 문법 오류");

            if (index >= originlist.Count)
                stack.Push(false);
            else
                stack.Push(originlist[index]);
        }

        private void Cons()
        {
            ArrayList newlist = new ArrayList { Evaluate(tempstack.Pop()) };
            ArrayList originlist = (ArrayList)Evaluate(tempstack.Pop());

            if (tempstack.Count > 0)
                throw new Exception("Error: 문법 오류");

            foreach (object o in originlist)
                newlist.Add(o);

            stack.Push(newlist);
        }

        private void Reverse()
        {
            ArrayList originlist = (ArrayList)Evaluate(tempstack.Pop());

            if (tempstack.Count > 0)
                throw new Exception("Error: 문법 오류");

            ArrayList newlist = (ArrayList)originlist.Clone();
            newlist.Reverse();

            stack.Push(newlist);
        }

        private void Append()
        {
            ArrayList newlist = new ArrayList();

            while (tempstack.Count > 0)
            {
                ArrayList oldlist = (ArrayList)Evaluate(tempstack.Pop());
                foreach (object o in oldlist)
                    newlist.Add(o);
            }

            stack.Push(newlist);
        }

        private void Length()
        {
            ArrayList originlist = (ArrayList)Evaluate(tempstack.Pop());

            if (tempstack.Count > 0)
                throw new Exception("Error: 문법 오류");

            stack.Push(originlist.Count);
        }

        private void Member()
        {
            object mem = Evaluate(tempstack.Pop());
            ArrayList originlist = (ArrayList)Evaluate(tempstack.Pop());

            if (tempstack.Count > 0)
                throw new Exception("Error: 문법 오류");

            ArrayList newlist = new ArrayList();
            bool isfound = false;
            foreach (object o in originlist)
            {
                if (mem.Equals(o))
                    isfound = true;
                if (isfound)
                    newlist.Add(o);
            }

            if (isfound)
                stack.Push(newlist);
            else
                stack.Push(false);
        }

        private void Assoc()
        {
            object key = Evaluate(tempstack.Pop());
            ArrayList dictionary = (ArrayList)Evaluate(tempstack.Pop());

            if (tempstack.Count > 0)
                throw new Exception("Error: 문법 오류");

            foreach (object o in dictionary)
            {
                if (key.Equals(((ArrayList)o)[0]))
                {
                    stack.Push(o);
                    break;
                }
            }
        }

        private void Remove()
        {
            object target = Evaluate(tempstack.Pop());
            ArrayList originlist = (ArrayList)Evaluate(tempstack.Pop());

            if (tempstack.Count > 0)
                throw new Exception("Error: 문법 오류");

            ArrayList newlist = new ArrayList();
            foreach (object o in originlist)
            {
                if (!target.Equals(o))
                    newlist.Add(o);
            }

            stack.Push(newlist);
        }

        private void Subst()
        {
            object newmem = Evaluate(tempstack.Pop());
            object target = Evaluate(tempstack.Pop());
            ArrayList originlist = (ArrayList)Evaluate(tempstack.Pop());

            if (tempstack.Count > 0)
                throw new Exception("Error: 문법 오류");

            ArrayList newlist = new ArrayList();
            foreach (object o in originlist)
            {
                if (target.Equals(o))
                    newlist.Add(newmem);
                else
                    newlist.Add(o);
            }

            stack.Push(newlist);
        }

        private void Atom()
        {
            object parameter = Evaluate(tempstack.Pop());

            if (tempstack.Count > 0)
                throw new Exception("Error: 문법 오류");

            if (parameter is ArrayList)
                stack.Push(false);
            else
                stack.Push(true);
        }

        private void Null()
        {
            object parameter = Evaluate(tempstack.Pop());

            if (tempstack.Count > 0)
                throw new Exception("Error: 문법 오류");

            if (parameter is bool b)
            {
                if (b == false)
                    stack.Push(true);
                else
                    stack.Push(false);
            }
            else
            {
                stack.Push(false);
            }

        }

        private void NumberP()
        {
            object parameter = Evaluate(tempstack.Pop());

            if (tempstack.Count > 0)
                throw new Exception("Error: 문법 오류");

            if (parameter is double)
                stack.Push(true);
            else
                stack.Push(false);
        }

        private void ZeroP()
        {
            object parameter = Evaluate(tempstack.Pop());

            if (tempstack.Count > 0)
                throw new Exception("Error: 문법 오류");

            if (parameter is double num)
            {
                if (num == 0)
                    stack.Push(true);
                else
                    stack.Push(false);
            }
            else
            {
                stack.Push(false);
            }
        }

        private void MinusP()
        {
            object parameter = Evaluate(tempstack.Pop());

            if (tempstack.Count > 0)
                throw new Exception("Error: 문법 오류");

            if (parameter is double num)
            {
                if (num < 0)
                    stack.Push(true);
                else
                    stack.Push(false);
            }
            else
                stack.Push(false);
        }

        private void Equal() //안됨
        {
            object parameter1 = Evaluate(tempstack.Pop());
            object parameter2 = Evaluate(tempstack.Pop());

            if (tempstack.Count > 0)
                throw new Exception("Error: 문법 오류");

            if (parameter1.Equals(parameter2))
                stack.Push(true);
            else
                stack.Push(false);
        }

        private void IsLessThan()
        {
            object parameter1 = Evaluate(tempstack.Pop());
            object parameter2 = Evaluate(tempstack.Pop());

            if (tempstack.Count > 0)
                throw new Exception("Error: 문법 오류");

            if (parameter1 is double num1 && parameter2 is double num2)
            {
                if (num1 < num2)
                    stack.Push(true);
                else
                    stack.Push(false);
            }
            else
            {
                stack.Push(false);
            }
        }

        private void IsGreaterThan()
        {
            object parameter1 = Evaluate(tempstack.Pop());
            object parameter2 = Evaluate(tempstack.Pop());

            if (tempstack.Count > 0)
                throw new Exception("Error: 문법 오류");

            if (parameter1 is double num1 && parameter2 is double num2)
            {
                if (num1 > num2)
                    stack.Push(true);
                else
                    stack.Push(false);
            }
            else
            {
                stack.Push(false);
            }
        }

        private void IsLessThanOrEqual()
        {
            object parameter1 = Evaluate(tempstack.Pop());
            object parameter2 = Evaluate(tempstack.Pop());

            if (tempstack.Count > 0)
                throw new Exception("Error: 문법 오류");

            if (parameter1 is double num1 && parameter2 is double num2)
            {
                if (num1 <= num2)
                    stack.Push(true);
                else
                    stack.Push(false);
            }
            else
            {
                stack.Push(false);
            }
        }

        private void IsGreaterThanOrEqual()
        {
            object parameter1 = Evaluate(tempstack.Pop());
            object parameter2 = Evaluate(tempstack.Pop());

            if (tempstack.Count > 0)
                throw new Exception("Error: 문법 오류");

            if (parameter1 is double num1 && parameter2 is double num2)
            {
                if (num1 >= num2)
                    stack.Push(true);
                else
                    stack.Push(false);
            }
            else
            {
                stack.Push(false);
            }
        }

        private void IsEqual()
        {
            object parameter1 = Evaluate(tempstack.Pop());
            object parameter2 = Evaluate(tempstack.Pop());

            if (tempstack.Count > 0)
                throw new Exception("Error: 문법 오류");

            if (parameter1 is double num1 && parameter2 is double num2)
            {
                if (num1 == num2)
                    stack.Push(true);
                else
                    stack.Push(false);
            }
            else
            {
                stack.Push(false);
            }
        }

        private void StringP()
        {
            object parameter = Evaluate(tempstack.Pop());

            if (tempstack.Count > 0)
                throw new Exception("Error: 문법 오류");

            if (parameter is string s)
            {
                if (new Regex(@""".*""").IsMatch(s))
                    stack.Push(true);
                else
                    stack.Push(false);
            }
            else
            {
                stack.Push(false);
            }
        }

        private void If() //안됨
        {
            object condition = tempstack.Pop();
            object iflist = tempstack.Pop();
            ((ArrayList)iflist).Reverse();
            object elselist = null;
            if (tempstack.Count > 0)
            {
                elselist = tempstack.Pop();
                ((ArrayList)elselist).Reverse();
            }

            if (tempstack.Count > 0)
                throw new Exception("Error: 문법 오류");

            switch (condition)
            {
                case bool b when b == true:
                    foreach (object o in (ArrayList)iflist)
                        tempstack.Push(Evaluate(o));
                    CallFunction();
                    break;

                case bool b when b == false && elselist != null:
                    foreach (object o in (ArrayList)elselist)
                        tempstack.Push(Evaluate(o));
                    CallFunction();
                    break;

                case bool b when b == false:
                    stack.Push(false);
                    break;

                default:
                    throw new Exception("Error: 문법 오류");
            }
        }

        private void Cond() //안됨
        {
        }

        private void MiniList(string temp)
        {
            ArrayList newlist = new ArrayList() { temp };

            while (tempstack.Count > 0)
                newlist.Add(tempstack.Pop());

            stack.Push(newlist);
        }
    }
}
