using System;
using System.Collections.Generic;

namespace Scallop
{  
  public class Pair<T, U>
  {
    T _head;
    U _rest;
    
    public Pair() : this(default(T), default(U)) { }
    public Pair(T head, U rest)
    {
      _head = head;
      _rest = rest;
    }
    
    public T Head
    {
      get { return _head; }
      set { _head = value; }
    }
    
    public U Rest
    {
      get { return _rest; }
      set { _rest = value; }
    }
    
    public override string ToString()
    {
      return string.Format("({0} {1})", _head, _rest);
    }
  }

  static class Utilities
  {
    public static SExp Eval(SExp[] exps, Environment env)
    {
      Utilities.CheckArguments(exps, args=>0<args.Length, "eval: ");

      SExp res = null;
      foreach (SExp exp in exps)
      {
        res = Eval(exp, env);
      }
      return res;
    }

    public static SExp Eval(SExp exp, Environment env)
    {
      SExp ret = null;
      if (exp is Identifier)
      {
        string name = (exp as Identifier).Value as string;
        bool found = env.Find(name, out ret);
        if (!found || null == ret)
        {
          string msg =
            string.Format("Identifier '{0}' is undefined.", name);
          throw new IdentifierNotDefinedException(msg);
        }
      }
      else if (exp is Atom ||
               exp is Nil ||
               exp is Procedure ||
               exp is Syntax)
      {
        ret = exp;
      }
      // exp is a list
      else
      {
        List<SExp> vals = new List<SExp>();
        for (SExp c = exp; c is Cell; c = (c as Cell).Cdr)
        {
          vals.Add((c as Cell).Car);
        }
        vals[0] = Eval(vals[0], env);

//         if (vals[0] is Atom)
//         {
//           string msg = "Invalid procedure call.";
//           throw new EvalTimeException(msg);
//         }
//         else if (vals[0] is Procedure)
        if (vals[0] is Procedure)
        {
          for (int i=1; i<vals.Count; i++)
          {
            vals[i] = Eval(vals[i], env);
          }

          if (vals[0] is Closure)
          {
            Closure p = vals[0] as Closure;
            vals.RemoveAt(0);
            ret = p.Call(vals.ToArray());
          }
          else
          {
            PrimitiveProcedure p = vals[0] as PrimitiveProcedure;
            vals.RemoveAt(0);
            ret = p.Call(vals.ToArray(), env);
          }
        }
        // syntax
        else
        {
          if (vals[0] is PrimitiveSyntax)
          {
            PrimitiveSyntax s = vals[0] as PrimitiveSyntax;
            List<SExp> tmp = new List<SExp>();
            foreach (SExp e in vals) tmp.Add(e);
            tmp.RemoveAt(0);
            ret = s.Call(tmp.ToArray(), env);
          }
          else
          {
            string name = vals[0].ToString();
            string msg =
              string.Format("User defined syntax was not available yet: '{0}'", name);
            throw new NotSupportedYetException(msg);
          }
        }
      }
      return ret;
    }

    public static bool CanRegardAsTrue(SExp exp)
    {
      bool isFalse =
        exp is Boolean &&
        !((bool)(exp as Boolean).Value);
      return !isFalse;
    }

    public static void CheckArguments(SExp[] vals, Predicate<SExp[]> pred, string procName)
    {      
      if (!pred(vals))
      {
        string msg = procName + "Invalid argument.";
        throw new ArityNotMatchException(msg);
      }
    }
  }
}