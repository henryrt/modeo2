namespace RTH.Modeo2
{
    public interface IObjective
    {
        // returns (double) the Value of the Objective for the given Solution
        double Value(ISolution soln);

        // returns (int) the Penalty the Value incurs for the given Solution
        int Penalty(ISolution soln);

        // returns (int) the Penalty the given Value incurs
        int Penalty(double val);
    }
}
