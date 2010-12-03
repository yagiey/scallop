using System.Collections.Generic;
using System.Linq;
using Entry = Scallop.Pair<string, Scallop.SExp>;

namespace Scallop
{
  static class Syntaxes
  {
    public static SExp Define(SExp[] vals, Environment env)
    {
      Utilities.CheckArguments(vals, args=>args.Length==2, "define: ");
      SExp name = vals[0];
      if (!(name is Identifier))
      {
        string msg = "define: The first argument must be identifier.";
        throw new TypeNotMatchException(msg);
      }

      SExp exp = Utilities.Eval(vals[1], env);

      env.Add(new Entry((name as Identifier).Value as string, exp));
      return name;
    }

    public static SExp Lambda(SExp[] vals, Environment env)
    {
      Utilities.CheckArguments(vals, args=>2<=args.Length, "lambda: ");
      SExp formal = vals[0];
      if (formal is Atom)
      {
        if (formal is Identifier)
        {
          string msg = "lambda: Variable-length argument is not supported yet.";
          throw new NotSupportedYetException(msg);
        }
        else
        {
          string msg = "lambda: Formal parameter must be pair or identifier.";
          throw new TypeNotMatchException(msg);
        }
      }

      List<string> f = new List<string>();
      for (SExp c = formal; c is Cell; c = (c as Cell).Cdr)
      {
        if (!((c as Cell).Car is Identifier))
        {
          string msg = "lambda: All parameters must be identifier.";
          throw new TypeNotMatchException(msg);
        }
        f.Add(((c as Cell).Car as Identifier).Value as string);
      }

      return
        new Closure(f.ToArray(), Enumerable.Skip(vals,1).ToArray(), env);
    }

    public static SExp Let(SExp[] vals, Environment env)
    {
      Utilities.CheckArguments(vals, args=>2<=args.Length, "let: ");
      EnvironmentFrame frame = null;
      try
      {
        frame = EnvironmentFrame.FromSExpression(vals[0]);
        foreach (Entry entry in frame)
        {
          entry.Rest = Utilities.Eval(entry.Rest, env);
        }
      }
      catch (TypeNotMatchException)
      {
        string msg = "let: malformed init list.";
        throw new TypeNotMatchException(msg);
      }

      SExp[] body = Enumerable.Skip(vals, 1).ToArray();
      return Closure.Eval(env, frame, body);
    }

    public static SExp Quote(SExp[] vals, Environment env)
    {
      Utilities.CheckArguments(vals, args=>args.Length==1, "quote: ");
      return vals[0];
    }

    public static SExp If(SExp[] vals, Environment env)
    {
      Utilities.CheckArguments(vals, args=>args.Length==3, "if: ");
      SExp test = vals[0];
      SExp case1 = vals[1];
      SExp case2 = vals[2];
      SExp ret = null;
      if (Utilities.CanRegardAsTrue(Utilities.Eval(test, env)))
      {
        ret = Utilities.Eval(case1, env);
      }
      else
      {
        ret = Utilities.Eval(case2, env);
      }
      return ret;
    }

    public static SExp Cond(SExp[] vals, Environment env)
    {
      SExp res = new Undefined();
      foreach (SExp exp in vals)
      {
        if (!(exp is Cell))
        {
          string msg = "cond: malformed clauses.";
          throw new TypeNotMatchException(msg);
        }

        SExp test = (exp as Cell).Car;

        if (!((exp as Cell).Cdr is Cell))
        {
          string msg = "cond: malformed clauses.";
          throw new TypeNotMatchException(msg);
        }

        SExp[] body = ((exp as Cell).Cdr as Cell).ToArray();

        if (Utilities.CanRegardAsTrue(Utilities.Eval(test, env)))
        {
          res = Utilities.Eval(body, env);
          break;
          
        }
      }
      return res;
    }

    public static SExp Or(SExp[] vals, Environment env)
    {
      SExp res = null;
      foreach (SExp exp in vals)
      {
        res = Utilities.Eval(exp, env);
        if (Utilities.CanRegardAsTrue(res)) return res;
      }
      return new Boolean(false);
    }

    public static SExp And(SExp[] vals, Environment env)
    {
      SExp res = new Boolean(true);
      foreach (SExp exp in vals)
      {
        res = Utilities.Eval(exp, env);
        if (!Utilities.CanRegardAsTrue(res)) return new Boolean(false);
      }
      return res;
    }

    public static SExp Setq(SExp[] vals, Environment env)
    {
      Utilities.CheckArguments(vals, args=>args.Length==2, "set!: ");
      if (!(vals[0] is Identifier))
      {
        string msg = "The first argument must be identifier.";
        throw new TypeNotMatchException(msg);
      }
 
      Identifier name = vals[0] as Identifier;
      SExp val = Utilities.Eval(vals[1], env);
      if (!env.Reassign(name.Value as string, val))
      {
        string msg =
          string.Format("Identifier '{0}' is undefined.", name);
        throw new IdentifierNotDefinedException(msg);
      }
      return name;
    }
  }
}
