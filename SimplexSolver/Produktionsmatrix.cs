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
        public void BerechneQutienten(){}
        //Pivotzeile durchdividieren
        public void DividierePivotZeile(){}
        //alle Zeilen "faktor"-mal von anderen abziehen
        public void SubtrahiereRestAusserPivotZeile()
        {
            if (matrix == null || pivotSpalte < 0 || pivotZeile < 0) return;

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int schlCols = schlupf.GetLength(1);

            for (int z = 0; z < rows; z++)
            {
                if (z == pivotZeile) continue;

                double f = matrix[z, pivotSpalte];
                if (faktor != null && z < faktor.Length) faktor[z] = f;

                for (int sp = 0; sp < cols; sp++)
                {
                    matrix[z, sp] -= f * matrix[pivotZeile, sp];
                }

                if (schlupf != null)
                {
                    for (int s = 0; s < schlCols; s++)
                    {
                        schlupf[z, s] -= f * schlupf[pivotZeile, s];
                    }
                }

                if (rs != null && pivotZeile < rs.Length && z < rs.Length)
                {
                    rs[z] -= f * rs[pivotZeile];
                }
            }
        }
        //Ausgabe
        public override string ToString(){}
        
    }
}
