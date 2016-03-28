using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace YHaplogroup
{
	class Program
	{
		static void Main(string[] args)
		{
			List<string> haplogroup = new List<string>();       //list of haplogroups that were expressed in Y chromosome
			string[] h;                                         //list of most specific haplogroups
			int[] pChromo;                                      //pointer to beginning of chromosomes
			bool[] ignore;                                      //corresponds to found haplogroups that may be ignored
			string[,] d1 = ReadDNAFile(@"../../DNAS.txt", out pChromo);             //dna file
			string[,] haplogroups = ReadHaplotypeFile(@"../../haplogroups.txt");    //haplogroups to search for
			for (int i = pChromo[23]; i < pChromo[24]; i++)             //look at Y chromosome
			{
				for (int j = 0; j < haplogroups.GetUpperBound(0); j++)  //iterate through the haplogroups
				{
					if (d1[i, 3] == haplogroups[j, 2])                  //position matches known haplogroup
					{
						if (d1[i, 1] == haplogroups[j, 3])              //mutation matches haplogroup
						{
							if (!haplogroup.Contains(haplogroups[j, 0]))
							{
								haplogroup.Add(haplogroups[j, 0]);
							}
						}
					}
				}
			}
			h = haplogroup.ToArray();                           //put haplogroups into array to maintain an index
			ignore = new bool[h.Length];
			for (int x = 0; x < h.Length; x++)                  //compare each pair of haplogroups once
			{
				for (int y = x + 1; y < h.Length; y++)
				{
					if (h[x]==null || h[y] == null || h[x].Length == h[y].Length)  //if haplogroup is less specific, or haplogroups have same number of characters
					{
						continue;                               //then cannot determine specificity
					}
					if (h[x].Length < h[y].Length)              //if lengths differ, begin looking at characters to see if they belong to same branch
					{
						bool morespecific = true;               //set to false when a character doesn't match, indicating two different branches
						for (int i = 0; i < h[x].Length; i++)   //iterate the chars, looking for a mismatch
						{
							if(h[x][i] != h[y][i])
							{
								morespecific = false;           //if mismatch is found, then the longer haplogroup is not more specific
							}
							
						}
						if (morespecific == true)               //if mismatch is not found, then the longer haplogroup is more specific
						{
							h[x] = null;                   //ignore the shorter haplogroup
						}
					}
					else                                        //the second haplogroup is longer, compare chars and look for a mismatch
					{
						bool morespecific = true;
						for (int i = 0; i < h[y].Length; i++)
						{
							if (h[x][i] != h[y][i])
							{
								morespecific = false;
							}

						}
						if (morespecific == true)
						{
							h[y] = null;
						}
					}
				}
			}
		}


		public static string[,] ReadHaplotypeFile(string filePath)
		{
			string[,] output = new string[1333, 4];    //preallocated array for Haplogroups
			string theLine = "";
			int x = 0;
			try
			{
				using (StreamReader sr = new StreamReader(filePath))
				{
					string[] fields;
					while (theLine != null)     //infinite loop with counter x
					{
						theLine = sr.ReadLine();
						if (theLine != null && theLine[0] != '#' && theLine[2] != 'i')
						{
							fields = theLine.Split(new char[] { '\t' });
							output[x, 0] = fields[0];               //group
							output[x, 1] = fields[1];               //rsid
							output[x, 2] = fields[2];               //position
							output[x, 3] = fields[3];               //mutation

							x++;
						}
					}
				}
			}
			catch (IOException e)
			{
				Console.WriteLine("The file could not be read: ");
				Console.WriteLine(e.Message);
			}

			return output;
		}


		public static string[,] ReadDNAFile(string filePath, out int[] chromoPointers)
		{
			chromoPointers = new int[26];                      //pointers to beginning of each chromosome (25+ endPointer)
			int x = 0, i = 0;                             //pointer in string[], and int[] chromoP
			string[,] output = new string[701478, 4];    //preallocated array for ChrisDNA.txt
			string theLine = "";

			try
			{
				using (StreamReader sr = new StreamReader(filePath))
				{
					string[] fields;
					while (theLine != null)     //infinite loop with counter x
					{
						theLine = sr.ReadLine();
						if (theLine != null && theLine[0] != '#' && theLine[2] != 'i')
						{
							fields = theLine.Split(new char[] { '\t' });
							output[x, 0] = fields[1];                //chromosome Number
							output[x, 3] = fields[2];               //position
							output[x, 1] = fields[3];               //Strand 1
							output[x, 2] = fields[4];               //strand 2
							/*This if statement saves a pointer to the beginning of every chromosome.
								Allows comparison of chromosome lengths. */
							if (x == 0)
							{
								chromoPointers[0] = 0;
							}
							else if (output[x, 0] != output[x - 1, 0])
							{
								chromoPointers[++i] = x;
							}
							x++;
						}
					}
					chromoPointers[chromoPointers.GetUpperBound(0)] = x;

				}


			}
			catch (IOException e)
			{
				Console.WriteLine("The file could not be read: ");
				Console.WriteLine(e.Message);
			}

			return output;
		}
	}
}
