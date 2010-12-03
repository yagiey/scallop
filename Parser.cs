using System;
using System.Collections.Generic;

namespace Scallop
{
  class Parser
  {
    public Parser() { }

    public SExp[] Parse(IEnumerable<LexerToken> tokens)
    {
      IEnumerator<LexerToken> itor = tokens.GetEnumerator();
      if (itor.MoveNext())
      {
        return CreateExpressions(tokens);
      }

      const string MSG = "Empty S-Expression.";
      throw new ParserException(MSG);
    }

    SExp CreateAtom(LexerToken token)
    {
      SExp atom = null;
      if (token is TokenLeftParen)
      {
        atom = new __LeftParen();
      }
      else if (token is TokenInteger)
      {
        int n = 0;
        int.TryParse(token.Value, out n);
        atom = new Integer(n);
      }
      else if (token is TokenString)
      {
        atom = new String(token.Value);
      }
      else if (token is TokenIdentifier)
      {
        atom = new Identifier(token.Value);
      }
      else
      {
        string msg =
          string.Format("failed to create atom from token '{0}'.", token.Value);
        throw new ParserException(msg);
      }
      return atom;
    }

    public SExp[] CreateExpressions(IEnumerable<LexerToken> tokens)
    {
      IEnumerator<LexerToken> itor = tokens.GetEnumerator();

      // confirm
      if (!itor.MoveNext())
      {
        const string MSG = "Empty S-Expression.";
        throw new ParserException(MSG);
      }

      itor = tokens.GetEnumerator();
      Stack<SExp> stkExp = new Stack<SExp>();
      SExp list = new Nil();
      bool consing = false;

      while (true)
      {
        if (consing)
        {
          SExp exp = null;
          try { exp = stkExp.Pop(); }
          catch
          {
            const string MSG = "Extra close parenthesis.";
            throw new ParserException(MSG);
          }

          if (exp is __LeftParen)
          {
            stkExp.Push(list);
            list = new Nil();
            consing = false;
          }
          else
          {
            list = new Cell(exp, list);
          }
        }
        else
        {
          if (!itor.MoveNext())
          {
            break;
          }
          else if (itor.Current is TokenRightParen)
          {
            consing = true;
          }
          else
          {
            SExp exp = CreateAtom(itor.Current);
            stkExp.Push(exp);
          }
        }

      }

      List<SExp> l = new List<SExp>(stkExp);
      l.Reverse();
      if (l.Exists(exp=>exp is __LeftParen))
      {
        string msg = "Unclosed list detected.";
        throw new ParserException(msg);
      }
      return l.ToArray();
    }
  }
}
