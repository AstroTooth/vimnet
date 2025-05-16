using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

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
        static bool quotes = false;
        static int comment = 0;
        static string comment_words = "";
        static int comment_timer = 0;
        static int left_char = 0;
        static bool printing = true;
        static bool printing_end_word = true;
        static bool first_char = true;
        static int first_char_num;
        static List<string> text2 = [" "];

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
                printing_end_word = true;
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
                comment = 0;
                comment_timer = 0;
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
                left_char = 0;
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
                    if (column >= width - max_num - 5)
                    {
                        left_char--;
                    }
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

                    if (column >= width - max_num - 2) 
                    {
                        left_char = column - (width - max_num - 2);
                    }
                    else
                    {
                        left_char = 0;
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

                if (width - max_num - 2 <= column)
                {
                    left_char += 4;
                }
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

                if (column >= width - max_num - 2)
                {
                    left_char += 2;
                }
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

                if (column >= width - max_num - 2)
                {
                    left_char += 2;
                }
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

                if (column >= width - max_num - 2)
                {
                    left_char += 2;
                }
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

                if (column >= width - max_num - 2)
                {
                    left_char++;
                }
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
                    if (column == left_char && column != 0)
                    {
                        left_char--;
                    }
                }
                else if (line != 0)
                {
                    line--;
                    column = text[line].Length - 1;
                    if (line + 1 == top_string)
                    {
                        top_string--;
                    }
                    if (column > width - max_num - 2)
                    {
                        left_char = column - (width - max_num - 2);
                    }
                    else
                    {
                        left_char = 0;
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
                    if (column > width - max_num - 2)
                    {
                        left_char = column - (width - max_num - 2);
                    }
                    else
                    {
                        left_char = 0;
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
                    if (column > width - max_num - 2)
                    {
                        left_char = column - (width - max_num - 2);
                    }
                    else
                    {
                        left_char = 0;
                    }
                }
            }
            else if (user_input.Key == ConsoleKey.L)//это для перемещения курсора впрво
            {
                if (text[line].Count() - 1 != column)
                {
                    column++;
                    if (column == left_char + (width - max_num - 2))
                    {
                        left_char++;
                    }
                }
                else if (line != text.Count - 1)
                {
                    line++;
                    column = 0;
                    if (line - 1 == top_string + height - 3)
                    {
                        top_string++;
                    }
                    left_char = 0;
                }
            }
            else if (user_input.Key == ConsoleKey.E)//это для создания файла
            {
                Console.CursorVisible = true;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("create file: ");
                Console.ResetColor();
                string file_name = Console.ReadLine();
                if (file_name != "none") {
                    StreamWriter file = new StreamWriter(file_name);
                    text.Clear();
                    text.Add(" ");
                    column = 0;
                    line = 0;
                    file.Close();

                    file_opened = file_name;
                }
                Console.CursorVisible = false;
                Console.Clear();
            }
            else if (user_input.Key == ConsoleKey.W) //сохранение файла
            {
                if (file_opened != "")
                {
                    StreamWriter file = new StreamWriter(file_opened);

                    for (int i = 0; i < text.Count; i++)
                    {
                        file.WriteLine(text[i]);
                    }

                    file.Close();

                    text2 = text;
                }
            }
            else if (user_input.Key == ConsoleKey.R) //это для открытия файла
            {
                Console.CursorVisible = true;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("open file: ");
                Console.ResetColor();
                string file_name = Console.ReadLine();
                if (file_name != "none") {
                    file_opened = file_name;

                    text.Clear();
                    column = 0;
                    line = 0;

                    StreamReader file = new StreamReader(file_name);

                    string read_line = file.ReadLine();

                    while (read_line != null)
                    {
                        text.Add(read_line);

                        if (text[text.Count - 1][read_line.Length - 1] != ' ')
                        {
                            text[text.Count - 1] += ' ';
                        }

                        read_line = file.ReadLine();
                    }

                    file.Close();
                }
                Console.CursorVisible = false;
                Console.Clear();
            }
            else if (user_input.Key == ConsoleKey.Q)
            {
                if (text != text2)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("save file? (y\\n): ");
                    Console.ResetColor();
                    Console.CursorVisible = true;
                    string y_n = Console.ReadLine();
                    Console.CursorVisible = false;

                    if (y_n == "y")
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
                }

                runing = false;
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

            string space_line = " ";
            if (text[num_line_i].Length + max_num + 2 < width)
            {
                space_line = new string(' ', width - text[num_line_i].Length - max_num - 2);
            }

            //тут рисуется строчка
            var words = new StringBuilder();
            string word = "";

            for (int i = 0; i < text[num_line_i].Length; i++)
            {
                if (line == num_line_i) 
                {
                    if (i >= left_char && i <= left_char + width - max_num - 4)
                    {
                        printing = true;
                    }
                    else
                    {
                        printing = false;
                    }
                }
                else
                {
                    if (i >= width - max_num - 3)
                    {
                        printing = false;
                    }
                    else
                    {
                        printing = true;
                    }
                }
                if (comment == 0)
                {
                    if (SeparatorChar(text[num_line_i][i]) == true)
                    {
                        first_char = true;
                        word = "";
                        if (i == 0)
                        {
                            Quotes(text[num_line_i][i], text[num_line_i][i], true);
                        }
                        else
                        {
                            Quotes(text[num_line_i][i], text[num_line_i][i - 1], false);
                        }

                        if (quotes == true)
                        {
                            if (text[num_line_i].Length >= 3 && i <= text[num_line_i].Length - 3 && text[num_line_i].Substring(i, 3) == "'''" && comment_timer == 0)
                            {
                                comment = 2;
                                comment_timer = 6;
                                if (num_line_i == line && column == i)
                                {
                                    words.Append("\x1b[0;30;47m");
                                    words.Append(text[num_line_i][i]);
                                    words.Append("\x1b[0;37;40m");
                                }
                                else if (printing == true)
                                {
                                    words.Append("\x1b[1;33m");
                                    words.Append(text[num_line_i][i]);
                                }
                            }
                            else
                            {
                                if (num_line_i == line && column == i)
                                {
                                    words.Append("\x1b[0;30;47m");
                                    words.Append(text[num_line_i][i]);
                                    words.Append("\x1b[0;37;40m");
                                }
                                else if(printing == true)
                                {
                                    words.Append("\x1b[1;32m");
                                    words.Append(text[num_line_i][i]);
                                }
                            }
                        }
                        else
                        {
                            if (text[num_line_i][i] == '"' && i != 1 || text[num_line_i][i] == '\'' && i != 1)
                            {
                                if (text[num_line_i][i - 1] != '\\')
                                {
                                    if (num_line_i == line && column == i)
                                    {
                                        words.Append("\x1b[0;30;47m");
                                    }
                                    else
                                    {
                                        words.Append("\x1b[1;32m");
                                    }
                                }
                                else
                                {
                                    if (num_line_i == line && column == i)
                                    {
                                        words.Append("\x1b[0;30;47m");
                                    }
                                    else
                                    {
                                        words.Append("\x1b[1;37m");
                                    }
                                }
                            }
                            else if (text[num_line_i][i] == '#' && lang == "py")
                            {
                                comment = 1;
                                if (num_line_i == line && column == i)
                                {
                                    words.Append("\x1b[0;30;47m");
                                }
                                else
                                {
                                    words.Append("\x1b[1;33m");
                                }
                            }
                            else
                            {
                                if (num_line_i == line && column == i)
                                {
                                    words.Append("\x1b[0;30;47m");
                                }
                                else
                                {
                                    words.Append("\x1b[1;37m");
                                }
                            }
                            if (printing == true)
                            {
                                words.Append(text[num_line_i][i]);
                            }
                            words.Append("\x1b[1;37;40m");
                        }
                    }
                    else
                    {
                        if (quotes == true)
                        {
                            if (num_line_i == line && column == i)
                            {
                                words.Append("\x1b[0;30;47m");
                                words.Append(text[num_line_i][i]);
                                words.Append("\x1b[0;37;40m");
                            }
                            else if (printing == true)
                            {
                                words.Append("\x1b[1;32m");
                                words.Append(text[num_line_i][i]);
                            }
                        }
                        else
                        {
                            if (first_char == true)
                            {
                                first_char = false;
                                first_char_num = i;
                                for (int j = i; j < text[num_line_i].Length; j++)
                                {
                                    if (SeparatorChar(text[num_line_i][j]) == false)
                                    {
                                        word += text[num_line_i][j];
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            if (IsWordColored(word, text[num_line_i][first_char_num + word.Length]) == 0)
                            {
                                if (num_line_i == line && column == i)
                                {
                                    words.Append("\x1b[0;30;47m");
                                    words.Append(text[num_line_i][i]);
                                    words.Append("\x1b[0;37;40m");
                                }
                                else if (printing == true)
                                {
                                    words.Append("\x1b[1;37m");
                                    words.Append(text[num_line_i][i]);
                                }
                            }
                            else if (IsWordColored(word, text[num_line_i][first_char_num + word.Length]) == 2)
                            {
                                if (num_line_i == line && column == i)
                                {
                                    words.Append("\x1b[0;30;47m");
                                    words.Append(text[num_line_i][i]);
                                    words.Append("\x1b[0;37;40m");
                                }
                                else if (printing == true)
                                {
                                    words.Append("\x1b[1;34m");
                                    words.Append(text[num_line_i][i]);
                                }
                            }
                            else if (IsWordColored(word, text[num_line_i][first_char_num + word.Length]) == 1)
                            {
                                if (num_line_i == line && column == i)
                                {
                                    words.Append("\x1b[0;30;47m");
                                    words.Append(text[num_line_i][i]);
                                    words.Append("\x1b[0;37;40m");
                                }
                                else if (printing == true)
                                {
                                    words.Append("\x1b[1;31m");
                                    words.Append(text[num_line_i][i]);
                                }
                            }
                        }
                    }

                    if (comment_timer != 0)
                    {
                        comment_timer--;
                    }
                }
                else if (comment == 1)
                {
                    if (num_line_i == line && column == i)
                    {
                        words.Append("\x1b[0;30;47m");
                        words.Append(text[num_line_i][i]);
                        words.Append("\x1b[0;37;40m");
                    }
                    else if (printing == true)
                    {
                        words.Append("\x1b[0;33m");
                        words.Append(text[num_line_i][i]);
                    }
                }
                else if (comment == 2)
                {
                    if (num_line_i == line && column == i)
                    {
                        words.Append("\x1b[0;30;47m");
                        words.Append(text[num_line_i][i]);
                        words.Append("\x1b[0;37;40m");
                    }
                    else if (printing == true)
                    {
                        words.Append("\x1b[1;33m");
                        words.Append(text[num_line_i][i]);
                    }

                    if (comment_timer != 0)
                    {
                        comment_timer--;
                    }

                    if (text[num_line_i].Length >= 3 && i <= text[num_line_i].Length - 3)
                    {
                        if (text[num_line_i].Substring(i, 3) == "'''" && comment_timer == 0)
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                if (num_line_i == line && column == i + 1 + j)
                                {
                                    words.Append("\x1b[0;30;47m");
                                    words.Append("'");
                                    words.Append("\x1b[0;37;40m");
                                }
                                else if (printing == true)
                                {
                                    words.Append("\x1b[1;33m");
                                    words.Append("'");
                                }
                            }
                            i += 2;
                            comment = 0;
                        }
                    }
                }
            }

            Console.Write($"{words.ToString()}{space_line}\n");

            if (comment == 1)
            {
                comment = 0;
            }
            quotes = false;
        }

        //этот метод рисует пустую строчку
        static void DrawVoidLine(string space)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"~{space}");
            Console.ResetColor();
        }

        //этот метод решает цвет подсветки
        static int IsWordColored(string word, char select_char)
        {
            if (lang == "py")
            {
                if (select_char == '(')
                {
                    return 2;
                }
                for (int i = 0; i < key_words.python.Length; i++)
                {
                    if (word == key_words.python[i])
                    {
                        return 1;
                    }
                }
            }

            return 0;
        }

        //этот метод отображает полезную информацию
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

        //этот метод говорит является ли выбраный символ частью слова
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

        //метод подсветки кавычек
        static void Quotes(char char1, char char2, bool first_char)
        {
            if (char1 == '"' || char1 == '\'')
            {
                if (first_char == true)
                {
                    if (quotes == true)
                    {
                        quotes = false;
                    }
                    else
                    {
                        quotes = true;
                    }
                }
                else
                {
                    if (char2 != '\\')
                    {
                        if (quotes == true)
                        {
                            quotes = false;
                        }
                        else
                        {
                            quotes = true;
                        }
                    }
                }
            }
        }
    }
}
