using System;
using System.Collections.Generic;
using Entry = Scallop.Pair<string, Scallop.SExp>;

namespace Scallop
{
  class Environment : IEnumerable<EnvironmentFrame>, ICollection<EnvironmentFrame>
  {
    List<EnvironmentFrame> _frames;
    
    public Environment() : this(new List<EnvironmentFrame>()) { }
    
    public Environment(IEnumerable<EnvironmentFrame> entries)
    {
      _frames = new List<EnvironmentFrame>(entries);
    }

    public static Environment MakeInitialEnvironment()
    {
      Environment env = new Environment();
      env.Add(EnvironmentFrame.MakeInitialFrame());
      return env;
    }
    
    public List<EnvironmentFrame> Frames
    {
      get { return _frames; }
    }

    public static Environment FromSExpression(SExp exp)
    {
      Environment env = new Environment();
      if (exp is Nil) return env;

      foreach (SExp frame in exp as Cell)
      {
        if (frame == null) break;
        env.Add(EnvironmentFrame.FromSExpression(frame));
      }
      return env;
    }

    public SExp ToSExpression()
    {
      SExp exp = new Nil();
      for (int i = _frames.Count - 1; 0 <= i; i--)
      {
        exp = new Cell(_frames[i].ToSExpression(), exp);
      }
      return exp;
    }

    public IEnumerator<EnvironmentFrame> GetEnumerator()
    {
      return _frames.GetEnumerator();
    }
    
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return ((_frames as System.Collections.IEnumerable)).GetEnumerator();
    }

    public void Add(Entry entry)
    {
      _frames[_frames.Count - 1].Add(entry);
    }
    
    public void Add(EnvironmentFrame item)
    {
      _frames.Add(item);
    }
    
    public void Clear()
    {
      _frames.Clear();
    }
    
    public bool Contains(EnvironmentFrame item)
    {
      return _frames.Contains(item);
    }
    
    public void CopyTo(EnvironmentFrame[] array, int arrayIndex)
    {
      _frames.CopyTo(array, arrayIndex);
    }
    
    public int Count
    {
      get { return _frames.Count; }
    }
    
    public bool IsReadOnly
    {
      get { return (_frames as ICollection<EnvironmentFrame>).IsReadOnly; }
    }
    
    public bool Remove(EnvironmentFrame item)
    {
      return _frames.Remove(item);
    }

    public bool Find(string name, out SExp exp)
    {
      exp = null;
      for (int i = _frames.Count - 1; 0 <= i; i--)
      {
        EnvironmentFrame frame = _frames[i];
        if (frame.Find(name, out exp))
        {
          return true;
        }
      }
      return false;
    }

    public bool Reassign(string name, SExp exp)
    {
      for (int i = _frames.Count - 1; 0 <= i; i--)
      {
        EnvironmentFrame frame = _frames[i];
        if (frame.Reassign(name, exp))
        {
          return true;
        }
      }
      return false;
    }
    
    public override string ToString()
    {
      string strEnv = string.Empty;
      for (int i = 0; i < _frames.Count; i++)
        {
          strEnv += _frames[i].ToString();
          if (i < _frames.Count - 1) strEnv += " ";
        }
      return string.Format("{{{0}}}", strEnv);
    }
    
  }
  
  class EnvironmentFrame : IEnumerable<Entry>, ICollection<Entry>
  {
    List<Entry> _entries;
    
    public EnvironmentFrame() : this(new List<Entry>()) { }
    
    public EnvironmentFrame(IEnumerable<Entry> entries)
    {
      _entries = new List<Entry>(entries);
    }

    public List<Entry> Entries
    {
      get { return _entries; }
    }

    public static EnvironmentFrame FromSExpression(SExp exp)
    {
      if (!(exp is Nil) && !(exp is Cell))
      {
        throw new TypeNotMatchException();
      }

      EnvironmentFrame frame = new EnvironmentFrame();
      if (exp is Nil) return frame;

      foreach (SExp e in exp as Cell)
      {
        Cell tmp = e as Cell;
        if (tmp == null) throw new TypeNotMatchException();

        SExp name = tmp.Car;
        if (!(name is Identifier)) throw new TypeNotMatchException();

        tmp = tmp.Cdr as Cell;
        if (tmp == null) throw new TypeNotMatchException();
        SExp val = tmp.Car;

        frame.Add(new Entry((name as Identifier).Value as string, val));
      }
      return frame;
    }

    public SExp ToSExpression()
    {
      SExp exp = new Nil();
      for (int i = _entries.Count - 1; 0 <= i; i--)
      {
        Entry entry = _entries[i];
        Identifier name = new Identifier(entry.Head);
        SExp val = entry.Rest;
        SExp elem = new Cell(name, new Cell(val, new Nil()));
        exp = new Cell(elem, exp);
      }
      return exp;
    }

    public static EnvironmentFrame MakeInitialFrame()
    {
      return
        new EnvironmentFrame(new Entry[]{
            new Entry("cons", new PrimitiveProcedure(Procedures.Cons, "cons")),
            new Entry("car", new PrimitiveProcedure(Procedures.Car, "car")),
            new Entry("cdr", new PrimitiveProcedure(Procedures.Cdr, "cdr")),
            new Entry("eq?", new PrimitiveProcedure(Procedures.Eq, "eq?")),
            new Entry("atom?", new PrimitiveProcedure(Procedures.Atom, "atom?")),
            new Entry("not", new PrimitiveProcedure(Procedures.Not, "not")),
            new Entry("null?", new PrimitiveProcedure(Procedures.Null, "null?")),
            new Entry("+", new PrimitiveProcedure(Procedures.Add, "+")),
            new Entry("-", new PrimitiveProcedure(Procedures.Sub, "-")),
            new Entry("*", new PrimitiveProcedure(Procedures.Mul, "*")),
            new Entry("<", new PrimitiveProcedure(Procedures.LessThan, "<")),
            new Entry(">", new PrimitiveProcedure(Procedures.GreaterThan, ">")),
            new Entry("=", new PrimitiveProcedure(Procedures.EqualNumbers, "=")),
            new Entry("exit", new PrimitiveProcedure(Procedures.Exit, "exit")),
            new Entry("eval", new PrimitiveProcedure(Procedures.Eval, "eval")),
            new Entry("interaction-environment", new PrimitiveProcedure(Procedures.InteractionEnvironment, "interaction-environment")),

            new Entry("define", new PrimitiveSyntax(Syntaxes.Define, "define")),
            new Entry("quote", new PrimitiveSyntax(Syntaxes.Quote, "quote")),
            new Entry("lambda", new PrimitiveSyntax(Syntaxes.Lambda, "lambda")),
            new Entry("let", new PrimitiveSyntax(Syntaxes.Let, "let")),
            new Entry("if", new PrimitiveSyntax(Syntaxes.If, "if")),
            new Entry("cond", new PrimitiveSyntax(Syntaxes.Cond, "cond")),
            new Entry("or", new PrimitiveSyntax(Syntaxes.Or, "or")),
            new Entry("and", new PrimitiveSyntax(Syntaxes.And, "and")),
            new Entry("set!", new PrimitiveSyntax(Syntaxes.Setq, "set!"))
          });
    }

    public static EnvironmentFrame FromNamesAndValues(string[] names, SExp[] vals)
    {
      if (names.Length != vals.Length)
        throw new ArityNotMatchException();

      List<Entry> entries = new List<Entry>();
      for (int i = 0; i < names.Length; i++)
      {
        entries.Add(new Entry(names[i], vals[i]));
      }

      return new EnvironmentFrame(entries);
    }
    
    public IEnumerator<Entry> GetEnumerator()
    {
      return _entries.GetEnumerator();
    }
    
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return ((_entries as System.Collections.IEnumerable)).GetEnumerator();
    }
    
    public void Add(Entry item)
    {
      _entries.Add(item);
    }
    
    public void Clear()
    {
      _entries.Clear();
    }
    
    public bool Contains(Entry item)
    {
      return _entries.Contains(item);
    }
    
    public void CopyTo(Entry[] array, int arrayIndex)
    {
      _entries.CopyTo(array, arrayIndex);
    }
    
    public int Count
    {
      get { return _entries.Count; }
    }
    
    public bool IsReadOnly
    {
      get { return (_entries as ICollection<Entry>).IsReadOnly; }
    }
    
    public bool Remove(Entry item)
    {
      return _entries.Remove(item);
    }

    public bool Find(string name, out SExp exp)
    {
      exp = null;
      for (int i = _entries.Count - 1; 0 <= i; i--)
      {
        Entry entry = _entries[i];
        if (entry.Head == name)
        {
          exp = entry.Rest;
          return true;
        }
      }
      return false;
    }

    public bool Reassign(string name, SExp exp)
    {
      for (int i = _entries.Count - 1; 0 <= i; i--)
      {
        Entry entry = _entries[i];
        if (entry.Head == name)
        {
          entry.Rest = exp;
          return true;
        }
      }
      return false;
    }
    
    public override string ToString()
    {
      string strFrame = string.Empty;
      for (int i = 0; i < _entries.Count; i++)
        {
          strFrame += _entries[i].ToString();
          if (i < _entries.Count - 1) strFrame += " ";
        }
      return string.Format("[{0}]", strFrame);
    }
  }
}
