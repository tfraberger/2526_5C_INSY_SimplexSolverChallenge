using LO_bibCORE;

namespace _2526_5C_INSY_SimplexSolverChallenge;

class Program {
//Gaense Beispiel
    //static Produktionsmatrix pm = new Produktionsmatrix(2, 2);

    //Barkeeper
    //static Produktionsmatrix pm = new Produktionsmatrix(3, 4);

    //Bierbrauer/Auto Beispiel
    static Produktionsmatrix pm = new Produktionsmatrix(2, 2);

    static void Main(string[] args)
    {
        Console.WriteLine(pm.ToString()); // Leeres System aus lauter 0 ausgeben
        Console.WriteLine("-------------------------------------------------------------------------------");
        Console.ReadKey();
        ////Bier
        pm.fillLine(0, new double[] { 100, 90, 0 });    //Gewinnfunktion
        pm.fillLine(1, new double[] { 5, 3, 100 });     //NB1
        pm.fillLine(2, new double[] { 2, 2, 50 });      //NB2

        Console.WriteLine(pm.ToString()); // Leeres System aus lauter 0 ausgeben
        Console.WriteLine("--------------------------------------------------------------------------------");
        

        #region sonstnowos
        ////Autoreifen
        ////pm.addLine(0, new double[] { 20, 16, 0 });    //Gewinnfunktion
        ////pm.addLine(1, new double[] { 2, 4, 400 });     //NB1
        ////pm.addLine(2, new double[] { 3, 2, 300 });      //NB2

        ////Gaense
        ////pm.addLine(0, new double[] { 0.03, 0.02, 0 });    //Gewinnfunktion
        ////pm.addLine(1, new double[] { 2, 1, 10000 });     //NB1
        ////pm.addLine(2, new double[] { 1, 1, 6500 });      //NB2
        ////pm.addLine(1, new double[] { 1, -2, 0 });     //NB1
        ////pm.addLine(2, new double[] { 3, -2, 0 });      //NB2


        ////pm.addLine(0, new double[] { 5.5, 4.5, 7, 0 });   //Gewinnfunktion
        ////pm.addLine(1, new double[] { 45,0,20,5000 });     //NB1
        ////pm.addLine(2, new double[] { 30,30,20,6000 });    //NB2
        ////pm.addLine(3, new double[] { 0, 30, 20, 4000 });  //NB3
        ////pm.addLine(4, new double[] { 0, 0, 20, 3000 });   //NB4
        #endregion
        //Console.WriteLine(pm.ToString());
        //Console.WriteLine("-------------------------------------------------------------------------------");

        while (!pm.Solved)
            Iterate(false);

        Console.ReadKey();
    }
    static void Iterate(bool debug)
    {
        // 3 Teilschritte zur besseren Darstellung als public Methoden 
        pm.BerechneQutienten();
        if (debug) Console.WriteLine(pm.ToString());
        if (debug) Console.WriteLine("-------------------------------------------------------------------------------");

        pm.DividierePivotZeile();
        if (debug) Console.WriteLine(pm.ToString());
        if (debug) Console.WriteLine("-------------------------------------------------------------------------------");

        pm.SubtrahiereRestAusserPivotZeile();
        Console.WriteLine(pm.ToString());
        Console.WriteLine("-------------------------------------------------------------------------------");

    }
}