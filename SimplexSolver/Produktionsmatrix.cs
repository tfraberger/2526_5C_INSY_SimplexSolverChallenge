namespace LO_bibCORE
{
    public class Produktionsmatrix
    {
        public bool Solved { get; private set; }

        private double[,] matrix;
        private double[,] schlupf;
        public double[] rs { get; private set; } //für Lösung ;)
        private double[] q;
        public string[] legenden { get; private set; } //für Lösung ;);

        int pivotSpalte;
        int pivotZeile;

        // Hilfsarray
        private double[] faktor;//quasi die absoluten Zellbezüge vom Excel
        
        public Produktionsmatrix(int anzProdukte, int anzNebenbed)
        {
            if (anzProdukte < 1) anzProdukte = 1;
            if (anzNebenbed < 0) anzNebenbed = 0;

            int rows = anzNebenbed + 1;
            int cols = anzProdukte + 1;

            matrix = new double[rows, cols];
            schlupf = new double[rows, anzNebenbed];

            for (int i = 1; i < rows; i++)
            {
                int slackIndex = i - 1;
                if (slackIndex < anzNebenbed)
                    schlupf[i, slackIndex] = 1.0;
            }

            rs = new double[rows];
            q = new double[rows];

            int totalColumns = anzProdukte + anzNebenbed + 1;
            faktor = new double[totalColumns];
            legenden = new string[totalColumns];

            for (int i = 0; i < anzProdukte; i++)
                legenden[i] = "x" + (i + 1);

            for (int i = 0; i < anzNebenbed; i++)
                legenden[anzProdukte + i] = "s" + (i + 1);

            legenden[anzProdukte + anzNebenbed] = "b";

            pivotSpalte = -1;
            pivotZeile = -1;
            Solved = false;
        }
        
        public void fillLine(int zeile, double[] werte){}
        //Pivot-Spalte ermitteln
        private int GetPivotSpalte(){}
        //Quotienten ausrechnen
        public void BerechneQutienten()
        {
            // Pivot-Spalte bestimmen
            pivotSpalte = GetPivotSpalte();

            q = new double[matrix.GetLength(0)];
            double minQ = double.PositiveInfinity;
            pivotZeile = -1;

            // Für jede Nebenbedingungs-Zeile (beginnend bei 1)
            for (int i = 1; i < matrix.GetLength(0); i++)
            {
                double pivotKandidat = matrix[i, pivotSpalte];

                if (pivotKandidat > 0)     // Nur positive Koeffizienten sind gültig
                {
                    q[i] = rs[i] / pivotKandidat;

                    if (q[i] < minQ)
                    {
                        minQ = q[i];
                        pivotZeile = i;
                    }
                }
                else
                {
                    q[i] = double.PositiveInfinity;
                }
            }

            // Fehlerbehandlung: keine gültige Pivotzeile → unbeschränkt
            if (pivotZeile == -1)
            {
                Console.WriteLine("Kein gültiger Pivot gefunden — Lösung unbeschränkt!");
                Solved = true;
            }
        }
        }
        //Pivotzeile durchdividieren
        public void DividierePivotZeile(){}
        //alle Zeilen "faktor"-mal von anderen abziehen
        public void SubtrahiereRestAusserPivotZeile(){}
        //Ausgabe
        public override string ToString(){}
        
    }
}
