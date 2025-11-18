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

        public Produktionsmatrix(int anzProdukte, int anzNebenbed){}
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
