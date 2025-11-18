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

            matrix = new double[rows, anzProdukte];
            schlupf = new double[rows, anzNebenbed];

            for (int i = 1; i < rows; i++)
            {
                int slackIndex = i - 1;
                if (slackIndex < anzNebenbed)
                    schlupf[i, slackIndex] = 1.0;
            }

            rs = new double[rows];
            q = new double[rows];

            int totalColumns = rows;
            faktor = new double[totalColumns];
            legenden = new string[totalColumns];

            legenden[0] = "Z";
            for (int i = 1; i < rows; i++)
                legenden[i] = "s" + (i + 1);
            
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
        public void DividierePivotZeile() {
            // Pivot-Element holen
            double pivotWert = matrix[pivotZeile, pivotSpalte];

            // Fehlerbehandlung, Division durch 0 vermeiden
            if (pivotWert == 0)
                throw new DivideByZeroException("Pivot-Wert ist 0, Division nicht möglich.");

            // Pivotzeile vollständig durch den Pivot-Wert teilen
            for (int spalte = 0; spalte < matrix.GetLength(1); spalte++) {
                matrix[pivotZeile, spalte] /= pivotWert;
            }

            // Auch den rechten Seitenwert (rs) teilen, falls vorhanden
            if (rs != null && rs.Length > pivotZeile)
                rs[pivotZeile] /= pivotWert;

            // Schlupfwerte ebenfalls normieren, falls verwendet
            if (schlupf != null) {
                for (int s = 0; s < schlupf.GetLength(1); s++) {
                    schlupf[pivotZeile, s] /= pivotWert;
                }
            }
        }
        //alle Zeilen "faktor"-mal von anderen abziehen
        public void SubtrahiereRestAusserPivotZeile(){}
        //Ausgabe
        public override string ToString(){}
        
    }
}
