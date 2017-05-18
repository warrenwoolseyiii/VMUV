
namespace Motus_1_Pipe_Server.Utilities
{
    class RawDataFilter
    {
        public int numAccumulationSamples = 15;
        public bool accumLimitReached = false;

        private int accumIndex = 0;
        private int[] accumWork;
        private int[] accumulatorStorage;
        private short[] averageStorage;

        public void AccumulateNextSample(short[] data)
        {
            if (data == null)
                return;

            if (accumIndex == 0)
            {
                accumWork = new int[data.Length];
                accumIndex++;

                for (int i = 0; i < data.Length; i++)
                    accumWork[i] += data[i];

                return;
            }

            accumIndex++;

            for (int i = 0; i < data.Length; i++)
                accumWork[i] += data[i];

            if (accumIndex >= numAccumulationSamples)
            {
                accumLimitReached = true;
                accumulatorStorage = accumWork;

                averageStorage = new short[accumulatorStorage.Length];
                ComputeAverage();

                accumIndex = 0;
            }
        }

        public int[] GetAccumCurrentAccum()
        {
            return accumulatorStorage;
        }

        public short[] GetAverageStorage()
        {
            return averageStorage;
        }

        private void ComputeAverage()
        {
            for (int i = 0; i < accumulatorStorage.Length; i++)
            {
                int tmp = (accumulatorStorage[i] / numAccumulationSamples);
                averageStorage[i] = (short)tmp;
            }
        }
    }
}
