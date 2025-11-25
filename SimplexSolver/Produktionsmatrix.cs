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

            for (int i = 0; i < rows - 1; i++)
            {
                int slackIndex = i;
                if (slackIndex < anzNebenbed)
                    schlupf[i, slackIndex] = 1.0;
            }

            rs = new double[rows];
            q = new double[rows];

            int totalColumns = rows;
            faktor = new double[totalColumns];
            legenden = new string[totalColumns];
            
            for (int i = 0; i < rows - 1; i++)
                legenden[i] = "s" + (i + 1);
            
            legenden[rows - 1] = "Z";
            
            pivotSpalte = -1;
            pivotZeile = -1;
            Solved = false;
        }
        
        public void fillLine(int zeile, double[] werte)
        {
            int cols = matrix.GetLength(1);

            for (int i = 0; i < cols; i++)
                matrix[zeile, i] = werte.Length > i ? werte[i] : 0;

            //if (zeile > 0 && zeile - 1 < schlupf.GetLength(1))
             //   schlupf[zeile, zeile - 1] = 1;

            if (werte.Length > cols)
                rs[zeile] = werte[cols];
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
                Console.WriteLine(i + " - " + letzteZeile[i]);
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
        public void BerechneQuotienten()
        {
            // Pivot-Spalte bestimmen
            pivotSpalte = GetPivotSpalte();

            // keine Pivotspalte → Optimale Lösung
            if (pivotSpalte == -1)
            {
                Console.WriteLine("Optimale Lösung erreicht.");
                Solved = true;
                return;
            }

            q = new double[matrix.GetLength(0)];
            double minQ = double.PositiveInfinity;
            pivotZeile = -1;

            for (int i = 1; i < matrix.GetLength(0); i++)
            {
                double pivotKandidat;

                // Pivot aus x- oder schlupf-Bereich holen
                if (pivotSpalte < matrix.GetLength(1))
                    pivotKandidat = matrix[i, pivotSpalte];
                else
                    pivotKandidat = schlupf[i, pivotSpalte - matrix.GetLength(1)];

                if (pivotKandidat > 0)
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

            // die matri, schlupf und b spalte
            for (int r = 0; r < rows; r++)
            {
                sb.Append($"{legenden[r]}\t");
                // matrix spalte
                for (int c = 0; c < cols; c++)
                    sb.Append($"{matrix[r, c]}\t");

                // schlupf splaten
                for (int c = 0; c < schlupflength; c++)
                    sb.Append($"{schlupf[r, c]}\t");

                // rs spalten
                sb.Append($"{rs[r]}\t");

                sb.AppendLine();
            }

            // q werte
            sb.AppendLine();
            sb.Append("q Werte: \t");
            for (int r = 0; r < q.Length; r++)
                sb.Append($"{q[r]},  ");

            sb.AppendLine();
            sb.AppendLine($"p spalte: \t{pivotSpalte}");
            sb.AppendLine($"p zeile: \t{pivotZeile}");
            sb.AppendLine($"ergebnis: \t{Solved}");

            return sb.ToString();
        }
        
    }
}
