using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NumberPlaceMaker
{
    public static class GV//グローバル変数
    {
        public static int[,] NoQ = new int[9, 9];//問題用配列
        public static int[,] NoA = new int[9, 9];//回答用配列
        public static int[,] NoC = new int[9, 9];//正当用配列
        public static int[,] NoO = new int[9, 9];//候補用配列
        public static int[] NFHL = new int[9];//横列フラグ
        public static int[] NFLL = new int[9];//縦列フラグ
        public static int[,] NFBL = new int[3, 3];//ブロックフラグ
        public static int GameLevel;
        public static Random R;
    }

    class Program
    {

        static void Main(string[] args)
        {
            Sub s = new Sub();
            string a;
            Menu();
            NewGame();
            Console.Write("\nEnter(a:Answer):");
            a=Console.ReadLine();
            if (a == "a")
            {
                Array.Copy(GV.NoC, GV.NoA, 81);
                s.PrintNP(); ;
                Console.Write("\nEnter:");
                a = Console.ReadLine();
            }
        }

        static void Menu()
        {
            Console.Write("難易度(１～５)：");
            int n;
            while (true)
            {
                n = int.Parse(Console.ReadLine());
                Console.WriteLine();
                if (1 <= n && n <= 5) { break; }
                Console.Write("ELLER もう一度入力：");
            }
            GV.GameLevel = n;

            Console.Write("シード値：");
            n = int.Parse(Console.ReadLine());
            GV.R = new Random(n);//乱数の生成
        }

        static void NewGame()
        {
            Sub s = new Sub();
            s.DefValue();
            s.LaidNumber();
            s.MakeSpace();
            s.PrintNP();
        }
    }

    class Sub
    {
        internal void DefValue()//数値初期化
        {
            Function f = new Function();
            f.Fill(ref GV.NoQ, 0);
            f.Fill(ref GV.NoA, 0);
            f.Fill(ref GV.NoC, 0);
            f.Fill(ref GV.NoO, 0);
            f.Fill(ref GV.NFHL, 0);
            f.Fill(ref GV.NFLL, 0);
            f.Fill(ref GV.NFBL, 0);
        }

        internal void LaidNumber()//解の敷き詰め
        {
            Function f = new Function();
            int m, n, o, p, x, y;
            int[] a = new int[9];//要素格納
            int b;
            f.LineUp(ref a);
            for (int i0 = 0; i0 < 3; i0++)//中央ブロックの入力
            {
                for (int i1 = 0; i1 < 3; i1++) { GV.NoA[3 + i1, 3 + i0] = a[3 * i0 + i1]; }
            }
            f.LineUp(ref a);
            for (int i = 0; i < 3; i++)
            {
                n = Array.IndexOf(a, GV.NoA[4, 3 + i]);
                f.Swap(ref a[n], ref a[3 + i]);
            }
            for (int i = 0; i < 3; i++)//縦中央の入力
            {
                GV.NoA[4, i] = a[i];
                GV.NoA[4, i + 6] = a[i + 6];
            }
            f.LineUp(ref a);
            for (int i = 0; i < 3; i++)
            {
                n = Array.IndexOf(a, GV.NoA[3 + i, 4]);
                f.Swap(ref a[n], ref a[3 + i]);
            }
            for (int i = 0; i < 3; i++)//横中央の入力
            {
                GV.NoA[i, 4] = a[i];
                GV.NoA[i + 6, 4] = a[i + 6];
            }

            for (int i0 = 0; i0 < 2; i0++)//十字ブロックの入力
            {
                for (int i1 = 0; i1 < 2; i1++)
                {
                    for (int i2 = 0; i2 < 2; i2++)
                    {
                        for (int i3 = 0; i3 < 3; i3++)
                        {
                            x = 4;
                            y = i3 + 6 * (1 - i1);
                            if(i0 == 1){ f.Swap(ref x, ref y); }
                            a[i3] = GV.NoA[x, y];
                        }
                        for (int i3 = 0; i3 < 3; i3++)
                        {
                            x = 3 + 2 * i2;
                            y = i3 + 3;
                            if(i0 == 1){ f.Swap(ref x, ref y); }
                            n = Array.IndexOf(a, GV.NoA[x, y]);
                            if (n != -1) { a[n] = 0; }
                        }
                        m = GV.R.Next(6);
                        for (int i = 0; i < 2; i++) { f.Swap(ref a[i], ref a[i + (m / f.Fact(3 - i - 1)) % (3 - i)]); }
                        for (int i3 = 0; i3 < 3; i3++)
                        {
                            x = 3 + 2 * i2;
                            y = i3 + 6 * i1;
                            if(i0 == 1){ f.Swap(ref x, ref y); }
                            GV.NoA[x, y] = a[i3];
                        }
                    }
                }
            }
            for (int i0 = 0; i0 < 2; i0++)
            {
                for (int i1 = 0; i1 < 3; i1++)
                {
                    x = i1+3;  y = 4;
                    if (i0 == 1) { f.Swap(ref x, ref y); }
                    a[i1] = GV.NoA[x, y];
                }
                m = GV.R.Next(6);
                f.Swap(ref a[0], ref a[m % 3]);
                f.Swap(ref a[1], ref a[1 + (m % 2)]);
                for (int i1 = 0; i1 < 3; i1++)
                {
                    for (int i2 = 0; i2 < 2; i2++)
                    {
                        x = i1 + 6 * i2; y = 3;
                        if (i0 == 1) { f.Swap(ref x, ref y); }
                        if (GV.NoA[x, y] == 0)
                        {
                            for (int i3 = 0; i3 < 3; i3++)
                            {
                                if (a[i3] != 0) { GV.NoA[x, y] = a[i3];  a[i3] = 0;  break;  }
                            }
                        }
                    }
                }
            }
            f.FieldFlag();
            for (int i0 = 0; i0 < 2; i0++)
            {
                for (int i1 = 0; i1 < 3; i1++)
                {
                    x = i1 + 3; y = 4;
                    if (i0 == 1) { f.Swap(ref x, ref y); }
                    a[i1] = GV.NoA[x, y];
                }
                m = GV.R.Next(6);
                f.Swap(ref a[0], ref a[m % 3]);
                f.Swap(ref a[1], ref a[1 + (m % 2)]);
                for (int i1 = 0; i1 < 3; i1++)
                {
                    for (int i2 = 0; i2 < 2; i2++)
                    {
                        x = i1 + 6 * i2; y = 5;
                        if (i0 == 1) { f.Swap(ref x, ref y); }
                        if (GV.NoA[x, y] == 0)
                        {
                            for (int i3 = 0; i3 < 3; i3++)
                            {
                                if (i0 == 0)
                                {
                                    if (a[i3] != 0 && (GV.NFBL[i2 * 2, 1] & (1 << (a[i3] - 1))) == 0)
                                    { GV.NoA[x, y] = a[i3]; a[i3] = 0; break; }
                                }
                                else if (i0 == 1)
                                {
                                    if (a[i3] != 0 && (GV.NFBL[1, i2 * 2] & (1 << (a[i3] - 1))) == 0)
                                    { GV.NoA[x, y] = a[i3]; a[i3] = 0; break; }
                                }
                            }
                        }
                    }
                }
            }

            do
            {
                m = 0;
                f.LineUp(ref a);//左上ブロックの入力
                f.FieldFlag();
                do
                {
                    n = 0;
                    for (int i0 = 0; i0 < 3; i0++)
                    {
                        for (int i1 = 0; i1 < 3; i1++)
                        {
                            b = (1 << (a[3 * i0 + i1] - 1));
                            if ((b & GV.NFHL[i0]) == b || (b & GV.NFLL[i1]) == b)
                            {
                                o = GV.R.Next(8);
                                f.Swap(ref a[3 * i0 + i1], ref a[o < (3 * i0 + i1) ? o : o + 1]);
                                n = 1;
                            }
                        }
                    }
                } while (n == 1);
                for (int i0 = 0; i0 < 3; i0++)
                {
                    for (int i1 = 0; i1 < 3; i1++) { GV.NoA[i1, i0] = a[3 * i0 + i1]; }
                }
                f.FieldFlag();
                for (int i0 = 0; i0 < 2; i0++)
                {
                    for (int i1 = 0; i1 < 3; i1++)
                    {
                        for (int i2 = 0; i2 < 3; i2++)
                        {
                            x = 6 + i2;
                            y = i1;
                            if (i0 == 1) { f.Swap(ref x, ref y); }
                            if ((GV.NFHL[y] | GV.NFLL[x]) == 0b111111111)
                            {
                                m = 1;
                                for (int i3 = 0; i3 < 3; i3++)
                                {
                                    for (int i4 = 0; i4 < 3; i4++) { GV.NoA[i3, i4] = 0; }
                                }
                                f.FieldFlag();
                            }
                        }
                    }
                }
            } while (m == 1);

            f.FieldFlag();//右下ブロックの入力
            int[] d = new int[30];
            int[] flug = new int[18];
            m = 0; n = 0;
            for (int i0 = 0; i0 < 362880;)
            {
                o = 0;
                a[n] = ((i0 / f.Fact(8 - n)) % (9 - n)) + 1;
                p = a[n];
                for (int i = n - 1; 0 <= i; i--) { p += a[i] <= p ? 1 : 0; }
                x = n % 3; y = n / 3;
                if (((GV.NFLL[x + 6] | GV.NFHL[y + 6]) & (1 << (p - 1))) == 0)
                {
                    for (int i1 = 0; i1 < 2; i1++)
                    {
                        for (int i2 = 0; i2 < 3; i2++)
                        {
                            if (i1 == 1 && i2 == y) { continue; }
                            for (int i3 = 0; i3 < 3; i3++)
                            {
                                if (i1 == 0 && i3 == x) { continue; }
                                if (((GV.NFLL[i3 + (1 - i1) * 6] | GV.NFHL[i2 + i1 * 6]) & (1 << (p - 1))) == 0)
                                {
                                    if (Array.IndexOf(flug, i1 * 9 + i2 * 3 + i3) < 0 || n * 2 <= Array.IndexOf(flug, i1 * 9 + i2 * 3 + i3))
                                    {
                                        o |= 1 << i1;
                                        flug[n * 2 + i1] = i1 * 9 + i2 * 3 + i3;
                                    }
                                    i2 = 2; i3 = 2;
                                }
                            }
                        }
                    }
                }
                if (o == 3)
                {
                    if (n == 8)
                    {
                        d[m] = i0;
                        m++; i0++;
                        for (int i = 0; i < 8; i++) { if ((i0 % f.Fact(8 - i)) == 0) { n = i; break; } }
                    }
                    else { n++; }
                }
                else
                {
                    i0 = ((i0 / f.Fact(8 - n)) + 1) * f.Fact(8 - n);
                    for (int i = 0; i < 8; i++) { if ((i0 % f.Fact(8 - i)) == 0) { n = i; break; } }
                }
            }
            m = GV.R.Next(m);
            for (int i0 = 0; i0 < 9; i0++)
            {
                a[i0] = ((d[m] / f.Fact(8 - i0)) % (9 - i0)) + 1;
                n = a[i0];
                for (int i1 = i0 - 1; 0 <= i1; i1--) { n += a[i1] <= n ? 1 : 0; }
                GV.NoA[(i0 % 3) + 6, (i0 / 3) + 6] = n;
            }

            f.FieldFlag();
            for (int i0 = 0; i0 < 9; i0++)//残りの入力
            {
                for (int i1 = 0; i1 < 9; i1++)
                {
                    if (((GV.NFLL[i0 % 3 + 6] | GV.NFHL[i0 / 3]) & (1 << i1)) == 0) { GV.NoA[(i0 % 3) + 6, (i0 / 3)] = i1 + 1; }
                    if (((GV.NFLL[i0 % 3] | GV.NFHL[i0 / 3 + 6]) & (1 << i1)) == 0) { GV.NoA[(i0 % 3), (i0 / 3) + 6] = i1 + 1; }
                }
            }
            
            Array.Copy(GV.NoA, GV.NoC, 81);
            Array.Copy(GV.NoA, GV.NoQ, 81);
        }

        internal void MakeSpace()//数字のブランクの作成
        {
            Function f = new Function();
            int x, y;
            int[] a = new int[41];//判定順番
            for (int i = 0; i < 41; i++) { a[i] = i; }
            for (int i = 0; i < 40; i++) { f.Swap(ref a[i], ref a[i + GV.R.Next(41 - i)]); }
            for (int i = 0; i < 4; i++)
            {
                x = a[i] % 9;  y = a[i] / 9;
                GV.NoQ[x, y] = 0;
                GV.NoQ[8-x, 8-y] = 0;
            }
            for (int i = 3; i < 41; i++)
            {

                x = a[i] % 9; y = a[i] / 9;
                if (GV.R.Next(2) == 1) { x = 8 - x; y = 8 - y; }
                if (Space(x, y))
                {
                    GV.NoQ[x, y] = 0;
                    if (x == 4 && y == 4) { continue; }
                    if (Space(8 - x, 8 - y)) { GV.NoQ[8 - x, 8 - y] = 0; }
                    else { GV.NoQ[x, y] = GV.NoC[x, y]; }
                }
            }

            f.Fill(ref GV.NoA, (0));
        }

        internal bool Space(int x, int y)//ブランク作成条件
        {
            Function f = new Function();
            Array.Copy(GV.NoQ, GV.NoA, 81);

            f.FieldFlag();
            if ((GV.NFLL[x]|GV.NFHL[y]|GV.NFBL[x/3,y/3])==0b111111111) { return true; }

            if (GV.GameLevel < 2) { return false; }//Lv2

            GV.NoA[x, y] = 0;
            f.FieldFlag();
            int a, bx, by;
            for (int i0 = 0; i0 < 9; i0++)//GV.NoO(候補)の入力
            {
                for (int i1 = 0; i1 < 9; i1++)
                {
                    if (GV.NoA[i1,i0]!=0) { GV.NoO[i1, i0] = (1 << (GV.NoA[i1, i0] - 1)); }
                    else
                    {
                        bx = (i1 / 3) * 3;  by = (i0 / 3) * 3;
                        a = 0;
                        for (int i2 = 0; i2 < 9; i2++)
                        {
                            if (GV.NoA[i1, i2] != 0) { a |= (1 << (GV.NoA[i1, i2] - 1)); }
                            if (GV.NoA[i2, i0] != 0) { a |= (1 << (GV.NoA[i2, i0] - 1)); }
                            if (GV.NoA[bx + i2 % 3, by + i2 / 3] != 0)
                            {
                                a |= (1 << (GV.NoA[bx + i2 % 3, by + i2 / 3] - 1));
                            }
                        }
                        GV.NoO[i1, i0] = ((~a) & (0b111111111));
                    }
                }
            }

            a = (1 << (GV.NoQ[x, y] - 1));
            if (GV.NoO[x, y] == a) { return true; }

            if (GV.GameLevel < 3) { return false; }//Lv3

            bool b = true;
            for (int i = 0; i < 9; i++)
            {
                if ((GV.NoO[x, i] & a) != 0)
                {
                    if (i != y) { b = false; break; }
                }
            }
            if (b == true) { return true; }
            b = true;
            for (int i = 0; i < 9; i++)
            {
                if ((GV.NoO[i, y] & a) != 0)
                {
                    if (i != x) { b = false; break; }
                }
            }
            if (b == true) { return true; }
            b = true; bx = x / 3; by = y / 3;
            for (int i0 = 0; i0 < 3; i0++)
            {
                for (int i1 = 0; i1 < 3; i1++)
                {
                    if ((GV.NoO[bx+i1, by+i0] & a) != 0)
                    {
                        if (bx + i1 != x || by + i0 != y) { b = false; break; }
                    }
                }
            }
            if (b == true) { return true; }

            if (GV.GameLevel < 4) { return false; }//Lv4

            int ax, ay, n0 = 0, n1 = 0, d1, d2, d3;
            do
            {
                ax = n0 % 9;
                ay = (n0 / 9) % 9;
                if (f.BitHowMuch(GV.NoO[ax, ay], 9) != 1)
                {
                    bx = (ax / 3) * 3;
                    by = (ay / 3) * 3;
                    for (int i = 0; i < 9; i++)
                    {
                        if (f.BitHowMuch(GV.NoO[ax, i], 9) == 1 &&
                            i != ay &&
                            (GV.NoO[ax, ay] & GV.NoO[ax, i]) != 0)
                        {
                            GV.NoO[ax, ay] &= (~GV.NoO[ax, i]);
                            n1 = n0 % 81;
                        }
                        if (f.BitHowMuch(GV.NoO[i, ay], 9) == 1 &&
                            i != ax &&
                            (GV.NoO[ax, ay] & GV.NoO[i, ay]) != 0)
                        {
                            GV.NoO[ax, ay] &= (~GV.NoO[i, ay]);
                            n1 = n0 % 81;
                        }
                        if (f.BitHowMuch(GV.NoO[bx + i % 3, by + i / 3], 9) == 1 &&
                            (bx + i % 3 != ax || by + i / 3 != ay) &&
                            (GV.NoO[ax, ay] & GV.NoO[bx + i % 3, by + i / 3]) != 0)
                        {
                            GV.NoO[ax, ay] &= (~GV.NoO[bx + i % 3, by + i / 3]);
                            n1 = n0 % 81;
                        }
                    }
                    d1 = 0; d2 = 0; d3 = 0;
                    for (int i = 0; i < 9; i++)
                    {
                        if (i != ax) { d1 |= GV.NoO[i, ay]; }
                        if (i != ay) { d2 |= GV.NoO[ax, i]; }
                        if (i != (ay % 3) * 3 + (ax % 3)) { d3 |= GV.NoO[bx + (i % 3), by + (i / 3)]; }
                        if (f.BitHowMuch(d1,9)==8) { GV.NoO[ax, ay] = ~d1 & 0b111111111; }
                        if (f.BitHowMuch(d2,9)==8) { GV.NoO[ax, ay] = ~d2 & 0b111111111; }
                        if (f.BitHowMuch(d3,9)==8) { GV.NoO[ax, ay] = ~d3 & 0b111111111; }
                    }
                }
                n0++;
            } while (((n0 < 81 * 2) && (GV.GameLevel < 5)) || ((n0 % 81) != n1));

            return false;
        }

        internal void PrintNP()
        {
            int a;
            Function f = new Function();
            string D = "―――――――――――――\n";
            for (int i0 = 0; i0 < 3; i0++)
            {
                for (int i1 = 0; i1 < 3; i1++)
                {
                    for (int i2 = 0; i2 < 3; i2++)
                    {
                        D += "｜";
                        for (int i3 = 0; i3 < 3; i3++)
                        {
                            a = GV.NoQ[i2 * 3 + i3, i0 * 3 + i1];
                            if (1 <= a && a <= 9) { D += f.ZenNo(a); }
                            else { D += f.ZenNo(GV.NoA[i2 * 3 + i3, i0 * 3 + i1]); }
                        }
                    }
                    D += "｜\n";
                }
                D += "―――――――――――――\n";
            }
            Console.Write(D);
        }

    }

    class Function
    {
        internal void Fill<Type>(ref Type[] x, Type a)//配列の定数敷き詰め
        {
            for (int i = 0; i < x.Length; i++) { x[i] = a; }
        }

        internal void Fill<Type>(ref Type[,] x, Type a)//配列の定数敷き詰め
        {
            for (int i0 = 0; i0 < x.GetLength(0); i0++)
            {
                for (int i1 = 0; i1 < x.GetLength(1); i1++) { x[i0, i1] = a; }
            }
        }

        internal int Fact(int x)//階乗関数
        {
            return x >= 1 ? x * Fact(x - 1) : 1;
        }

        internal int BitHowMuch(int x, int a)//ビットフラグtrue量
        {
            int b = 0;
            for (int i = 0; i < a; i++)
            {
                if (((x >> i) & 1) == 1) { b++; }
            }
            return b;
        }

        internal void FieldFlag()//フィールドフラグ操作(右から1,2,3…)
        {
            Fill(ref GV.NFHL, 0);
            Fill(ref GV.NFLL, 0);
            Fill(ref GV.NFBL, 0);
            int a;
            for (int i0 = 0; i0 < 9; i0++)
            {
                for (int i1 = 0; i1 < 9; i1++)
                {
                    if (GV.NoA[i1,i0]!=0)
                    {
                        a = 1 << (GV.NoA[i1, i0] - 1);
                        GV.NFHL[i0] |= a;
                        GV.NFLL[i1] |= a;
                        GV.NFBL[i1 / 3, i0 / 3] |= a;
                    }
                }
            }
        }

        internal string ZenNo(int x)//全角数字変換
        {
            switch (x)
            {
                case 0:
                    return "　";
                case 1:
                    return "１";
                case 2:
                    return "２";
                case 3:
                    return "３";
                case 4:
                    return "４";
                case 5:
                    return "５";
                case 6:
                    return "６";
                case 7:
                    return "７";
                case 8:
                    return "８";
                case 9:
                    return "９";
                default:
                    return x.ToString();
            }
        }

        internal void Swap<Type> (ref Type a,ref Type b)//２項の入れ替え
        {
            Type temp = a;
            a = b;
            b = temp;
        }


        internal void LineUp(ref int[] x)//１からランダムに配列
        {
            int a = x.Length;
            int b;
            for (int i = 0; i < a; i++)//階差数列
            {
                x[i] = i + 1;
            }

            b = GV.R.Next(Fact(a));
            for (int i = 0; i < a-1; i++)//ランダムに入れ替える
            {
                Swap(ref x[i], ref x[i + (b / Fact(a - i - 1)) % (a - i)]);
            }
        }

    }
}
