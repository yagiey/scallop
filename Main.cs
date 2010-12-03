using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Entry = Scallop.Pair<string, Scallop.SExp>;

namespace Scallop
{
  class Program
  {
    public static void Main(string[] args)
    {
      Processor p = new Processor();
      p.RunREPL(Console.ReadLine, Console.Write);
    }
  }
}
