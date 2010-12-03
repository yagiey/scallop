using System.Collections.Generic;

namespace Scallop
{
  static class Procedures
  {
    public static SExp Eval(SExp[] vals, Environment env)
    {
      Utilities.CheckArguments(vals, args=>args.Length==2, "eval: ");
      SExp body = vals[0];
      Environment envGiven = Environment.FromSExpression(vals[1]);
      return Utilities.Eval(body, envGiven);
    }

    public static SExp InteractionEnvironment(SExp[] vals, Environment env)
    {
      Utilities.CheckArguments(vals, args=>args.Length==0, "interaction-environment: ");
      return env.ToSExpression();
    }

    public static SExp Exit(SExp[] vals, Environment env)
    {
      Utilities.CheckArguments(vals, args=>args.Length==0, "exit: ");
      string msg = "Scalop was terminated.";
      throw new ApplicationTerminationException(msg);
    }

    public static SExp Cons(SExp[] vals, Environment env)
    {
      Utilities.CheckArguments(vals, args=>args.Length==2, "cons: ");
      SExp car = vals[0];
      SExp cdr = vals[1];
      return new Cell(car, cdr);
    }

    public static SExp Car(SExp[] vals, Environment env)
    {
      Utilities.CheckArguments(vals, args=>args.Length==1, "car: ");
      SExp cell = vals[0];
      if (!(cell is Cell))
      {
        string msg = "car: Parameter must be pair.";
        throw new TypeNotMatchException(msg);
      }
      return (cell as Cell).Car;
    }

    public static SExp Cdr(SExp[] vals, Environment env)
    {
      Utilities.CheckArguments(vals, args=>args.Length==1, "cdr: ");
      SExp cell = vals[0];
      if (!(cell is Cell))
      {
        string msg = "cdr: Parameter must be pair.";
        throw new TypeNotMatchException(msg);
      }
      return (cell as Cell).Cdr;
    }

    public static SExp Eq(SExp[] vals, Environment env)
    {
      Utilities.CheckArguments(vals, args=>args.Length==2, "eq: ");
      SExp lhs = vals[0];
      SExp rhs = vals[1];
      return new Boolean(lhs == rhs);
    }

    public static SExp Atom(SExp[] vals, Environment env)
    {
      Utilities.CheckArguments(vals, args=>args.Length==1, "atom?: ");
      SExp exp = vals[0];
      return new Boolean(exp is Atom);
    }

    public static SExp Not(SExp[] vals, Environment env)
    {
      Utilities.CheckArguments(vals, args=>args.Length==1, "not: ");
      SExp exp = vals[0];
      return
        Utilities.CanRegardAsTrue(exp) ?
        Boolean.FALSE :
        Boolean.TRUE;
    }

    public static SExp Null(SExp[] vals, Environment env)
    {
      Utilities.CheckArguments(vals, args=>args.Length==1, "null?: ");
      SExp exp = vals[0];
      return
        exp is Nil ?
        Boolean.TRUE :
        Boolean.FALSE;
    }

    public static SExp Add(SExp[] vals, Environment env)
    {
      int sum = 0;
      foreach (SExp exp in vals)
      {
        if (!(exp is Integer))
        {
          string msg = "+: Parameters must be numeric value.";
          throw new TypeNotMatchException(msg);
        }
        sum += ((int)(exp as Integer).Value);
      }

      return new Integer(sum);
    }

    public static SExp Sub(SExp[] vals, Environment env)
    {
      if (vals.Length == 0)
      {
        string msg = "-: Procedure require at least one argument.";
        throw new ArityNotMatchException(msg);
      }

      else
      {
        SExp exp = vals[0];
        if (!(exp is Integer))
        {
          string msg = "-: Parameters must be numeric value.";
          throw new TypeNotMatchException(msg);
        }
        int n = (int)(exp as Integer).Value;

        if (vals.Length == 1)
        {
          n = -n;
        }
        else
        {
          for (int i = 1; i < vals.Length; i++)
          {
            if (!(vals[i] is Integer))
            {
              string msg = "-: Parameters must be numeric value.";
              throw new TypeNotMatchException(msg);
            }
            n -= ((int)(vals[i] as Integer).Value);
          }
        }
        return new Integer(n);
      }
    }

    public static SExp Mul(SExp[] vals, Environment env)
    {
      int n = 1;
      foreach (SExp exp in vals)
      {
        if (!(exp is Integer))
        {
          string msg = "*: Parameters must be numeric value.";
          throw new TypeNotMatchException(msg);
        }
        n *= ((int)(exp as Integer).Value);
      }

      return new Integer(n);
    }

    public static SExp LessThan(SExp[] vals, Environment env)
    {
      Utilities.CheckArguments(vals, args=>args.Length==2, "<: ");
      SExp lhs = vals[0];
      SExp rhs = vals[1];
      if (!(lhs is Integer) || !(rhs is Integer))
      {
        string msg = "<: Parameters must be numeric value.";
        throw new TypeNotMatchException(msg);
      }
      return new Boolean(((int)(lhs as Integer).Value) < ((int)(rhs as Integer).Value));
    }

    public static SExp GreaterThan(SExp[] vals, Environment env)
    {
      Utilities.CheckArguments(vals, args=>args.Length==2, ">: ");
      SExp lhs = vals[0];
      SExp rhs = vals[1];
      if (!(lhs is Integer) || !(rhs is Integer))
      {
        string msg = ">: Parameters must be numeric value.";
        throw new TypeNotMatchException(msg);
      }
      return new Boolean(((int)(rhs as Integer).Value) < ((int)(lhs as Integer).Value));
    }

    public static SExp EqualNumbers(SExp[] vals, Environment env)
    {
      Utilities.CheckArguments(vals, args=>args.Length==2, "=: ");
      SExp lhs = vals[0];
      SExp rhs = vals[1];
      if (!(lhs is Integer) || !(rhs is Integer))
      {
        string msg = "=: Parameters must be numeric value.";
        throw new TypeNotMatchException(msg);
      }
      return new Boolean(((int)(lhs as Integer).Value) == ((int)(rhs as Integer).Value));
    }
  }
}
