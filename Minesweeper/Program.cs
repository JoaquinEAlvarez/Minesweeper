using System;
using System.Threading;

namespace mineSweeper
{
    public class Celda
    {
        public int CantMinasCerca;
        public bool EsMina;
        public bool TieneBandera;
        public bool EstaCubierta;


        public Celda(bool esMina)
        {
            this.EsMina = esMina;
            this.EstaCubierta = true;
        }
    }


    class MineSweeper
    {

        public Random RNG = new Random();

        public Celda[,] Celdas;
        public int CantMinas;
        public int CasillerosCubiertos;
        public int CantBanderasRestantes;

        public bool Jugando;
        public bool EsVictoria;

        public MineSweeper(int cantMinas)
        {
            this.Jugando = true;
            this.EsVictoria = false;
            this.CantMinas = cantMinas;
            this.CantBanderasRestantes = this.CantMinas;

            this.CasillerosCubiertos = this.CantMinas * this.CantMinas;
        }
    }

    class Input
    {
        public int CurPosFil;
        public int CurPosCol;
        public bool PonerBandera;
        public bool DescubrirCelda;
        public bool Cambio;
    }



    class MineSweeperGUI
    {
        public MineSweeper Juego;
        public Input Input;

        public MineSweeperGUI(MineSweeper juego)
        {
            this.Juego = juego;
            this.Input = new Input();
        }
    }






    class Program
    {
        //----------------------------------------------------------------------------------
        //Celda
        //----------------------------------------------------------------------------------
        static void Celda_Descubrir(Celda celda)
        {
            celda.EstaCubierta = false;
        }



        //----------------------------------------------------------------------------------
        //MineSweeper
        //----------------------------------------------------------------------------------
        static void MineSweeper_ColocarMinas(MineSweeper mineSweeper)
        {
            int minasColocadas = 0;

            mineSweeper.Celdas = new Celda[mineSweeper.CantMinas, mineSweeper.CantMinas];

            while (minasColocadas < mineSweeper.CantMinas)
            {
                for (int fila = 0; fila < mineSweeper.Celdas.GetLength(0); fila++)
                {
                    for (int col = 0; col < mineSweeper.Celdas.GetLength(1); col++)
                    {
                        bool esMina = (minasColocadas < mineSweeper.CantMinas && MineSweeper_DebeColocarMina(mineSweeper));

                        if (mineSweeper.Celdas[fila, col] == null)
                        {
                            mineSweeper.Celdas[fila, col] = new Celda(esMina);
                            if (esMina)
                            {
                                minasColocadas++;
                            }
                        }
                        else if (esMina && !mineSweeper.Celdas[fila, col].EsMina)
                        {
                            minasColocadas++;
                            mineSweeper.Celdas[fila, col].EsMina = esMina;
                        }

                    }
                }
            }
        }

        static bool MineSweeper_DebeColocarMina(MineSweeper mineSweeper)
        {
            return mineSweeper.RNG.Next(0, 100) > 90;
        }

        static bool MineSweeper_EsCoordValida(MineSweeper mineSweeper, int fil, int col)
        {
            return fil >= 0 && fil < mineSweeper.Celdas.GetLength(0) && col >= 0 && col < mineSweeper.Celdas.GetLength(1);
        }

        static void MineSweeper_DescubrirMinas(MineSweeper mineSweeper)
        {
            for (int fila = 0; fila < mineSweeper.Celdas.GetLength(0); fila++)
            {
                for (int col = 0; col < mineSweeper.Celdas.GetLength(1); col++)
                {
                    if (mineSweeper.Celdas[fila, col].EsMina)
                        Celda_Descubrir(mineSweeper.Celdas[fila, col]);
                }
            }
        }

        static void MineSweeper_DescubrirCelda(MineSweeper mineSweeper, int fil, int col)
        {

            if (!MineSweeper_EsCoordValida(mineSweeper, fil, col))
                return;

            if (mineSweeper.Celdas[fil, col].TieneBandera)
                return;

            if (mineSweeper.Celdas[fil, col].EsMina)
            {
                mineSweeper.Jugando = false;
                mineSweeper.EsVictoria = false;
                MineSweeper_DescubrirMinas(mineSweeper);
                return;
            }

            int minasCerca = 0;

            for (int f = fil - 1; f <= fil + 1; f++)
            {
                for (int c = col - 1; c <= col + 1; c++)
                {
                    if ((f != fil || c != col) && MineSweeper_EsCoordValida(mineSweeper, f, c) && mineSweeper.Celdas[f, c].EsMina)
                    {
                        minasCerca++;
                    }
                }
            }

            mineSweeper.Celdas[fil, col].CantMinasCerca = minasCerca;
            Celda_Descubrir(mineSweeper.Celdas[fil, col]);
            mineSweeper.CasillerosCubiertos--;

            if (mineSweeper.Celdas[fil, col].CantMinasCerca == 0)
            {
                for (int f = fil - 1; f <= fil + 1; f++)
                {
                    for (int c = col - 1; c <= col + 1; c++)
                    {
                        if ((f != fil || c != col) && MineSweeper_EsCoordValida(mineSweeper, f, c) && !mineSweeper.Celdas[f, c].EsMina)
                        {
                            if (mineSweeper.Celdas[f, c].EstaCubierta)
                                MineSweeper_DescubrirCelda(mineSweeper, f, c);
                        }
                    }
                }
            }
        }

        static void MineSweeper_UbicarBandera(MineSweeper mineSweeper, int fil, int col)
        {


            if (!MineSweeper_EsCoordValida(mineSweeper, fil, col))
                return;

            if (!mineSweeper.Celdas[fil, col].TieneBandera && mineSweeper.CantBanderasRestantes > 0)
            {
                mineSweeper.CantBanderasRestantes--;
                mineSweeper.CasillerosCubiertos--;
                mineSweeper.Celdas[fil, col].TieneBandera = true;

            }
            else if (mineSweeper.Celdas[fil, col].TieneBandera)
            {
                mineSweeper.CantBanderasRestantes++;
                mineSweeper.CasillerosCubiertos++;
                mineSweeper.Celdas[fil, col].TieneBandera = false;
            }

        }



        static void MineSweeper_ValidarTablero(MineSweeper mineSweeper)
        {
            if (mineSweeper.CasillerosCubiertos == 0)
            {

                mineSweeper.Jugando = false;
                mineSweeper.EsVictoria = true;

                for (int fila = 0; fila < mineSweeper.Celdas.GetLength(0) && mineSweeper.EsVictoria; fila++)
                {
                    for (int col = 0; col < mineSweeper.Celdas.GetLength(1) && mineSweeper.EsVictoria; col++)
                    {
                        if (mineSweeper.Celdas[fila, col].EsMina && !mineSweeper.Celdas[fila, col].TieneBandera)
                            mineSweeper.EsVictoria = false;
                    }
                }

                if (!mineSweeper.EsVictoria)
                    MineSweeper_DescubrirMinas(mineSweeper);
            }
        }

        static bool MineSweeper_EsFinDelJuego(MineSweeper mineSweeper)
        {
            return !mineSweeper.Jugando || mineSweeper.EsVictoria;
        }

        static bool MineSweeper_EsVictoria(MineSweeper mineSweeper)
        {
            return mineSweeper.EsVictoria;
        }





        //----------------------------------------------------------------------------------
        //Input
        //----------------------------------------------------------------------------------
        static void Input_Reset(Input input)
        {
            input.Cambio = false;
            input.PonerBandera = false;
            input.DescubrirCelda = false;
        }

        //----------------------------------------------------------------------------------
        //MineSweeperGUI
        //----------------------------------------------------------------------------------
        static void MineSweeperGUI_ActualizarInput(MineSweeperGUI gui)
        {
            Input_Reset(gui.Input);
            if (Console.KeyAvailable)
            {
                gui.Input.Cambio = true;
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        gui.Input.CurPosFil--;
                        break;
                    case ConsoleKey.DownArrow:
                        gui.Input.CurPosFil++;
                        break;
                    case ConsoleKey.LeftArrow:
                        gui.Input.CurPosCol--;
                        break;
                    case ConsoleKey.RightArrow:
                        gui.Input.CurPosCol++;
                        break;
                    case ConsoleKey.Spacebar:
                        gui.Input.PonerBandera = true;
                        break;
                    case ConsoleKey.Enter:
                        gui.Input.DescubrirCelda = true;
                        break;
                }
                gui.Input.CurPosFil = Math.Clamp(gui.Input.CurPosFil, 0, gui.Juego.Celdas.GetLength(0) - 1);
                gui.Input.CurPosCol = Math.Clamp(gui.Input.CurPosCol, 0, gui.Juego.Celdas.GetLength(1) - 1);

            }
        }

        static void MineSweeperGUI_DibujarCeldaConNumero(int nro)
        {
            ConsoleColor consoleColor = Console.ForegroundColor;

            if (nro == 1)
                consoleColor = ConsoleColor.Cyan;
            else if (nro == 2)
                consoleColor = ConsoleColor.Yellow;
            else if (nro >= 3)
                consoleColor = ConsoleColor.Magenta;
            Console.ForegroundColor = consoleColor;
            Console.Write($"[{nro:D3}]");
        }




        static void MineSweeperGUI_DibujarCelda(Celda c)
        {
            if (c.TieneBandera)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("[ P ]");
            }
            else if (!c.EstaCubierta)
            {
                if (c.EsMina)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("[ X ]");
                }
                else
                {
                    MineSweeperGUI_DibujarCeldaConNumero(c.CantMinasCerca);
                }
            }
            else
            {
                Console.Write("[   ]");
            }
        }

        static void MineSweeperGUI_DibujarTablero(MineSweeperGUI gui)
        {
            Console.SetCursorPosition(0, 0);
            for (int fila = 0; fila < gui.Juego.Celdas.GetLength(0); fila++)
            {
                for (int columna = 0; columna < gui.Juego.Celdas.GetLength(1); columna++)
                {
                    if (fila == gui.Input.CurPosFil && columna == gui.Input.CurPosCol)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    MineSweeperGUI_DibujarCelda(gui.Juego.Celdas[fila, columna]);


                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }


        static void MineSweeperGUI_Actualizar(MineSweeperGUI gui)
        {
            MineSweeperGUI_ActualizarInput(gui);

            if (gui.Input.DescubrirCelda)
            {
                MineSweeper_DescubrirCelda(gui.Juego, gui.Input.CurPosFil, gui.Input.CurPosCol);

            }
            if (gui.Input.PonerBandera)
            {
                MineSweeper_UbicarBandera(gui.Juego, gui.Input.CurPosFil, gui.Input.CurPosCol);
            }

        }

        static void MineSweeperGUI_Dibujar(MineSweeperGUI gui)
        {
            MineSweeperGUI_DibujarTablero(gui);
            Console.WriteLine($"Banderas Restantes {gui.Juego.CantBanderasRestantes:D3}");
            Console.WriteLine($"Movimiento = Flechas");
            Console.WriteLine($"Colocar Bandera = Espacio");
            Console.WriteLine($"Descubrir = Enter");
        }


        static void Jugar(MineSweeperGUI gui)
        {
            MineSweeper_ColocarMinas(gui.Juego);
            MineSweeperGUI_Dibujar(gui);
            while (!MineSweeper_EsFinDelJuego(gui.Juego))
            {
                MineSweeperGUI_Actualizar(gui);
                MineSweeper_ValidarTablero(gui.Juego);
                if (gui.Input.Cambio)
                    MineSweeperGUI_Dibujar(gui);
                Thread.Sleep(16);
            }


            Console.WriteLine(gui.Juego.EsVictoria ? "Victoria!" : "Derrota!");
            Console.ReadLine();
        }

        static void Main(string[] args)
        {

            MineSweeper juego = new MineSweeper(10);
            MineSweeperGUI g = new MineSweeperGUI(juego);
            Jugar(g);

        }
    }
}