using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace hw1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private static string IPaddr = new IPAddress(Dns.GetHostByName(Dns.GetHostName()).AddressList[0].Address).ToString();
        string myConnectionString = "SERVER =" + IPaddr + "; DATABASE=csharp_partice ; UID=root ; PASSWORD=Yisinglabuse ; charset=utf8;";

        public MainWindow()
        {
            InitializeComponent();
        }

        string input_string = "";

        Stack<string> operands = new Stack<string>();
        Stack<string> operators = new Stack<string>();

        private void P1_Click(object sender, RoutedEventArgs e)
        {
            input_string = input_string + "1";
            input_prefix.Text = input_string;
        }

        private void P4_Click(object sender, RoutedEventArgs e)
        {
            input_string = input_string + "4";
            input_prefix.Text = input_string;
        }

        private void P7_Click(object sender, RoutedEventArgs e)
        {
            input_string = input_string + "7";
            input_prefix.Text = input_string;
        }

        private void P0_Click(object sender, RoutedEventArgs e)
        {
            input_string = input_string + "0";
            input_prefix.Text = input_string;
        }

        private void P2_Click(object sender, RoutedEventArgs e)
        {
            input_string = input_string + "2";
            input_prefix.Text = input_string;
        }

        private void P5_Click(object sender, RoutedEventArgs e)
        {
            input_string = input_string + "5";
            input_prefix.Text = input_string;
        }

        private void P8_Click(object sender, RoutedEventArgs e)
        {
            input_string = input_string + "8";
            input_prefix.Text = input_string;
        }

        private void P3_Click(object sender, RoutedEventArgs e)
        {
            input_string = input_string + "3";
            input_prefix.Text = input_string;
        }

        private void P6_Click(object sender, RoutedEventArgs e)
        {
            input_string = input_string + "6";
            input_prefix.Text = input_string;
        }

        private void P9_Click(object sender, RoutedEventArgs e)
        {
            input_string = input_string + "9";
            input_prefix.Text = input_string;
        }

        private void Pplus_Click(object sender, RoutedEventArgs e)
        {
            input_string = input_string + "+";
            input_prefix.Text = input_string;
        }

        private void Pminus_Click(object sender, RoutedEventArgs e)
        {
            input_string = input_string + "-";
            input_prefix.Text = input_string;
        }

        private void Pmulti_Click(object sender, RoutedEventArgs e)
        {
            input_string = input_string + "*";
            input_prefix.Text = input_string;
        }

        private void Pdiv_Click(object sender, RoutedEventArgs e)
        {
            input_string = input_string + "/";
            input_prefix.Text = input_string;
        }

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {

            input_string = input_string.Remove(input_string.Length - 1, 1);
            input_prefix.Text = input_string;
            
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            input_prefix.Text = "";
            postfix.Text = "";
            demical.Text = "";
            binary.Text = "";
            input_string = "";
            pre = "";
            post = "";
            s = "";
            result = "";
        }

        string s, result = "", pre, post;
        private void Pequal_Click(object sender, RoutedEventArgs e)
        {

            if( input_string.Length != 0 )
            {
                var a = new System.Data.DataTable().Compute(input_string, "");
                s = Convert.ToString(a);
                demical.Text = s;

                int point = 0;
                while ( point < s.Length )
                {
                    if (s[point] == '.') break;
                    point++;
                }

                result = "";
                if ( point == s.Length )
                {
                    int re = int.Parse(s);
                    result = Convert.ToString(re,2);
                }
                else
                {
                    string in_part = s.Substring(0, point), float_part = s.Substring(point+1);
                    int re = int.Parse(in_part);
                    result = Convert.ToString(re, 2) + ".";
                    double dd = double.Parse(float_part);
                    if (dd < 0) dd *= -1;
                    for (int i = 0; i < float_part.Length; i++) dd *= 0.1;
                    for ( int i = 0; i < 10; i++ )
                    {
                        dd *= 2;
                        if (dd > 1)
                        {
                            result += "1";
                            dd -= 1;
                        }
                        else if (dd < 1) result += "0";
                        else
                        {
                            result += "1";
                            break;
                        }
                    }

                }

                binary.Text = result;

                pre = toPrefix(input_string);
                post = toPostfix(input_string);

                input_prefix.Text = pre;
                postfix.Text = post;
            }

            //connect to database here
            MySqlConnection conn = new MySqlConnection(myConnectionString);
            conn.Open();
            string compare = "SELECT infix FROM calculator WHERE infix = '" + input_string + "'" ;
            string query =
                "INSERT INTO calculator(infix,prefix,postfix,ans_demical,ans_binary) VALUES('" + input_string + "','" + pre + "','" + post + "','" + s + "','" + result + "')";

            MySqlCommand find = new MySqlCommand(compare, conn);
            MySqlDataReader reader = find.ExecuteReader();
            string com = null;
            while ( reader.Read() ) com = reader.GetString(0);
            reader.Close();
            if ( input_string != "" && com != input_string )
            {
                MySqlCommand insert = new MySqlCommand(query, conn);
                insert.ExecuteNonQuery();
            }else
            {
                MessageBox.Show("Same expression already exist");
            }
            reader.Close();
            conn.Close();

            input_string = "";
            pre = "";
            post = "";
            s = "";
            result = "";

        }

        private string toPrefix(String input)
        {

            operators.Clear();
            operands.Clear();

            int i = 0;
            char c, t;
            string opa, opb, tem, cc, op;
            
            while ( i < input.Length )
            {
                c = input[i];
                cc = input.Substring(i,1) ;
                if ( c != '+' && c != '-' && c != '/' && c != '*')
                {
                    int shift = 0;
                    while ( i+shift < input.Length)
                    {
                        t = input[i + shift];
                        if (t != '+' && t != '-' && t != '*' && t != '/')
                        {
                            shift++;
                        }
                        else break;
                    }
                    tem = input.Substring(i, shift);
                    operands.Push(tem);
                    i += shift - 1;
                }
                else if ( c == '+' || c == '-')
                {
                    if (operators.Count() == 0) { }
                    else if (operators.Peek() == "*" || operators.Peek() == "/")
                    {
                        op = operators.Pop();
                        opb = operands.Pop();
                        opa = operands.Pop();
                        tem = " " + op + " " + opa + " " + opb;
                        operands.Push(tem);
                        if (operators.Count() != 0 && (operators.Peek() == "+" || operators.Peek() == "-") )
                        {
                            op = operators.Pop();
                            opb = operands.Pop();
                            opa = operands.Pop();
                            tem = " " + op + " " + opa + " " + opb;
                            operands.Push(tem);
                        }
                    }
                    else if (operators.Peek() == "+" || operators.Peek() == "-")
                    {
                        op = operators.Pop();
                        opb = operands.Pop();
                        opa = operands.Pop();
                        tem = " " + op + " " + opa + " " + opb;
                        operands.Push(tem);
                    }
                    operators.Push(cc);
                }
                else if ( c == '*' || c == '/' )
                {
                    if (operators.Count() == 0) { }
                    else if (operators.Peek() == "*" || operators.Peek() == "/" )
                    {
                        op = operators.Pop();
                        opb = operands.Pop();
                        opa = operands.Pop();
                        tem = " " + op + " " + opa + " " + opb;
                        operands.Push(tem);
                    }
                    operators.Push(cc);
                }
                i++;
            }

            while (operators.Count() > 0)
            {
                op = operators.Pop();
                opb = operands.Pop();
                opa = operands.Pop();
                tem = " " + op + " " + opa + " " + opb;
                operands.Push(tem);
            }

            return operands.Peek();
        }

        private string toPostfix(String input)
        {

            operators.Clear();
            operands.Clear();

            int i = 0;
            char c, t;
            string opa, opb, tem, cc, op;

            while (i < input.Length)
            {
                c = input[i];
                cc = input.Substring(i, 1);
                if (c != '+' && c != '-' && c != '/' && c != '*')
                {
                    int shift = 0;
                    while (i + shift < input.Length)
                    {
                        t = input[i + shift];
                        if (t != '+' && t != '-' && t != '*' && t != '/')
                        {
                            shift++;
                        }
                        else break;
                    }
                    tem = input.Substring(i, shift);
                    operands.Push(tem);
                    i += shift - 1;
                }
                else if (c == '+' || c == '-')
                {
                    if (operators.Count() == 0) { }
                    else if (operators.Peek() == "*" || operators.Peek() == "/")
                    {
                        op = operators.Pop();
                        opb = operands.Pop();
                        opa = operands.Pop();
                        tem = opa + " " + opb + " " + op + " ";
                        operands.Push(tem);
                        if ( operators.Count() != 0 && (operators.Peek() == "+" || operators.Peek() == "-") )
                        {
                            op = operators.Pop();
                            opb = operands.Pop();
                            opa = operands.Pop();
                            tem = opa + " " + opb + " " + op + " ";
                            operands.Push(tem);
                        }
                    }
                    else if (operators.Peek() == "+" || operators.Peek() == "-")
                    {
                        op = operators.Pop();
                        opb = operands.Pop();
                        opa = operands.Pop();
                        tem = opa + " " + opb + " " + op + " ";
                        operands.Push(tem);
                    }
                    operators.Push(cc);
                }
                else if (c == '*' || c == '/')
                {
                    if (operators.Count() <= 0) { }
                    else if (operators.Peek() == "*" || operators.Peek() == "/")
                    {
                        op = operators.Pop();
                        opb = operands.Pop();
                        opa = operands.Pop();
                        tem = opa + " " + opb + " " + op + " ";
                        operands.Push(tem);
                    }
                    operators.Push(cc);
                }
                i++;
            }

            while (operators.Count() != 0 )
            {
                op = operators.Pop();
                opb = operands.Pop();
                opa = operands.Pop();
                tem = opa + " " + opb + " " + op + " ";
                operands.Push(tem);
            }

            return operands.Peek();
        }

        private void Goto_data_Click(object sender, RoutedEventArgs e)
        {
            Window1 nextpage = new Window1();
            this.Close();
            nextpage.Show();
        }


    }
}
