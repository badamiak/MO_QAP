namespace MO_QAP
{
    public class DataMatrices
    {
        public DataMatrices(int size)
        {
            this.MatricesSize = size;
            this.MatrixA = new int[size,size];
            this.MatrixB = new int[size,size];
        }

        public int MatricesSize {get;}
        public int[,] MatrixA{get;set;}
        public int[,] MatrixB{get;set;}
    }
}