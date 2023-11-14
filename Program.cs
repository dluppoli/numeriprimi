using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NumeriPrimi
{
    internal class Program
    {
        private static int maxNumber = 10000000;
        private static int gradoParallelismo = 10;
        static void Main(string[] args)
        {
            //Versione Sincrona
            Stopwatch sw = Stopwatch.StartNew();
            int tot1 = CalcoloSincrono(maxNumber);
            sw.Stop();
            Console.WriteLine($"Sincrono ha calcolato {tot1} e impiegato {sw.ElapsedMilliseconds} ms");
            
            //Versione parallela con Thread
            sw.Restart();
            int tot2 = CalcoloConThread(maxNumber);
            sw.Stop();
            Console.WriteLine($"Thread ha calcolato {tot2} e impiegato {sw.ElapsedMilliseconds} ms");

            //Versione parallela con Task
            sw.Restart();
            int tot3 = CalcoloConTask(maxNumber);
            sw.Stop();
            Console.WriteLine($"Task ha calcolato {tot3} e impiegato {sw.ElapsedMilliseconds} ms");

            //Versione Eratostene
            sw.Restart();
            int tot4 = CalcoloConEratostene(maxNumber); 
            sw.Stop();
            Console.WriteLine($"Eratostene ha calcolato {tot4} e impiegato {sw.ElapsedMilliseconds} ms");
            Console.ReadLine();
        }

        private static int CalcoloSincrono(int stop, int start=1)
        {
            int contatore = 0;
            for(int i = start; i <= stop; i++)
            {
                if( isPrime(i) ) contatore++;
            }
            return contatore;
        }

        private static int CalcoloConThread(int maxNumber)
        {
            //List<Thread> threads = new List<Thread>();
            List<int> risultati = new List<int>();

            //for(int i = 0; i < gradoParallelismo; i++) {
            //    Thread t = new Thread( o => {
            //        int start = (maxNumber / gradoParallelismo) * (int)o + 1;
            //        int stop = (maxNumber / gradoParallelismo) * ((int)o + 1);
            //        risultati.Add(CalcoloSincrono((maxNumber / gradoParallelismo) * ((int)o + 1), (maxNumber / gradoParallelismo) * (int)o + 1));
            //    });
            //    t.Start(i);
            //    threads.Add(t);
            //}

            //foreach(Thread t in threads) t.Join();

            Parallel.For(0, gradoParallelismo, o =>
            {
                risultati.Add(CalcoloSincrono((maxNumber / gradoParallelismo) * (o + 1), (maxNumber / gradoParallelismo) * o + 1));
            });

            return risultati.Sum();
        }

        private static int CalcoloConTask(int maxNumber)
        {
            List<Task<int>> tasks = new List<Task<int>>();

            for(int i = 0; i < gradoParallelismo; i++) {
                int start = (maxNumber / gradoParallelismo) * i + 1;
                int stop = (maxNumber / gradoParallelismo) * (i + 1);
                Task<int> t = Task.Run( () => CalcoloSincrono(stop, start) );
                tasks.Add(t);
            }

            Task.WaitAll(tasks.ToArray());

            return tasks.Sum( t => t.Result );
        }
        private static bool isPrime(int n)
        {
            if( n==1 || n == 2 ) return true;
            if( n<=0 || n%2 == 0) return false;

            for(int i = 3; i*i <= n; i+=2) {
                if (n % i == 0) return false;
            }
            return true;
        }

        private static int CalcoloConEratostene(int maxNumber)
        {
            bool[] numeri = new bool[maxNumber+1];
            numeri[0] = true;

            for(int i = 2; i*i <= maxNumber; i++)
            {
                if (numeri[i] == true) continue;
                for(int j=i*2; j<=maxNumber; j+=i)
                {
                    if (j % i == 0) numeri[j] = true;
                }
            }

            return numeri.ToList().FindAll(x => x == false).Count();
        }
    }

}
