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
        const string separator_chars = ", ./\\'\"[]{}-=+_!@#$%^&*()№;:<>`~|";
        static bool separator = false;
        static string space = "";

        static string file_opened = "";
        static string lang = "";

        static int width = Console.WindowWidth;
        static int height = Console.WindowHeight;

        static char mode = 'i';

        //это самый главный метод, он центр вселенной
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            for (int i = 0; i < width - 1; i++)
            {
                space += ' ';
            }
            for (int i = 0; i < text.Count; i++)
            {
                DrawLine(i);
            }
            for (int i = 0; i < height - text.Count - 2; i++)
            {
                DrawVoidLine(space);
            }
            DrawInfo();

            while (runing == true)
            {
                lang = "";
                for (int i = 0; i < file_opened.Length; i++)
                {
                    if (file_opened[i] == '.')
                    {
                        lang = "";
                    }
                    else
                    {
                        lang += file_opened[i];
                    }
                }

                if (height != Console.WindowHeight || width != Console.WindowWidth)
                {
                    width = Console.WindowWidth;
                    height = Console.WindowHeight;
                    for (int i = 0; i < width - 1; i++)
                    {
                        space += ' ';
                    }
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
                Console.SetCursorPosition(0, 0);

                int line_num_draw = height - 2;
                for (int i = top_string; i < top_string + height - 2; i++)
                {
                    if (text.Count > i)
                    {
                        DrawLine(i);
                        line_num_draw--;
                    }
                }
                for (int i = 0; i < line_num_draw; i++)
                {
                    DrawVoidLine(space);
                }
                DrawInfo();
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
                Console.Write("создать файл с названием: ");
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
                Console.Write("открыть файл с названием: ");
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

        //этот метод рисует не пустую строчку
        static void DrawLine(int num_line_i)
        {

            //это рисует номер строчки
            string space_num_line = new string(' ', max_num - (num_line_i + 1).ToString().Length);

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"{num_line_i + 1}{space_num_line}|");
            Console.ResetColor();

            string space_line = new string(' ', width - text[num_line_i].Length - max_num - 2);

            //тут рисуется строчка в которой находится курсор
            if (num_line_i == line)
            {
                string word = "";
                List<string> words = [];
                List<int> num_words = [];

                for (int i = 0; i < text[num_line_i].Length; i++)
                {
                    separator = SeparatorChar(text[num_line_i][i]);
                    
                    if (separator == false)
                    {
                        word += text[num_line_i][i];
                    }
                    else
                    {
                        if (word != "")
                        {
                            words.Add(word);
                            num_words.Add(i - word.Length);
                            word = "";
                        }
                    }
                }

                for (int i = 0; i < text[num_line_i].Length; i++)
                {   
                    for (int j = 0; j < words.Count; j++)
                    {
                        if (i >= num_words[j] && i < num_words[j] + words[j].Length)
                        {
                            if (IsWordColored(words[j], text[num_line_i][num_words[j] + words[j].Length]) == 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Gray;
                            }
                            else if (IsWordColored(words[j], text[num_line_i][num_words[j] + words[j].Length]) == 2)
                            {
                                Console.ForegroundColor = ConsoleColor.Blue;
                            }
                            else if (IsWordColored(words[j], text[num_line_i][num_words[j] + words[j].Length]) == 1)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                            }
                        }
                    }
                    if (column == i)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.Gray;
                    }
                    Console.Write($"{text[num_line_i][i]}");

                    Console.ResetColor();
                }
                Console.Write($"{space_line}\n");
            }

            //тут рисуется строчка в которой нету курсора
            else
            {
                string word = "";
                string words = "";
                for (int i = 0; i < text[num_line_i].Length; i++)
                {
                    separator = false;
                    for (int j = 0; j < separator_chars.Length; j++)
                    {
                        if (text[num_line_i][i] == separator_chars[j])
                        {
                            separator = true;
                            break;
                        }
                    }
                    if (separator == true)
                    {
                        if (IsWordColored(word, text[num_line_i][i]) == 0)
                        {
                            words += word + text[num_line_i][i];  
                        }
                        else if (IsWordColored(word, text[num_line_i][i]) == 2)
                        {
                            Console.Write($"{words}");

                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write($"{word}");
                            Console.ResetColor();
                            Console.Write($"{text[num_line_i][i]}");
                            words = "";
                        }
                        else if (IsWordColored(word, text[num_line_i][i]) == 1)
                        {
                            Console.Write($"{words}");

                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write($"{word}");
                            Console.ResetColor();
                            Console.Write($"{text[num_line_i][i]}");
                            words = "";
                        }
                        word = "";
                    }
                    else
                    {
                        word += text[num_line_i][i];
                    }
                }
                Console.Write($"{words}{space_line}\n");
            }
        }

        //этот метод рисует пустую строчку
        static void DrawVoidLine(string space)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"~{space}");
            Console.ResetColor();
        }

        static int IsWordColored(string word, char select_char)
        {
            int true_false = 0;
            if (lang == "py")
            {
                if (select_char == '(')
                {
                    true_false = 2;
                }
                for (int i = 0; i < key_words.python.Length; i++)
                {
                    if (word == key_words.python[i])
                    {
                        true_false = 1;
                        break;
                    }
                }
            }

            return true_false;
        }

        static void DrawInfo()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            if (mode == 'i')
            {
                Console.Write("MODE: Insert|");
            }
            else if (mode == 'v')
            {
                Console.Write("MODE: Visual|");
            }
            Console.ResetColor();
        }

        static bool SeparatorChar(char char_selection)
        {
            for (int i = 0; i < separator_chars.Length; i++)
            {
                if (char_selection == separator_chars[i])
                {
                    return true;
                }
            }

            return false;
        }
    }
}
