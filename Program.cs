using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cal
{
    class Program
    {
        public const string separator = "-----------------------------";

        static void Main(string[] args)
        {
            // Поменяем региональные параметры текущего потока (чтобы double выводился через точку)
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            //Точность вычисления при делении будем запрашивать у пользователя
            Console.WriteLine("Type precision of dividing (number of digits after point):");
            int prec = Convert.ToInt32(Console.ReadLine());

            while (true)
            {
                Console.WriteLine(separator);
                Console.WriteLine("Type \"esc\" to exit or type your expression in format \"float1 [+-*/] float2\":");
                string expr = Console.ReadLine();
                if (expr == "esc")
                {
                    break;
                }
                Console.WriteLine(separator);

                //Определяем, какую операцию ввел пользователь
                string[] split_expr = expr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (split_expr.Length != 3)
                {
                    Console.WriteLine("Your expression is not recognized. Try again.");
                    continue;
                }

                switch (split_expr[1])
                {
                    case "/":
                        divide(split_expr[0], split_expr[2], prec);
                        break;
                    case "+":
                        add(split_expr[0], split_expr[2]);
                        break;
                    case "-":
                        subtract(split_expr[0], split_expr[2]);
                        break;
                    case "*":
                        multiply(split_expr[0], split_expr[2]);
                        break;
                    default:
                        Console.WriteLine("Your expression is not recognized. Try again:");
                        continue;
                }
            }

            //Console.ReadLine();
        }

        static string removeZeroesFromStart(string a)
        {
            if (a[0] == '-')
            {
                a = a.Substring(1);
                while (a.StartsWith("0") && a.IndexOf(".") != 1 && a.Length > 1)
                    a = a.Substring(1);
                return "-" + a;
            }

            while (a.StartsWith("0") && a.IndexOf(".") != 1 && a.Length > 1)
                a = a.Substring(1);
            return a;
        }
        static string removeZeroesFromEnd(string a)
        {

            if (a.IndexOf('.') == -1)
            {
                if (a == "-0") a = "0";
                return a;
            } 

            while (a.EndsWith("0")) a = a.Substring(0, a.Length - 1);
            if (a.EndsWith(".")) a = a.Substring(0, a.Length - 1);
            if (a == "-0") a = "0";

            return a;
        }
        static void addZeroesToEnd(ref string a, ref string b)
        {
            if (a.IndexOf('.') == -1 && b.IndexOf('.') == -1) return;

            int a_fraq = 0;
            int b_fraq = 0;
            if (a.IndexOf('.') > -1)
            {
                while (a.EndsWith("0"))
                {
                    a = a.Remove(a.Length - 1);
                }
                a_fraq = a.Length - a.IndexOf('.') - 1;
            }
            if (b.IndexOf('.') > -1)
            {
                while (b.EndsWith("0"))
                {
                    b = b.Remove(b.Length - 1);
                }
                b_fraq = b.Length - b.IndexOf('.') - 1;
            }

            int fraq_diff = a_fraq - b_fraq;
            if (a.IndexOf('.') == -1) a += '.';
            if (b.IndexOf('.') == -1) b += '.';
            while (fraq_diff != 0)
            {
                if (fraq_diff > 0)
                {
                    b += '0';
                    fraq_diff--;
                }
                else
                {
                    a += '0';
                    fraq_diff++;
                }
            }

            if (a.EndsWith(".")) a = a.Substring(0, a.Length - 1);
            if (b.EndsWith(".")) b = b.Substring(0, b.Length - 1);
        }
        static void addZeroesToStart(ref string a, ref string b)
        {
            int a_int = a.IndexOf('.');
            if (a_int == -1) a_int = a.Length;
            int b_int = b.IndexOf('.');
            if (b_int == -1) b_int = b.Length;


            int fraq_diff = a_int - b_int;
            while (fraq_diff != 0)
            {
                if (fraq_diff > 0)
                {
                    b = '0' + b;
                    fraq_diff--;
                }
                else
                {
                    a = '0' + a;
                    fraq_diff++;
                }
            }
            a = '0' + a;
            b = '0' + b;
        }
        static string divide(string a, string b, int prec)
        {
            if (removeZeroesFromEnd(b) == "0")
            {
                Console.WriteLine("Error: dividing by 0");
                return "0";
            }

            Console.Write("{0} / {1} = ", a, b);

            //Сначала переведем оба числа в целые, чтобы использовать целочисленное деление
            addZeroesToEnd(ref a, ref b);
            a = removeZeroesFromStart(a.Replace(".", ""));
            b = removeZeroesFromStart(b.Replace(".", ""));

            Console.WriteLine("{0} / {1}", a, b);
            Console.WriteLine();


            //Определим знак результата
            string sign = "";
            if (a[0] == '-')
            {
                sign += "-";
                a = a.Substring(1);
            }
            if (b[0] == '-')
            {
                sign += "-";
                b = b.Substring(1);
            }
            if (sign == "--") sign = "";


            string result = "";
            int a_pointer = 0;
            int a_int = Convert.ToInt32(a);
            int b_int = Convert.ToInt32(b);

            
            if (a.Length >= b.Length)
            {
                a_pointer = b.Length;
                a_int = Convert.ToInt32(a.Substring(0, a_pointer));
                b_int = Convert.ToInt32(b);

                while (a_pointer <= a.Length)
                {
                    int i = 0;
                    while (b_int * (i + 1) <= a_int)
                    {
                        i++;
                    }
                    Console.WriteLine("{0} * {1} = {2}", i, b_int, i * b_int);
                    Console.WriteLine("{0} - {1} = {2}", a_int, i * b_int, a_int - i * b_int);
                    a_int -= i * b_int;

                    result += Convert.ToString(i);
                    if (a_pointer != a.Length) a_int = (a_int * 10) + (a[a_pointer] - '0');
                    a_pointer++;

                }
            }

            if (result == "") result = "0";
            result = removeZeroesFromStart(result);
            result += ".";

            for (int j = 0; j < prec; j++)
            {

                a_int *= 10;
                int i = 0;
                while (b_int * (i + 1) <= a_int)
                {
                    i++;
                }
                Console.WriteLine("{0} * {1} = {2}", i, b_int, i * b_int);
                Console.WriteLine("{0} - {1} = {2}", a_int, i * b_int, a_int - i * b_int);                
                a_int -= i * b_int;

                result += Convert.ToString(i);
                a_pointer++;

            }
            result = sign + result;
            result = removeZeroesFromEnd(result);
            Console.WriteLine();
            Console.WriteLine("Result: {0}", result);
            return result;
        }
        static string add(string a, string b)
        {
            string result = "";
            string sign = "";

            if (a[0] == '-' && b[0] == '-')
            {
                Console.Write("{0} + {1} = ", a, b);
                a = a.Substring(1);
                b = b.Substring(1);
                sign = "-";
                Console.WriteLine("-({0} + {1})\n", a, b);
            }

            if (a[0] == '-' && b[0] != '-')
            {
                Console.WriteLine("{0} + {1} = {1} - {2}\n", a, b, a.Substring(1));
                return subtract(b, a.Substring(1));
            }

            if (a[0] != '-' && b[0] == '-')
            {
                Console.WriteLine("{0} + {1} = {0} - {2}\n", a, b, b.Substring(1));
                return subtract(a, b.Substring(1));
            }
            
            addZeroesToStart(ref a, ref b);
            addZeroesToEnd(ref a, ref b);
            Console.WriteLine(" {0}\n+\n {1}\n=", a, b);

            int point_pos = a.IndexOf('.');
            a = a.Replace(".", "");
            b = b.Replace(".", "");

            int mod = 0;
            int cur_sum = 0;

            for (int i = a.Length-1; i>=0; i--)
            {
                cur_sum = a[i] + b[i] - 2 * '0' + mod;
                result = (char)('0' + cur_sum % 10) + result;
                mod = cur_sum / 10;
            }

            if (point_pos != -1) result = result.Substring(0, point_pos) + "." + result.Substring(point_pos);
            Console.WriteLine(" " + result);
            Console.WriteLine("");

            result = sign + removeZeroesFromStart(result);
            result = removeZeroesFromEnd(result);
            Console.WriteLine("Result: {0}", result);
            return result;
        }
        static string subtract(string a, string b)
        {
            string result = "";
            string sign = "";
            

            if (a[0] == '-' && b[0] == '-')
            {
                Console.Write("{0} - {1} = ", a, b);
                a = a.Substring(1);
                b = b.Substring(1);
                sign = "-";
                Console.WriteLine("-({0} - {1})\n", a, b);
            }

            if (a[0] == '-' && b[0] != '-')
            {
                Console.WriteLine("{0} - {1} = {0} + -{1}\n", a, b);
                return add(a, '-' + b);
            }
            if (a[0] != '-' && b[0] == '-')
            {
                Console.WriteLine("{0} - {1} = {0} + {2}\n", a, b, b.Substring(1));
                return add(a, b.Substring(1));
            }

            string a_start = a;
            string b_start = b;

            addZeroesToStart(ref a, ref b);
            addZeroesToEnd(ref a, ref b);

            for(int i = 0; i < a.Length; i++)
            {
                if (a[i] > b[i]) break;
                if (a[i] < b[i])
                {
                    Console.WriteLine("{0} - {1} = - ({1} - {0})\n", a_start, b_start);
                    string t = a;
                    a = b;
                    b = t;
                    sign += "-";
                    
                    if (sign == "--") sign = "";
                    break;
                }
            }

            Console.WriteLine(" {0}\n-\n {1}\n=", a, b);

            int point_pos = a.IndexOf('.');
            a = a.Replace(".", "");
            b = b.Replace(".", "");

            int mod = 0;
            int cur_sub = 0;

            for (int i = a.Length - 1; i >= 0; i--)
            {
                cur_sub = a[i] - b[i] + mod;
                if (cur_sub < 0)
                {
                    cur_sub += 10;
                    mod = -1;
                }
                else mod = 0;
                result = (char)('0' + cur_sub) + result;
            }

            if (point_pos != -1) result = result.Substring(0, point_pos) + "." + result.Substring(point_pos);
            Console.WriteLine(" " + result);
            Console.WriteLine("");

            result = sign + removeZeroesFromStart(result);
            result = removeZeroesFromEnd(result);
            Console.WriteLine("Result: {0}", result);
            return result;
        }
        static string multiply(string a, string b)
        {
            string result = "";
            double result_d = 0;
            string sign = "";
            
            if (a[0] == '-')
            {
                sign += "-";
                a = a.Substring(1);
            }
            if (b[0] == '-')
            {
                sign += "-";
                b = b.Substring(1);
            }
            if (sign == "--") sign = "";

            Console.WriteLine(" {0}\n*\n {1}\n=", a, b);

            //addZeroesToEnd(ref a, ref b);
            double a_f = Convert.ToDouble(a);
            int point_pos = Math.Max(b.IndexOf('.'), 0);
            
            b = b.Replace(".", "");
            int placement_pos = a.Length + b.Length;
            int mult = 1;

            for (int i = b.Length-1; i>=0; i--)
            {
                double cur_plus = a_f * (b[i] - '0') * mult;

                if (i != b.Length - 1) { Console.Write('+'); }
                else { Console.Write(' '); };

                string cur_plus_s = Convert.ToString(cur_plus);
                if(cur_plus_s.IndexOf('.') == -1)
                {
                    for (int j = 0; j < placement_pos - cur_plus_s.Length; j++)
                    {
                        Console.Write(' ');

                    }
                }
                else
                {
                    for (int j = 0; j < placement_pos - cur_plus_s.IndexOf('.'); j++)
                    {
                        Console.Write(' ');

                    }
                    //Console.Write("  ");
                }
                
                //if (cur_plus_s.IndexOf('.') != -1) 

                Console.WriteLine(cur_plus);
                result_d += cur_plus;
                mult *= 10;
            }

            result = Convert.ToString(result_d);

            if (point_pos != 0)
            {
                int b_k = b.Length - point_pos;
                if (result.IndexOf('.') == -1)
                {
                    b_k = result.Length - b_k;
                }
                else
                {
                    
                    b_k =  result.IndexOf('.') - b_k;
                    result = result.Replace(".", "");
                }
                if (b_k < 0)
                {
                    while(b_k < 0)
                    {
                        result = "0" + result;
                        b_k++;
                    }

                    result = "0." + result;
                }
                else
                {
                    result = result.Substring(0, b_k) + "." + result.Substring(b_k);
                    if (b_k == 0) result = "0" + result;
                }
                
            }
            result = sign + result;
            result = removeZeroesFromEnd(result);
            Console.WriteLine("\n\nResult: {0}", result);
            //Console.WriteLine("\n\nResult: {0}", result_d);

            return "";
        }
    }
}
