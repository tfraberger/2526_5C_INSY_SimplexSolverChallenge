using System.Text;

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
        
        public void fillLine(int zeile, double[] werte)
        {
            var spalten = matrix.GetLength(1);

            for (var i = 0; i < spalten; i++) {
                if (zeile == 0 && i == spalten - 5)
                    matrix[zeile, i] = 1;
                if (zeile == 1 && i == spalten - 4)
                    matrix[zeile, i] = 1;
                if (zeile == 2 && i == spalten - 3)
                    matrix[zeile, i] = 1;

                matrix[zeile, i] = 0;
                
                // Legende
                if (i < 3)
                    legenden [i] = "x" + (i + 1);
                else if (i >= spalten - 5 && i < spalten - 2)
                    legenden [i] = "s" + (i - (spalten - 5));
                else if (i == spalten - 2)
                    legenden [i] = "RS";
                else if (i == spalten - 1)
                    legenden [i] = "Q";
            }
            
            if (werte.Length > 0) matrix[zeile, 0] = werte[0];                 // x1
            if (werte.Length > 1) matrix[zeile, 1] = werte[1];                 // x2
            if (werte.Length > 2 && spalten > 1) matrix[zeile, spalten - 2] = werte[2]; // RS
        }
        
        //Pivot-Spalte ermitteln
        private int GetPivotSpalte()
        {
            int index = -1;
            double min = 0;

            int lastrow = matrix.GetLength(0) - 1;
                
            int colsMatrix = matrix.GetLength(1);
            int colsSchlupf = schlupf.GetLength(1);

            int totalCols = colsMatrix + colsSchlupf + 1;
            double[] letzteZeile = new double[totalCols];

            for (int i = 0; i < colsMatrix; i++)
                letzteZeile[i] = matrix[lastrow, i];

            for (int i = 0; i < colsSchlupf; i++)
                letzteZeile[colsMatrix + i] = schlupf[lastrow, i];
                
            letzteZeile[colsMatrix + colsSchlupf] = rs[lastrow];
                
            for (int i = 0; i < letzteZeile.Length; i++)
            {
                if (letzteZeile[i] < min)
                {
                    min = letzteZeile[i];
                    index = i;
                }
            }

            pivotSpalte = index;
            return index;
        }
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
        public override string ToString() {
            var sb = new StringBuilder();
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int schlupflength = schlupf.GetLength(1);

            sb.AppendLine("matrix:");
            sb.AppendLine();

            for (int c = 0; c < cols + schlupflength + 1; c++)
                sb.Append($"{legenden[c]}");
            sb.AppendLine();

            // die matri, schlupf und b spalte
            for (int r = 0; r < rows; r++)
            {
                // matrix spalte
                for (int c = 0; c < cols; c++)
                    sb.Append($"{matrix[r, c]}");

                // schlupf splaten
                for (int c = 0; c < schlupflength; c++)
                    sb.Append($"{schlupf[r, c]}");

                // rs spalten
                sb.Append($"{rs[r]}");

                sb.AppendLine();
            }

            // q werte
            sb.AppendLine();
            sb.Append("q Werte: ");
            for (int r = 0; r < q.Length; r++)
                sb.Append($"{q[r]},  ");

            sb.AppendLine();
            sb.AppendLine($"p spalte: {pivotSpalte}");
            sb.AppendLine($"p zeile: {pivotZeile}");
            sb.AppendLine($"ergebnis: {Solved}");

            return sb.ToString();
        }
        
    }
}
