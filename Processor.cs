using System;
using System.Collections.Generic;
using System.Linq;

namespace Scallop
{
  class Processor
  {
    Environment _env;

    Lexer _lexer;

    Parser _parser;

    const string _PROMPT = "scallop> ";

    public Processor()
    {
      _env = Environment.MakeInitialEnvironment();
      _lexer = new Lexer(ch => ch == ' ' || ch == '\t');
      _parser = new Parser();
    }

    public void RunREPL(Func<string> read, Action<string> write)
    {
      while (true)
      {
        string line = Prompt(read, write, _PROMPT);
        IEnumerable<SExp> exps = null;
        try
        {
          exps = _parser.Parse(_lexer.EnumerateTokens(line));
        }
        catch (LexerException e)
        {
          write(e.Message + "\n");
          continue;
        }
        catch (ParserException e)
        {
          write(e.Message + "\n");
          continue;
        }

        SExp result = null;
        try { result = Utilities.Eval(exps.ToArray(), _env); }
        catch (ApplicationTerminationException e)
        {
          write(e.Message + "\n");
          break;
        }
        catch (Exception e)
        {
          write(e.Message + "\n");
          continue;
        }
        catch (System.Exception e)
        {
          write(e.Message + "\n");
          continue;
        }

        write(result + "\n");
      }
    }

    public static string Prompt(Func<string> read, Action<string> write, string msg)
    {
      write(msg);
      return read();
    }
  }
}