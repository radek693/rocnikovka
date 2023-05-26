using System;
using System.Security.Policy;

namespace Checkers
{
    class Program
    {
        static void Main(string[] args)
        {
            int hrac = 1;
            int[,] sachovnice = new int[8, 8];
            int[,] predchoziPozice = new int[8, 8];

            // definuje šachovnici[], zapisuje startovní pozici
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (i < 3 && (i + j) % 2 == 0)
                    {
                        sachovnice[i, j] = 1; // hráč 1
                    }
                    else if (i > 4 && (i + j) % 2 == 0)
                    {
                        sachovnice[i, j] = 2; // hráč 2
                    }
                }
            }
            bool prohra = false;

            while (true) // cyklus, který se opakuje dokud je podmínka prohra false
            {
                Console.Clear(); // vyčistí konzoli
                Console.WriteLine("(O) - hráč-1 (X) - hráč - 2");
                Console.WriteLine("Právě na tahu: Hráč-" + hrac); // vypiše kdo právě hraje

                VykresliPole(sachovnice); // vykreslí šachovnici

                // získání začáteční pozice na šachovnici
                Console.WriteLine("Vložte začínající řádek a sloupec (oddělit mezerou):");
                string[] vstup = Console.ReadLine().Split();
                int startovniradek, startovnisloupec;

                // kontroluje zda začáteční pozice obsahuje 2 prvky platnými čísli
                if (vstup.Length != 2 || !int.TryParse(vstup[0], out startovniradek) || !int.TryParse(vstup[1], out startovnisloupec))
                {                              // TryParse metoda na číslo
                    Console.WriteLine("Neplatný Tah (Pro pokračování stiskněte enter)");
                    Console.ReadKey();
                    continue;
                }

                // získání konečné pozice na šachovnici 
                Console.WriteLine("Vložte končící řádek a sloupec (oddělit mezerou):");
                string[] vstup2 = Console.ReadLine().Split();
                int konecnyradek, konecnysloupec;

                // kontroluje zda koncová pozice obsahuje 2 prvky platnými čísli
                if (vstup2.Length != 2 || !int.TryParse(vstup2[0], out konecnyradek) || !int.TryParse(vstup2[1], out konecnysloupec))
                {
                    Console.WriteLine("Neplatný Tah (Pro pokračování stiskněte enter)");
                    Console.ReadKey();
                    continue;
                }

                // ověřuje platnost tahu
                int validMove = IsValidMove(sachovnice, startovniradek, startovnisloupec, konecnyradek, konecnysloupec, hrac);
                if (validMove == 0)
                {
                    Console.ReadKey();
                    continue; // Pokud tah není platný, přeskočí se zbytek kódu a pokračuje další iterací cyklu.
                }
                else if (validMove == 2)
                {
                    sachovnice[(startovniradek + konecnyradek) / 2, (startovnisloupec + konecnysloupec) / 2] = 0;
                }
                // ověřuje platnost tahu 
                if (sachovnice[startovniradek, startovnisloupec] == hrac &&
                    IsValidMove(sachovnice, startovniradek, startovnisloupec, konecnyradek, konecnysloupec, hrac) > 0)
                {
                    if (IsValidMove(sachovnice, startovniradek, startovnisloupec, konecnyradek, konecnysloupec, hrac) == 2)
                    {
                        sachovnice[(startovniradek + konecnyradek) / 2, (startovnisloupec + konecnysloupec) / 2] = 0;
                    }
                    sachovnice[konecnyradek, konecnysloupec] = sachovnice[startovniradek, startovnisloupec];
                    sachovnice[startovniradek, startovnisloupec] = 0;

                    // Proměna figurky na písmeno 'K' u hráče 1
                    if (hrac == 1 && konecnyradek == 7)
                    {
                        sachovnice[konecnyradek, konecnysloupec] = 'K';
                    }
                    // Proměna figurky na písmeno 'M' u hráče 2
                    else if (hrac == 2 && konecnyradek == 0)
                    {
                        sachovnice[konecnyradek, konecnysloupec] = 'M';
                    }

                    // uložení předchozí pozice
                    predchoziPozice = (int[,])sachovnice.Clone(); // Vytvoří se kopie šachovnice

                }

                // zkontroluje jestli hráč vyhrál
                if (Vyhral(sachovnice, hrac))
                {
                    Console.Clear(); // vyčistí konzoli
                    VykresliPole(sachovnice);
                    Console.WriteLine("Hráč: " + hrac + " Vyhrál!"); // vypíše hráče, který vyhrál hru
                    prohra = true;
                    break;
                }
                else
                {
                    hrac = (hrac == 1) ? 2 : 1;      // přepne na druhého hráče
                }
            }
        }

        static void VykresliPole(int[,] sachovnice)
        {
            Console.WriteLine("  0 1 2 3 4 5 6 7"); // vypíše čísla sloupců
            for (int i = 0; i < 8; i++)
            {
                Console.Write(i);
                for (int j = 0; j < 8; j++)     // for cyklus
                {
                    switch (sachovnice[i, j])     // i - řádek, j - sloupec
                    {
                        case 0:
                            Console.Write(" -");
                            break;
                        case 1:
                            Console.Write(" O");
                            break;
                        case 2:
                            Console.Write(" X");
                            break;
                        case 'K':
                            Console.Write(" K");
                            break;
                        case 'M':
                            Console.Write(" Q");
                            break;
                    }
                }
                Console.WriteLine();
            }
        }

        // vrací čísla: 0 - neplatný tah, 1 - platný tah, nikoho neskočil, 2 - platný tah, něco skočil
        static int IsValidMove(int[,] sachovnice, int startovniRadek, int startovniSloupec, int konecnyRadek, int konecnySloupec, int hrac)
        {               // Math.Abs počítá rozdíl mezi deltaX a deltaY mezi  konecnyradek a konecnysloupec,tím s získá vzdálenost mezi dvěma hodnotami
            int deltaX = Math.Abs(konecnyRadek - startovniRadek);
            int deltaY = Math.Abs(konecnySloupec - startovniSloupec);

            // podmínka ověřující platnost tahu
            if (deltaX != deltaY || deltaX > 2 || deltaY > 2)
            {
                Console.WriteLine("Neplatný Tah (Pro pokračování stiskněte enter)");
                Console.ReadKey();
                return 0;
            }

            int opponent = (hrac == 1) ? 2 : 1;

            // kontroluje jestli skáče přes hráče
            if (sachovnice[(startovniRadek + konecnyRadek) / 2, (startovniSloupec + konecnySloupec) / 2] == opponent && sachovnice[konecnyRadek, konecnySloupec] == 0)
            {
                return 2;
            }
            else if (deltaX == 1 && deltaY == 1 && sachovnice[konecnyRadek, konecnySloupec] == 0)
            {
                return 1;
            }
            return 1;
        }

        // kontroluje jestli hráč na tahu neprohrál
        static bool Vyhral(int[,] sachovnice, int hrac)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (sachovnice[i, j] == hrac || sachovnice[i, j] == hrac + 2)
                    {
                        return false;
                    }
                }
            }
            Console.WriteLine("Vyhrál hráč - " + hrac);
            return true;
        }
    }
}


