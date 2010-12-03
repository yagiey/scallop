using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Entry = Scallop.Pair<string, Scallop.SExp>;

namespace Scallop
{
  abstract class SExp { }

  abstract class Atom : SExp
  {
    public abstract object Value { get; }
  }

  class Boolean : Atom
  {
    bool _val;

    public static Boolean FALSE;

    public static Boolean TRUE;

    static Boolean()
    {
      FALSE = new Boolean(false);
      TRUE = new Boolean(true);
    }

    public Boolean(bool b)
    {
      _val = b;
    }

    public override object Value
    {
      get { return _val; }
    }

    public override string ToString()
    {
      return _val ? "#t" : "#f";
    }
  }

  class Integer : Atom
  {
    int _val;

    public Integer(int n)
    {
      _val = n;
    }

    public override object Value
    {
      get { return _val; }
    }

    public override string ToString()
    {
      return _val.ToString();
    }
  }

  class String : Atom
  {
    string _val;

    public String(string str)
    {
      _val = str;
    }

    public override object Value
    {
      get { return _val; }
    }

    public override string ToString()
    {
      return string.Format("\"{0}\"", _val);
    }
  }

  class Identifier : Atom
  {
    string _val;

    public Identifier(string name)
    {
      _val = name;
    }

    public override object Value
    {
      get { return _val; }
    }

    public override string ToString()
    {
      return _val;
    }
  }

  class Procedure : SExp { }

  class PrimitiveProcedure : Procedure
  {
    string _name;

    Func<SExp[], Environment, SExp> _proc;

    public PrimitiveProcedure(Func<SExp[], Environment, SExp> proc, string name)
    {
      _name = name;
      _proc = proc;
    }

    public SExp Call(SExp[] vals, Environment env)
    {
      return _proc(vals, env);
    }

    public override string ToString()
    {
      return string.Format("{{procedure:{0}}}", _name);
    }
  }

  class Closure : Procedure
  {
    Environment _env;

    SExp[] _body;

    string[] _names;

    public Closure(string[] names, SExp[] body, Environment env)
    {
      _names = names;
      _body = body;
      _env = env;
    }

    public SExp Call(SExp[] vals)
    {
      EnvironmentFrame frame =
        EnvironmentFrame.FromNamesAndValues(_names, vals);
      SExp res = null;
      res = Eval(_env, frame, _body);
      return res;
    }

    public static SExp Eval(Environment env, EnvironmentFrame frame, SExp[] body)
    {
      Environment newEnv = new Environment(env);
      newEnv.Add(frame);
      return Utilities.Eval(body, newEnv);
    }

    public int Arity
    {
      get { return _names.Length; }
    }

    public override string ToString()
    {
      return "{closure}";
    }
  }

  class Syntax : SExp { }

  class PrimitiveSyntax : Syntax
  {
    string _name;

    Func<SExp[], Environment, SExp> _proc;

    public PrimitiveSyntax(Func<SExp[], Environment, SExp> proc, string name)
    {
      _name = name;
      _proc = proc;
    }

    public SExp Call(SExp[] vals, Environment env)
    {
      return _proc(vals, env);
    }

    public override string ToString()
    {
      return string.Format("{{syntax:{0}}}", _name);
    }
  }

  class Cell : SExp, IEnumerable<SExp>
  {
    SExp _car;
    SExp _cdr;

    public Cell(SExp car, SExp cdr)
    {
      _car = car;
      _cdr = cdr;
    }

    public SExp Car
    {
      get { return _car; }
    }

    public SExp Cdr
    {
      get { return _cdr; }
    }

    public IEnumerator<SExp> GetEnumerator()
    {
      for (Cell exp = this; exp != null; exp = exp.Cdr as Cell)
      {
        yield return (exp as Cell).Car;
      }
    }

    IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public override string ToString()
    {
      return ToString(this, true);
    }

    static string ToString(Cell cell, bool needParen)
    {
      StringBuilder sb = new StringBuilder();
      if (needParen) sb.Append("(");
     // if (!(cell is Nil))
      {
        SExp car = cell.Car;
        SExp cdr = cell.Cdr;

        sb.Append(car.ToString());
        if (cdr is Cell)
        {
          sb.Append(" ");
          sb.Append(Cell.ToString(cdr as Cell, false));
        }
        else if (cdr is Nil)
        {
          // nop
        }
        else
        {
          sb.Append(" . ");
          sb.Append(cdr.ToString());
        }
      }
      if (needParen) sb.Append(")");

      return sb.ToString();
    } 
  }

  class Nil : SExp
  {
    public override string ToString()
    {
      return "()";
    }
  }

  class Undefined : SExp { }

  class __LeftParen : SExp { }
}
