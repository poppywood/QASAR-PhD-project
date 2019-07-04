using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using Qasar.Mva;
namespace Qasar.Mva
{

    public class ApproxMva
    {
        int R = 0;
        int MAXITS = 1000;
        MVARequest request = null;
        public MVAResponse PerformCalc(MVARequest request)
        {

            this.request = request;
            R = request.NoClasses;      //not including terminal     
            int K = request.NoResources;    //row
            int[] N = new int[R + 1];
            double epsilon = 0.00001;
            double smallestDiff = 10;
            double Error = 1;
            double[,] Xprev = new double[K + 1, R + 1];
            double[,] Xbest = new double[K + 1, R + 1];
            double[,] Rbest = new double[K + 1, R + 1];
            double[,] Xcurr = new double[K + 1, R + 1];
            int Nbar = 0; //sum of N[r]
            for (int r = 1; r <= R; r += 1)
            {
                //N[r] = (int)request.Classes[r - 1].Resources[0].EquivLoad;
                N[r] = (int)request.Classes[r - 1].TotalPopulation;
                Nbar += N[r];
            }
            //Nbar = (int)request.Classes[0].Resources[0].EquivLoad;
            double[,] P = new double[K + 1, Convert.ToInt32(Nbar) + 1];  //Pi(j|N)
            double[,] Rd = new double[K + 1, R + 1];
            double[,] RunRd = new double[K + 1, R + 1];
            double[,] D = new double[K + 1, R + 1];

            for (int r = 1; r <= R; r += 1)
            {
                for (int i = 0; i <= K; i += 1)
                {
                    if (i == 0)
                        D[i, r] = Rd[i, r] = request.Classes[r - 1].Resources[i].ServiceTime;//
                    else
                        D[i, r] = Rd[i, r] = request.Classes[r - 1].Resources[i].ServiceTime * request.Classes[r - 1].Resources[i].EquivLoad;
                }
            }

            //INIT
            for (int i = 1; i <= K; i += 1)
            {

                for (int j = 1; j <= Nbar; j += 1)
                {
                    P[0, j] = 1;
                    P[i, j] = 1D / Nbar;
                }
            }
            for (int r = 1; r <= R; r += 1)
            {
                double SumDi = 0;
                for (int i = 0; i <= K; i += 1)
                {
                    SumDi += D[i, r];
                    if (SumDi > 0)
                        Xprev[i, r] = Math.Min(N[r] / (SumDi + Rd[i, r]), 1D / (largestD(D, R, K) + Rd[i, r]));
                        //Xprev[i, r] = 1D / (largestD(D, R, K) + Rd[i, r]);
                    else
                        Xprev[i, r] = 0;
                }
                
            }
            //MAIN LOOP
            int it = 0;

            while (Error > epsilon)
            {
                it += 1;

                //RES TIMES
                for (int i = 0; i <= K; i += 1)
                {
                    for (int r = 1; r <= R; r += 1)
                    {
                        Rd[i, r] = D[i, r] * SumPN(i, N[r], P, r);
                        //Rd[i, r] = D[i, r] * SumPN(i, Nbar, P, r);
                        RunRd[i, r] += Rd[i, r];

                    }
                }
                //PROBS

                for (int i = 1; i <= K; i += 1)
                {
                    double runningTotal = 0;
                    for (int j = Convert.ToInt32(Nbar); j >= 1; j -= 1)
                    {

                        double dum = P[i, j];
                        P[i, j] = P[i, j - 1] * SumDX(i, j, D, R, Xprev);
                        if (P[i, j] > 1)
                        { P[i, j] = 1; }//this happens if the guess for x is too big
                        if (P[i, j] < 0)
                        { P[i, j] = dum; }//this happens if the guess for x is too small
                        runningTotal = runningTotal + P[i, j];
                    }
                    double dum2 = P[i, 0];
                    P[i, 0] = 1 - runningTotal;
                    if (P[i, 0] > 1)
                        P[i, 0] = 1;
                    if (P[i, 0] < 0)
                        P[i, 0] = dum2;

                }

                //THROUGHPUT
                for (int i = 0; i <= K; i += 1)
                {
                    for (int r = 1; r <= R; r += 1)
                    {
                        double d = SumR(Rd, K, r) + Rd[i, r];
                        if (d > 0) Xcurr[i, r] = N[r] / (SumR(Rd, K, r) + Rd[i, r]);
                        else Xcurr[i, r] = 0;
                    }
                }

                Error = largestError(Xprev, Xcurr, R);
                if (smallestDiff > Error)
                {
                    smallestDiff = Error;
                    //take a snapshot in case we can't iterate in any further
                    for (int i = 0; i <= K; i += 1)
                    {
                        for (int r = 1; r <= R; r += 1)
                        {
                            Xbest[i, r] = Xcurr[i, r];
                            Rbest[i, r] = Rd[i, r];
                        }
                    }

                }
                for (int i = 0; i <= K; i += 1)
                {
                    for (int r = 1; r <= R; r += 1)
                    {
                        Xprev[i, r] = Xcurr[i, r];
                    }
                }
                if (it > MAXITS) Error = 0;
            }
            if (it > MAXITS)
            {
                //if the # of iterations is huge
                //so take best result
                for (int i = 0; i <= K; i += 1)
                {
                    for (int r = 1; r <= R; r += 1)
                    {
                        Xcurr[i, r] = Xbest[i, r];
                        Rd[i, r] = Rbest[i, r];
                    }
                }
            }
            MVAResponse response = new MVAResponse();
            
            MVAResponseClass[] classArray = new MVAResponseClass[R];
            for (int r = 1; r <= R; r += 1)
            {
                classArray[r - 1] = new MVAResponseClass();
                classArray[r - 1].ClassID = request.Classes[r - 1].ClassID;
                MetricsType metricsR = new MetricsType();
                //this will be the average metrics for each class when averaged
                //over all resources
                double sR = 0;
                double sX = 0;
                double sD = 0;
                for (int k = 1; k <= K; k += 1)
                {
                    sR = sR + Rd[k, r];
                    sX = sX + Xcurr[0, r];
                    sD = sD + D[k, r];
                }
                metricsR.ResponseTime = sR/K;
                metricsR.ServiceDemand = sD / K;
                metricsR.Throughput = sX / K;
                metricsR.Utilization = Convert.ToDecimal(sD / K * sX);
                classArray[r - 1].Metrics = metricsR;
                MVAResponseClassResource[] resourceArray = new MVAResponseClassResource[K + 1];
                for (int k = 0; k <= K; k += 1)
                {
                    resourceArray[k] = new MVAResponseClassResource();
                    resourceArray[k].ResourceID = k;
                    resourceArray[k].Internal = true;//to do
                    MetricsType metricsKR = new MetricsType();
                    metricsKR.ResponseTime = Rd[k, r];
                    metricsKR.ServiceDemand = D[k, r];
                    metricsKR.Throughput = Xcurr[k, r];
                    metricsKR.Utilization = Convert.ToDecimal(D[k, r] * Xcurr[k, r]);
                    resourceArray[k].Metrics = metricsKR;
                }
                classArray[r - 1].Resources = resourceArray;
            }
            response.Classes = classArray;
            return response;
        }

        private double largestError(double[,] Xprev, double[,] Xcurr, int R)
        {
            double err1 = 0;
            for (int r = 1; r <= R; r += 1)
            {
                //double err = (Xcurr[0 , r] - Xprev[0 , r]) / Xprev[0 , r];
                double err = Math.Abs(Xcurr[0, r] - Xprev[0, r]);
                if (Math.Abs(err) > err1) err1 = Math.Abs(err);
            }
            return err1;
        }

        private double largestD(double[,] D, int R, int K)
        {
            double largest = 0;
            for (int r = 1; r <= R; r += 1)
            {
                for (int i = 1; i <= K; i += 1)
                {
                    if (D[i, r] > largest) largest = D[i, r];
                }
            }
            return largest;
        }

        private double SumR(double[,] Rd, int K, int r)
        {
            double sum = 0;
            for (int i = 1; i <= K; i += 1)
            {
                sum += Rd[i, r];
            }
            return sum;
        }

        private double SumPN(int i, int Nbar, double[,] P, int r)
        {
            double sum = 0;
            for (int j = 1; j <= Nbar; j += 1)
            {
                double a = request.Classes[r - 1].Resources[i].LoadRatio;
                sum += (j / a) * P[i, j - 1];
            }
            return sum;
        }

        private double SumDX(int i, int j, double[,] D, int R, double[,] Xprev)
        {
            double sum = 0;

            for (int r = 1; r <= R; r += 1)
            {
                double a = request.Classes[r - 1].Resources[i].LoadRatio;
                sum += D[i, r] * Xprev[0, r] / a ;
            }
            return sum;
        }

        private double GetLargestD(int i, double[,] D, int R)
        {
            double largest = 0;
            for (int r = 1; r <= R; r += 1)
            {
                if (D[i, r] > largest) largest = D[i, r];
            }
            return largest;
        }

    }
}
