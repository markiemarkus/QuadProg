using System;
using System.Diagnostics;
using CommonLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class Exacta_Integrator
    {
        [TestMethod]
        public void TestMethod1()
        {
        }
    }
    public struct Race
    {
        public int RaceID;
        public int First;
        public int Second;
        public uint EvalCount;
        //public TimeSpan Duration;
        public double ExactaPool;
        public Logistic[] Dists;
        public GridCell[] ExactaOdds;
        public double PublicScore;
        public double NormalScore;
        public double RobustScore;

        // Perfrom a simple normalization on the exacta odds:
        public GridCell[] GetExactaProbs()
        {
            double sum = 0.0;
            GridCell[] probs = new GridCell[ExactaOdds.Length];
            for (int idx = 0; idx < ExactaOdds.Length; idx++)
            {
                probs[idx].Runner1 = ExactaOdds[idx].Runner1;
                probs[idx].Runner2 = ExactaOdds[idx].Runner2;
                sum += probs[idx].Value = 1.0 / ExactaOdds[idx].Value;
            }

            for (int idx = 0; idx < probs.Length; idx++)
                probs[idx].Value /= sum;

            return probs;
        }
    }

    [TestClass]
    public static class Exacta_Integrator_Test
	{
		const double _Precision = 1.0e-18;
		const double _ExactaRegFactor = 0.0;

        [TestMethod]
		public static void TestProbs(Race[] races )
		{
			long elapsed = 0;
			long sum_as_times = 0L;
			long min_as_times = Int64.MaxValue;
			long max_as_times = Int64.MinValue;
			long sum_de_times = 0L;
			long min_de_times = Int64.MaxValue;
			long max_de_times = Int64.MinValue;
			ulong sum_evals = 0L;
			uint min_evals = UInt32.MaxValue;
			uint max_evals = UInt32.MinValue;
			double sum_mse = 0.0;
			double max_errs = 0.0;

			double dummy = LogisticGrid.CalcExactaProbs( races[0].Dists )[0].Value;

			Stopwatch timer = new Stopwatch();
			for( int idx = 0; idx < races.Length; idx++ )
			{
				timer.Restart();
				GridCell[] as_cells = LogisticGrid.CalcExactaProbsAS( races[idx].Dists, _Precision, out races[idx].EvalCount );
				timer.Stop();

				//races[idx].Duration = timer.Elapsed;
				elapsed = timer.ElapsedTicks;
				sum_as_times += elapsed;
				if( elapsed < min_as_times )
					min_as_times = elapsed;
				if( elapsed > max_as_times )
					max_as_times = elapsed;

				timer.Restart();
				GridCell[] de_cells = LogisticGrid.CalcExactaProbs( races[idx].Dists );
				timer.Stop();

				//races[idx].Duration = timer.Elapsed;
				elapsed = timer.ElapsedTicks;
				sum_de_times += elapsed;
				if( elapsed < min_de_times )
					min_de_times = elapsed;
				if( elapsed > max_de_times )
					max_de_times = elapsed;

				double sumsq = 0.0;
				double max_err = 0.0;
				for( int cell = 0; cell < de_cells.Length; cell++ )
				{
					double err = de_cells[cell].Value-as_cells[cell].Value;
					sumsq += err*err;
					if( Math.Abs( err ) > max_err )
						max_err = Math.Abs( err );
				}
				sumsq = Math.Sqrt( sumsq );
				sum_mse += sumsq;
				if( max_err > max_errs )
					max_errs = max_err;

				sum_evals += (ulong)races[idx].EvalCount;
				if( races[idx].EvalCount < min_evals )
					min_evals = races[idx].EvalCount;
				if( races[idx].EvalCount > max_evals )
					max_evals = races[idx].EvalCount;
			}
			double avg_as_times = ((double)sum_as_times)/((double)TimeSpan.TicksPerMillisecond)/(double)races.Length;
			double avg_de_times = ((double)sum_de_times)/((double)TimeSpan.TicksPerMillisecond)/(double)races.Length;
			double avg_evals = ((double)sum_evals)/(double)races.Length;
			double avg_mse = ((double)sum_mse)/(double)races.Length;

			Console.WriteLine( "{0:D} Races, Precision={1:G}", races.Length, _Precision, dummy );
			Console.WriteLine( "Times:" );
			Console.WriteLine( "   Adaptive Simpson:   Avg={0:F3}ms, Min={1:F3}ms, Max={2:F3}ms", avg_as_times,
				((double)min_as_times)/((double)TimeSpan.TicksPerMillisecond),
				((double)max_as_times)/((double)TimeSpan.TicksPerMillisecond) );
			Console.WriteLine( "   Double Exponential: Avg={0:F3}ms, Min={1:F3}ms, Max={2:F3}ms", avg_de_times,
				((double)min_de_times)/((double)TimeSpan.TicksPerMillisecond),
				((double)max_de_times)/((double)TimeSpan.TicksPerMillisecond) );
			Console.WriteLine( "   Ratio:              Avg={0:F3},  Min={1:F3},  Max={2:F3}", avg_as_times/avg_de_times,
				((double)min_as_times)/((double)min_de_times), ((double)max_as_times)/((double)max_de_times) );
			Console.WriteLine( "Evals: Avg={0:F3}, Min={1:D}, Max={2:D}", avg_evals, min_evals, max_evals );
			Console.WriteLine( "Errors: Avg={0:G9}, Max={1:G9}", avg_mse, max_errs );
		}

        [TestMethod]
		public static void TestGrads( Race[] races )
		{
			long elapsed = 0;
			long sum_as_times = 0L;
			long min_as_times = Int64.MaxValue;
			long max_as_times = Int64.MinValue;
			long sum_de_times = 0L;
			long min_de_times = Int64.MaxValue;
			long max_de_times = Int64.MinValue;
			ulong sum_evals = 0L;
			uint min_evals = UInt32.MaxValue;
			uint max_evals = UInt32.MinValue;
			double sum_prob_mse = 0.0;
			double sum_loc_mse = 0.0;
			double sum_scl_mse = 0.0;
			double max_prob_err = 0.0;
			double max_loc_err = 0.0;
			double max_scl_err = 0.0;

			double dummy = LogisticGrid.CalcExactaProbs( races[0].Dists )[0].Value;

			Stopwatch timer = new Stopwatch();
			for( int idx = 0; idx < races.Length; idx++ )
			{
				timer.Restart();
				GridCellGradients[] as_cells = LogisticGrid.CalcExactaGradsAS( races[idx].Dists, _Precision );
				timer.Stop();

				//races[idx].Duration = timer.Elapsed;
				elapsed = timer.ElapsedTicks;
				sum_as_times += elapsed;
				if( elapsed < min_as_times )
					min_as_times = elapsed;
				if( elapsed > max_as_times )
					max_as_times = elapsed;

				timer.Restart();
				GridCellGradients[] de_cells = LogisticGrid.CalcExactaGrads( races[idx].Dists );
				timer.Stop();

				//races[idx].Duration = timer.Elapsed;
				elapsed = timer.ElapsedTicks;
				sum_de_times += elapsed;
				if( elapsed < min_de_times )
					min_de_times = elapsed;
				if( elapsed > max_de_times )
					max_de_times = elapsed;

				double sumsq_prob = 0.0;
				double sumsq_loc = 0.0;
				double sumsq_scl = 0.0;
				double max_prob_diff = 0.0;
				double max_loc_diff = 0.0;
				double max_scl_diff = 0.0;
				for( int cell = 0; cell < de_cells.Length; cell++ )
				{
					double err = de_cells[cell].Value-as_cells[cell].Value;
					sumsq_prob += err*err;
					if( Math.Abs( err ) > max_prob_diff )
						max_prob_diff = Math.Abs( err );

					for( int grad = 0; grad < de_cells[cell].LocationGrads.Length; grad++ )
					{
						err = Math.Abs( de_cells[cell].LocationGrads[grad] - as_cells[cell].LocationGrads[grad] );
						sumsq_loc += err*err;
						if( Math.Abs( err ) > max_loc_diff )
							max_loc_diff = Math.Abs( err );

						err = Math.Abs( de_cells[cell].ScaleGrads[grad] - as_cells[cell].ScaleGrads[grad] );
						sumsq_scl += err*err;
						if( Math.Abs( err ) > max_scl_diff )
							max_scl_diff = Math.Abs( err );
					}
				}
				sumsq_prob = Math.Sqrt( sumsq_prob );
				sum_prob_mse += sumsq_prob;
				if( max_prob_diff > max_prob_err )
					max_prob_err = max_prob_diff;

				sumsq_loc = Math.Sqrt( sumsq_loc );
				sum_loc_mse += sumsq_loc/(double)races[idx].Dists.Length;
				if( max_loc_diff > max_loc_err )
					max_loc_err = max_loc_diff;

				sumsq_scl = Math.Sqrt( sumsq_scl );
				sum_scl_mse += sumsq_scl/(double)races[idx].Dists.Length;
				if( max_scl_diff > max_scl_err )
					max_scl_err = max_scl_diff;

				sum_evals += (ulong)races[idx].EvalCount;
				if( races[idx].EvalCount < min_evals )
					min_evals = races[idx].EvalCount;
				if( races[idx].EvalCount > max_evals )
					max_evals = races[idx].EvalCount;
			}
			double avg_as_times = ((double)sum_as_times)/((double)TimeSpan.TicksPerMillisecond)/(double)races.Length;
			double avg_de_times = ((double)sum_de_times)/((double)TimeSpan.TicksPerMillisecond)/(double)races.Length;
			double avg_evals = ((double)sum_evals)/(double)races.Length;
			double avg_prob_mse = ((double)sum_prob_mse)/(double)races.Length;
			double avg_loc_mse = ((double)sum_loc_mse)/(double)races.Length;
			double avg_scl_mse = ((double)sum_scl_mse)/(double)races.Length;

			Console.WriteLine( "{0:D} Races, Precision={1:G}", races.Length, _Precision, dummy );
			Console.WriteLine( "Times:" );
			Console.WriteLine( "   Adaptive Simpson:   Avg={0:F3}ms, Min={1:F3}ms, Max={2:F3}ms", avg_as_times,
				((double)min_as_times)/((double)TimeSpan.TicksPerMillisecond),
				((double)max_as_times)/((double)TimeSpan.TicksPerMillisecond) );
			Console.WriteLine( "   Double Exponential: Avg={0:F3}ms, Min={1:F3}ms, Max={2:F3}ms", avg_de_times,
				((double)min_de_times)/((double)TimeSpan.TicksPerMillisecond),
				((double)max_de_times)/((double)TimeSpan.TicksPerMillisecond) );
			Console.WriteLine( "   Ratio:              Avg={0:F3},  Min={1:F3},  Max={2:F3}", avg_as_times/avg_de_times,
				((double)min_as_times)/((double)min_de_times), ((double)max_as_times)/((double)max_de_times) );
			Console.WriteLine( "Errors:" );
			Console.WriteLine( "   Probs:    Avg={0:G9}, Max={1:G9}", avg_prob_mse, max_prob_err );
			Console.WriteLine( "   Location: Avg={0:G9}, Max={1:G9}", avg_loc_mse, max_loc_err );
			Console.WriteLine( "   Scale:    Avg={0:G9}, Max={1:G9}", avg_scl_mse, max_scl_err );
			//Console.WriteLine( "Evals: Avg={0:F3}, Min={1:D}, Max={2:D}", avg_evals, min_evals, max_evals );
		}

        [TestMethod]
		public static void TestInversion( Race[] races )
		{
			long elapsed_as = 0;
			long elapsed_de = 0;
			long sum_as_times = 0L;
			long min_as_times = Int64.MaxValue;
			long max_as_times = Int64.MinValue;
			long sum_de_times = 0L;
			long min_de_times = Int64.MaxValue;
			long max_de_times = Int64.MinValue;
			//ulong sum_evals = 0L;
			//uint min_evals = UInt32.MaxValue;
			//uint max_evals = UInt32.MinValue;
			//double sum_prob_mse = 0.0;
			//double sum_loc_mse = 0.0;
			//double sum_scl_mse = 0.0;
			//double max_prob_err = 0.0;
			//double max_loc_err = 0.0;
			//double max_scl_err = 0.0;
			int termination_code, no_of_iterations, no_of_func_evals;

			Stopwatch timer = new Stopwatch();
			for( int idx = 0; idx < races.Length; idx++ )
			{
				timer.Restart();
				Logistic[] as_dists = LogisticGrid.InvertExactasRobust( races[idx].ExactaOdds, _Precision, 0.9,
					out no_of_iterations, out no_of_func_evals, out termination_code );
				timer.Stop();

				//races[idx].Duration = timer.Elapsed;
				elapsed_as = timer.ElapsedTicks;
				sum_as_times += elapsed_as;
				if( elapsed_as < min_as_times )
					min_as_times = elapsed_as;
				if( elapsed_as > max_as_times )
					max_as_times = elapsed_as;

				timer.Restart();
				Logistic[] de_dists = LogisticGrid.InvertExactas( races[idx].ExactaOdds, _ExactaRegFactor, _Precision,
					out no_of_iterations, out no_of_func_evals, out termination_code );
				timer.Stop();

				//races[idx].Duration = timer.Elapsed;
				elapsed_de = timer.ElapsedTicks;
				sum_de_times += elapsed_de;
				if( elapsed_de < min_de_times )
					min_de_times = elapsed_de;
				if( elapsed_de > max_de_times )
					max_de_times = elapsed_de;

				//double sumsq_prob = 0.0;
				//double sumsq_loc = 0.0;
				//double sumsq_scl = 0.0;
				//double max_prob_diff = 0.0;
				//double max_loc_diff = 0.0;
				//double max_scl_diff = 0.0;
				//for( int cell = 0; cell < de_dists.Length; cell++ )
				//{
				//	double err = de_dists[cell].Value-as_dists[cell].Value;
				//	sumsq_prob += err*err;
				//	if( Math.Abs( err ) > max_prob_diff )
				//		max_prob_diff = Math.Abs( err );

				//	for( int grad = 0; grad < de_dists[cell].LocationGrads.Length; grad++ )
				//	{
				//		err = Math.Abs( de_dists[cell].LocationGrads[grad] - as_dists[cell].LocationGrads[grad] );
				//		sumsq_loc += err*err;
				//		if( Math.Abs( err ) > max_loc_diff )
				//			max_loc_diff = Math.Abs( err );

				//		err = Math.Abs( de_dists[cell].ScaleGrads[grad] - as_dists[cell].ScaleGrads[grad] );
				//		sumsq_scl += err*err;
				//		if( Math.Abs( err ) > max_scl_diff )
				//			max_scl_diff = Math.Abs( err );
				//	}
				//}
				//sumsq_prob = Math.Sqrt( sumsq_prob );
				//sum_prob_mse += sumsq_prob;
				//if( max_prob_diff > max_prob_err )
				//	max_prob_err = max_prob_diff;

				//sumsq_loc = Math.Sqrt( sumsq_loc );
				//sum_loc_mse += sumsq_loc/(double)races[idx].Dists.Length;
				//if( max_loc_diff > max_loc_err )
				//	max_loc_err = max_loc_diff;

				//sumsq_scl = Math.Sqrt( sumsq_scl );
				//sum_scl_mse += sumsq_scl/(double)races[idx].Dists.Length;
				//if( max_scl_diff > max_scl_err )
				//	max_scl_err = max_scl_diff;

				//sum_evals += (ulong)races[idx].EvalCount;
				//if( races[idx].EvalCount < min_evals )
				//	min_evals = races[idx].EvalCount;
				//if( races[idx].EvalCount > max_evals )
				//	max_evals = races[idx].EvalCount;

				Console.WriteLine( "{0:D7}: AS={1:F3}ms, DE={2:F3}ms, Ratio={3:F3}", races[idx].RaceID,
					((double)elapsed_as)/((double)TimeSpan.TicksPerMillisecond),
					((double)elapsed_de)/((double)TimeSpan.TicksPerMillisecond),
					((double)elapsed_as)/((double)elapsed_de) );
			}
			double avg_as_times = ((double)sum_as_times)/((double)TimeSpan.TicksPerMillisecond)/(double)races.Length;
			double avg_de_times = ((double)sum_de_times)/((double)TimeSpan.TicksPerMillisecond)/(double)races.Length;
			//double avg_evals = ((double)sum_evals)/(double)races.Length;
			//double avg_prob_mse = ((double)sum_prob_mse)/(double)races.Length;
			//double avg_loc_mse = ((double)sum_loc_mse)/(double)races.Length;
			//double avg_scl_mse = ((double)sum_scl_mse)/(double)races.Length;

			Console.WriteLine( "{0:D} Races, Precision={1:G}", races.Length, _Precision );
			Console.WriteLine( "Times:" );
			Console.WriteLine( "   Adaptive Simpson:   Avg={0:F3}ms, Min={1:F3}ms, Max={2:F3}ms", avg_as_times,
				((double)min_as_times)/((double)TimeSpan.TicksPerMillisecond),
				((double)max_as_times)/((double)TimeSpan.TicksPerMillisecond) );
			Console.WriteLine( "   Double Exponential: Avg={0:F3}ms, Min={1:F3}ms, Max={2:F3}ms", avg_de_times,
				((double)min_de_times)/((double)TimeSpan.TicksPerMillisecond),
				((double)max_de_times)/((double)TimeSpan.TicksPerMillisecond) );
			Console.WriteLine( "   Ratio:              Avg={0:F3},  Min={1:F3},  Max={2:F3}", avg_as_times/avg_de_times,
				((double)min_as_times)/((double)min_de_times), ((double)max_as_times)/((double)max_de_times) );
			//Console.WriteLine( "Errors:" );
			//Console.WriteLine( "   Probs:    Avg={0:G9}, Max={1:G9}", avg_prob_mse, max_prob_err );
			//Console.WriteLine( "   Location: Avg={0:G9}, Max={1:G9}", avg_loc_mse, max_loc_err );
			//Console.WriteLine( "   Scale:    Avg={0:G9}, Max={1:G9}", avg_scl_mse, max_scl_err );
		}
	}
}


