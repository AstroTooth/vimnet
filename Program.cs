using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace winvim_меньше_говнокода
{
    internal class Program
    {
        static bool runing = true;

        static List<string> text = [" "];
        static int column = 0;
        static int line = 0;
        static int max_num = 1;
        static int top_string = 0;

        static string file_opened = "";

        static int width = Console.WindowWidth;
        static int height = Console.WindowHeight;

        static char mode = 'i';

        //это самый главный метод, он центр вселенной
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            DrawAll();

            while (runing == true)
            {
                if (height != Console.WindowHeight || width != Console.WindowWidth)
                {
                    width = Console.WindowWidth;
                    height = Console.WindowHeight;
                }

                ConsoleKeyInfo user_input = Console.ReadKey(true);

                if (mode == 'i')
                {
                    Insert(user_input);
                }
                else if (mode == 'v')
                {
                    Visual(user_input);
                }

                DrawAll();
            }
        }

        //тут обрабатываются клавиши в режиме Insert
        static void Insert(ConsoleKeyInfo user_input)
        {
            if (user_input.Key == ConsoleKey.Escape)//обработчик эскейп
            {
                mode = 'v';
            }
            else if (user_input.Key == ConsoleKey.Enter)//обработчик энтэр
            {
                text.Add(" ");
                for (int i = text.Count() - 2; i > -1; i--)
                {
                    if (i > line)
                    {
                        text[i + 1] = text[i];
                    }
                    else if (i == line)
                    {
                        string text2 = "";
                        for (int j = 0; j < text[line].Length; j++)
                        {
                            if (j < column)
                            {
                                text2 += text[line][j];
                            }
                            else if (j == column)
                            {
                                text[line + 1] = "";
                                text[line + 1] += text[line][j];
                            }
                            else
                            {
                                text[line + 1] += text[line][j];
                            }
                        }

                        text[line] = text2 + ' ';
                    }
                }
                if (text.Count.ToString().Length > max_num)
                {
                    max_num++;
                }
                if (line == top_string + height - 3)
                {
                    top_string++;
                }

                line++;
                column = 0;
            }
            else if (user_input.Key == ConsoleKey.Backspace)//обработчик для Бэкспэйс
            {
                if (column != 0)
                {
                    string text2 = "";
                    for (int i = 0; i < text[line].Length; i++)
                    {
                        if (i != column - 1)
                        {
                            text2 += text[line][i];
                        }
                    }
                    text[line] = text2;
                    column--;
                }
                else if (line != 0)
                {
                    for (int i = 0; i < text.Count; i++)
                    {
                        if (i == line)
                        {
                            string text2 = "";
                            for (int j = 0; j < text[i - 1].Length - 1; j++)
                            {
                                text2 += text[i - 1][j];
                            }
                            text[i - 1] = text2;
                            column = text[i - 1].Length;
                            text[i - 1] += text[i];
                        }
                        if (i > line)
                        {
                            text[i - 1] = text[i];
                        }
                    }
                    if (top_string == line)
                    {
                        top_string--;
                    }

                    line--;
                    text[text.Count - 1] = "";

                    text.RemoveAt(text.Count - 1);

                    if (text.Count.ToString().Length < max_num)
                    {
                        max_num--;
                    }
                }
            }
            else if (user_input.Key == ConsoleKey.Tab)//обработчик табуляции
            {
                string update_text = "";
                for (int i = 0; i < text[line].Length + 4; i++)
                {
                    if (i < column)
                    {
                        update_text += text[line][i];
                    }
                    else if (i == column || i == column + 1 || i == column + 2 || i == column + 3)
                    {
                        update_text += ' ';
                    }
                    else
                    {
                        update_text += text[line][i - 4];
                    }
                }

                text[line] = update_text;
                column += 4;
            }
            //дальше идёт "далина скобок"
            else if (user_input.KeyChar == '(')//обработчик круглых скобок
            {
                string update_text = "";
                for (int i = 0; i < text[line].Length + 2; i++)
                {
                    if (i < column)
                    {
                        update_text += text[line][i];
                    }
                    else if (i == column)
                    {
                        update_text += '(';
                    }
                    else if (i == column + 1)
                    {
                        update_text += ')';
                    }
                    else if (i > column + 1)
                    {
                        update_text += text[line][i - 2];
                    }
                }
                text[line] = update_text;
                column++;
            }
            else if (user_input.KeyChar == '[')//обработчик квадратных скобок
            {
                string update_text = "";
                for (int i = 0; i < text[line].Length + 2; i++)
                {
                    if (i < column)
                    {
                        update_text += text[line][i];
                    }
                    else if (i == column)
                    {
                        update_text += '[';
                    }
                    else if (i == column + 1)
                    {
                        update_text += ']';
                    }
                    else if (i > column + 1)
                    {
                        update_text += text[line][i - 2];
                    }
                }
                text[line] = update_text;
                column++;
            }
            else if (user_input.KeyChar == '{')//обработчик фигурных скобок
            {
                string update_text = "";
                for (int i = 0; i < text[line].Length + 2; i++)
                {
                    if (i < column)
                    {
                        update_text += text[line][i];
                    }
                    else if (i == column)
                    {
                        update_text += '{';
                    }
                    else if (i == column + 1)
                    {
                        update_text += '}';
                    }
                    else if (i > column + 1)
                    {
                        update_text += text[line][i - 2];
                    }
                }
                text[line] = update_text;
                column++;
            }
            else if (user_input.KeyChar == '<')//обработчик угловых скобок
            {
                string update_text = "";
                for (int i = 0; i < text[line].Length + 2; i++)
                {
                    if (i < column)
                    {
                        update_text += text[line][i];
                    }
                    else if (i == column)
                    {
                        update_text += '<';
                    }
                    else if (i == column + 1)
                    {
                        update_text += '>';
                    }
                    else if (i > column + 1)
                    {
                        update_text += text[line][i - 2];
                    }
                }
                text[line] = update_text;
                column++;
            }
            //конец "далины скобок"
            else//обработчик всего остального
            {
                string update_text = "";
                for (int i = 0; i < text[line].Length + 1; i++)
                {
                    if (i < column)
                    {
                        update_text += text[line][i];
                    }
                    else if (i == column)
                    {
                        string up_down_text = "";

                        up_down_text = user_input.KeyChar.ToString();

                        if (up_down_text.Length > 1 || up_down_text == "і" || up_down_text == "І")
                        {
                            up_down_text = up_down_text.ToLower();
                            if (up_down_text[0] == 'd')//тут обрабатываются цифры
                            {
                                update_text += up_down_text[1];
                            }
                            else if (up_down_text == "spacebar")//тут обрабатывается пробел
                            {
                                update_text += ' ';
                            }
                            else if (up_down_text == "oemcomma")//тут обрабатывается запятой
                            {
                                update_text += ',';
                            }
                            else if (up_down_text == "oemperiod")//тут обрабатывается точки
                            {
                                update_text += '.';
                            }
                            else if (up_down_text == "oemplus")//тут обрабатывается равно
                            {
                                update_text += '=';
                            }
                            else if (up_down_text == "oemminus")//тут обрабатывается минус
                            {
                                update_text += '-';
                            }
                            else if (up_down_text == "і")//тут обрабатывается Українська "і"
                            {
                                if (Console.CapsLock == true)
                                    update_text += 'I';
                                else
                                {
                                    update_text += 'i';
                                }
                            }
                            else
                                column--;
                        }
                        else
                            update_text += up_down_text[0];
                    }
                    else
                    {
                        update_text += text[line][i - 1];
                    }
                }
                text[line] = update_text;
                column++;
            }
        }

        //тут обрабатываются клавиши в режиме Visual
        static void Visual(ConsoleKeyInfo user_input)
        {
            if (user_input.Key == ConsoleKey.I)//это для перехода в режим Insert
            {
                mode = 'i';
            }
            else if (user_input.Key == ConsoleKey.H)//это для перемещения курсора влево
            {
                if (column != 0)
                {
                    column--;
                }
                else if (line != 0)
                {
                    line--;
                    column = text[line].Length - 1;
                    if (line + 1 == top_string)
                    {
                        top_string--;
                    }
                }
            }
            else if (user_input.Key == ConsoleKey.J)//это для перемещения курсора вверх
            {
                if (line != 0)
                {
                    line--;
                    if (column > text[line].Length - 1)
                    {
                        column = text[line].Length - 1;
                    }
                    if (line + 1 == top_string)
                    {
                        top_string--;
                    }
                }
            }
            else if (user_input.Key == ConsoleKey.K)//это для перемещения курсора вниз
            {
                if (line != text.Count - 1)
                {
                    line++;
                    if (column > text[line].Length - 1)
                    {
                        column = text[line].Length - 1;
                    }
                    if (line - 1 == top_string + height - 3)
                    {
                        top_string++;
                    }
                }
            }
            else if (user_input.Key == ConsoleKey.L)//это для перемещения курсора впрво
            {
                if (text[line].Count() - 1 != column)
                {
                    column++;
                }
                else if (line != text.Count - 1)
                {
                    line++;
                    column = 0;
                    if (line - 1 == top_string + height - 3)
                    {
                        top_string++;
                    }
                }
            }
            else if (user_input.Key == ConsoleKey.D)//это для создания файла
            {
                Console.CursorVisible = true;
                Console.Write("название файла: ");
                string file_name = Console.ReadLine();
                Console.CursorVisible = false;
                StreamWriter file = new StreamWriter(file_name);
                text.Clear();
                text.Add(" ");
                column = 0;
                line = 0;
                file.Close();

                Console.Clear();
                file_opened = file_name;
            }
            else if (user_input.Key == ConsoleKey.S)
            {
                if (file_opened != "")
                {
                    StreamWriter file = new StreamWriter(file_opened);

                    for (int i = 0; i < text.Count; i++)
                    {
                        file.WriteLine(text[i]);
                    }

                    file.Close();
                }
            }
            else if (user_input.Key == ConsoleKey.F)
            {
                Console.CursorVisible = true;
                Console.Write("название файла: ");
                string file_name = Console.ReadLine();
                Console.CursorVisible = false;
                file_opened = file_name;

                text.Clear();
                column = 0;
                line = 0;

                StreamReader file = new StreamReader(file_name);

                string read_line = file.ReadLine();

                while (read_line != null)
                {
                    text.Add(read_line);

                    read_line = file.ReadLine();
                }

                file.Close();
                Console.Clear();
            }
        }

        //этот метод всё рисует
        static void DrawAll()
        {
            Console.SetCursorPosition(0, 0);
            string space = "";
            string space2 = "";
            string space3 = "";
            for (int i = 0; i < width - 1; i++)
            {
                space2 += ' ';
            }
            for (int i = top_string; i < top_string + height - 2; i++)
            {
                if (text.Count > i)
                {
                    int i2 = max_num - (i + 1).ToString().Length;
                    space = "";
                    for (int j = 0; j < i2; j++)
                    {
                        space += ' ';
                    }

                    Console.Write($"{i + 1}{space}|");
                    for (int j = 0; j < width - max_num - 1; j++)
                    {
                        if (text[i].Length > j) 
                        {
                            if (line == i)
                            {
                                if (column == j)
                                {
                                    Console.BackgroundColor = ConsoleColor.Gray;
                                    Console.ForegroundColor = ConsoleColor.Black;
                                }

                                Console.Write($"{text[i][j]}");

                                Console.ResetColor();
                            }
                            else
                            {
                                Console.Write($"{text[i]}");
                                j = text[i].Length;
                            }
                        }
                        else
                        {
                            space3 += ' ';
                        }
                    }
                }
                else
                {
                    Console.Write($"~{space2}");
                }
                Console.Write($"{space3}");
                space3 = "";
                Console.Write("\n");
            }

            Console.Write("MODE: ");
            if (mode == 'i')
            {
                Console.Write("Insert ");
            }
            else if (mode == 'v')
            {
                Console.Write("Visible ");
            }
        }
    }
}
