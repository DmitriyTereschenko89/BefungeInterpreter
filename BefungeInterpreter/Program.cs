using System.Text;
using System.Text.RegularExpressions;

namespace BefungeInterpreter
{
    internal class Program
    {
        #region Befunge Interpreter
        public class BefungeInterpreter
        {
            private enum Direction
            {
                RIGHT,
                LEFT,
                DOWN,
                UP
            }

            private class Befunge
            {
                private const int StartXPosition = 0;
                private const int StartYPosition = 0;
                private const Direction DefaultDirection = Direction.RIGHT;

                private readonly Stack<int> st;
                private readonly char[][] code;
                private readonly Direction[] directions;
                private readonly Random randomDirection;

                private Direction direction;
                private bool isStringMode;
                private bool isTrampoline;
                private int x;
                private int y;

                public Befunge(string code)
                {
                    x = StartXPosition;
                    y = StartYPosition;
                    isTrampoline = false;
                    isStringMode = false;
                    randomDirection = new Random();
                    st = new Stack<int>();
                    directions = new Direction[] { Direction.RIGHT, Direction.LEFT, Direction.DOWN, Direction.UP };
                    direction = DefaultDirection;
                    string[] codeLines = Regex.Split(code, @"(?:\r\n)|(?:\r)|(?:\n)");
                    this.code = new char[codeLines.Length][];
                    for (int i = 0; i < codeLines.Length; ++i)
                    {
                        this.code[i] = codeLines[i].ToCharArray();
                    }
                }

                private void Additional()
                {
                    int a = st.Pop();
                    int b = st.Pop();
                    st.Push(a + b);
                }

                private void Subtraction()
                {
                    int a = st.Pop();
                    int b = st.Pop();
                    st.Push(b - a);
                }

                private void Multiplication()
                {
                    int a = st.Pop();
                    int b = st.Pop();
                    st.Push(a * b);
                }

                private void Division()
                {
                    int a = st.Pop();
                    int b = st.Pop();
                    st.Push(a == 0 ? 0 : b / a);
                }

                private void Modulo()
                {
                    int a = st.Pop();
                    int b = st.Pop();
                    st.Push(a == 0 ? 0 : b % a);
                }

                private void LogicalNot()
                {
                    st.Push(st.Pop() == 0 ? 1 : 0);
                }

                private void Backtrick()
                {
                    int a = st.Pop();
                    int b = st.Pop();
                    st.Push(b > a ? 1 : 0);
                }

                private void Right()
                {
                    y = y == code[x].Length - 1 ? 0 : y + 1;
                }

                private void Left()
                {
                    y = y == 0 ? code[x].Length - 1 : y - 1;
                }

                private void Down()
                {
                    x = x == code.Length - 1 ? 0 : x + 1;
                }

                private void Up()
                {
                    x = x == 0 ? code.Length - 1 : x - 1;
                }

                private void RandomDirection()
                {
                    direction = directions[randomDirection.Next(directions.Length)];
                }

                private void RightOrLeft()
                {
                    if (st.Pop() == 0)
                    {
                        direction = Direction.RIGHT;
                    }
                    else
                    {
                        direction = Direction.LEFT;
                    }
                }

                private void DownOrUp()
                {
                    if (st.Pop() == 0)
                    {
                        direction = Direction.DOWN;
                    }
                    else
                    {
                        direction = Direction.UP;
                    }
                }

                private void StringMode()
                {
                    isStringMode = !isStringMode;
                }

                private void Duplicate()
                {
                    st.Push(st.Count > 0 ? st.Peek() : 0);
                }

                private void Swap()
                {
                    if (st.Count > 1)
                    {
                        int a = st.Pop();
                        int b = st.Pop();
                        st.Push(a);
                        st.Push(b);
                    }
                    else
                    {
                        int a = st.Pop();
                        st.Push(0);
                        st.Push(a);
                    }
                }

                private void DiscardPop()
                {
                    st.Pop();
                }

                private int Pop()
                {
                    return st.Pop();
                }

                private char PopASCII()
                {
                    return (char)st.Pop();
                }

                private void Put()
                {
                    int y = st.Pop();
                    int x = st.Pop();
                    int v = st.Pop();
                    code[y][x] = (char)v;
                }

                private void Get()
                {
                    int y = st.Pop();
                    int x = st.Pop();
                    st.Push(code[y][x]);
                }

                private void Push(char val)
                {
                    st.Push(isStringMode ? val : val - '0');
                }

                private void Move()
                {
                    switch (direction)
                    {
                        case Direction.RIGHT:
                            Right();
                            break;
                        case Direction.LEFT:
                            Left();
                            break;
                        case Direction.DOWN:
                            Down();
                            break;
                        case Direction.UP:
                            Up();
                            break;
                        default:
                            break;
                    }
                }

                public string Start()
                {
                    StringBuilder output = new();
                    while (code[x][y] != '@')
                    {
                        if (isTrampoline)
                        {
                            isTrampoline = false;
                        }
                        else
                        {
                            switch (code[x][y])
                            {
                                case '+':
                                    Additional();
                                    break;
                                case '-':
                                    Subtraction();
                                    break;
                                case '*':
                                    Multiplication();
                                    break;
                                case '/':
                                    Division();
                                    break;
                                case '%':
                                    Modulo();
                                    break;
                                case '!':
                                    if (isStringMode)
                                    {
                                        st.Push(code[x][y]);
                                    }
                                    else
                                    {
                                        LogicalNot();
                                    }
                                    break;
                                case '`':
                                    Backtrick();
                                    break;
                                case '>':
                                    direction = Direction.RIGHT;
                                    break;
                                case '<':
                                    direction = Direction.LEFT;
                                    break;
                                case 'v':
                                    direction = Direction.DOWN;
                                    break;
                                case '^':
                                    direction = Direction.UP;
                                    break;
                                case '?':
                                    RandomDirection();
                                    break;
                                case '_':
                                    RightOrLeft();
                                    break;
                                case '|':
                                    DownOrUp();
                                    break;
                                case '"':
                                    StringMode();
                                    break;
                                case ':':
                                    Duplicate();
                                    break;
                                case '\\':
                                    Swap();
                                    break;
                                case '$':
                                    DiscardPop();
                                    break;
                                case '.':
                                    output.Append(Pop());
                                    break;
                                case ',':
                                    output.Append(PopASCII());
                                    break;
                                case '#':
                                    isTrampoline = true;
                                    break;
                                case 'p':
                                    Put();
                                    break;
                                case 'g':
                                    Get();
                                    break;
                                case ' ':
                                    if (isStringMode)
                                    {
                                        Push(code[x][y]);
                                    }
                                    break;
                                default:
                                    Push(code[x][y]);
                                    break;
                            }
                        }
                        Move();
                    }
                    return output.ToString();
                }
            }

            public string Interpret(string code)
            {
                Befunge befunge = new(code);
                return befunge.Start();
            }
        }
        #endregion
        static void Main(string[] args)
        {
            BefungeInterpreter befungeInterpreter = new();
            Console.WriteLine(befungeInterpreter.Interpret(">987v>.v\r\nv456<  :\r\n>321 ^ _@"));
            Console.WriteLine(befungeInterpreter.Interpret("08>:1-:v v *_$.@ \r\n  ^    _$>\\:^  ^    _$>\\:^"));
            Console.WriteLine(befungeInterpreter.Interpret(">25*\"!dlroW olleH\":v\r\n                v:,_@\r\n                >  ^"));
            Console.WriteLine(befungeInterpreter.Interpret("01->1# +# :# 0# g# ,# :# 5# 8# *# 4# +# -# _@"));
            Console.WriteLine(befungeInterpreter.Interpret("2>:3g\" \"-!v\\  g30          <\r\n |!`\"&\":+1_:.:03p>03g+:\"&\"`|\r\n @               ^  p3\\\" \":<\r\n2 2345678901234567890123456789012345678"));
        }
    }
}