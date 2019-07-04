using System;
using System.Collections.Generic;
using System.Text;

using Qasar.GA;
using Qasar.Mva;

namespace Qasar.Web
{
   
    class TestController
    {

        public bool GATest()
        {
            MVARequest request = getMVARequest();
            MVAResponse response = new MVAResponse();
            double[] targetResponse = new double[request.NoClasses];
            double[] targetX = new double[request.NoClasses];
            targetResponse[0] = 40;
            targetResponse[1] = 60;
            DateTime d1 = DateTime.Now;
            GeneticAlgorithm ga = new GeneticAlgorithm(request, targetResponse, targetX, 1, 0);
            response = ga.GetSolution();
            DateTime d2 = DateTime.Now;
            //without GA Class A av = 52 and Class B av = 75
            //should be able to get around 10 and 49 for the two classes after GA
            if (Convert.ToInt32(response.Classes[0].Metrics.ResponseTime) < 40)
                return true;
            else
                return false;
        }

        public bool LazowskaTest()
        {
            MVARequest request = getMVARequest();
            MVAResponse response = new MVAResponse();
            ApproxMva mva = new ApproxMva();
            response = mva.PerformCalc(request);
            if (Convert.ToInt32(response.Classes[0].Resources[1].Metrics.ResponseTime) == 250)
                return true;
            else
            return false;
        }

        private static MVARequest getMVARequest()
        {
            MVARequest request = new MVARequest();
            request.NoClasses = 2;
            request.NoResources = 5; //don't count the terminal device
            request.NoClients = 1;
            MVARequestClass[] classes = new MVARequestClass[2];
            classes[0] = new MVARequestClass();
            classes[1] = new MVARequestClass();
            classes[0].ClassID = 1;
            classes[1].ClassID = 2;
            classes[0].TotalPopulation = 16; //this is the sum of the equiv loads on each res
            classes[1].TotalPopulation = 40; //for each class not counting the terminal
            for (int j = 0; j < 2; j++)
            {
                MVARequestClassResource[] resources = new MVARequestClassResource[6];
                for (int i = 0; i < 6; i++)
                {
                    resources[i] = new MVARequestClassResource();
                    resources[i].ResourceID = i;
                }
                if (j == 0)
                {
                    resources[0].EquivLoad = 10;
                    resources[0].ServiceTime = 10;
                    resources[0].LoadRatio = 1;
                    resources[1].EquivLoad = 8;
                    resources[1].ServiceTime = 2; ///////////////////
                    resources[1].LoadRatio = 1;
                    resources[2].EquivLoad = 2;
                    resources[2].ServiceTime = 1;
                    resources[2].LoadRatio = 1;
                    resources[3].EquivLoad = 2;
                    resources[3].ServiceTime = 1;
                    resources[3].LoadRatio = 1;
                    resources[4].EquivLoad = 2;
                    resources[4].ServiceTime = 1;
                    resources[4].LoadRatio = 1;
                    resources[5].EquivLoad = 2;
                    resources[5].ServiceTime = 1;
                    resources[5].LoadRatio = 1;
                    classes[0].Resources = resources;
                }
                else
                {
                    resources[0].EquivLoad = 6;
                    resources[0].ServiceTime = 0;
                    resources[0].LoadRatio = 1;
                    resources[1].EquivLoad = 20;
                    resources[1].ServiceTime = 2;
                    resources[1].LoadRatio = 1;
                    resources[2].EquivLoad = 2;
                    resources[2].ServiceTime = 1;
                    resources[2].LoadRatio = 1;
                    resources[3].EquivLoad = 4;
                    resources[3].ServiceTime = 1;
                    resources[3].LoadRatio = 1;
                    resources[4].EquivLoad = 6;
                    resources[4].ServiceTime = 1;
                    resources[4].LoadRatio = 1;
                    resources[5].EquivLoad = 8;
                    resources[5].ServiceTime = 1;
                    resources[5].LoadRatio = 1;
                    classes[1].Resources = resources;
                }

            }
            request.Classes = classes;
            MVARequestClient[] cl = new MVARequestClient[1];
            cl[0] = new MVARequestClient();
            cl[0].ClientID = 1;
            request.Clients = cl;
            return request;
        }
    }
}
