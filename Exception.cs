using System;

namespace Scallop
{
  class Exception : System.Exception
  {
    public Exception() : base() { }
    public Exception(string msg) : base(msg) { }
  }

  class LexerException : Exception
  {
    public LexerException() : base() { }
    public LexerException(string msg) : base(msg) { }
  }

  class ParserException : Exception
  {
    public ParserException() : base() { }
    public ParserException(string msg) : base(msg) { }    
  }

  class EvalTimeException : Exception
  {
    public EvalTimeException() : base() { }
    public EvalTimeException(string msg) : base(msg) { }    
  }

  class NotSupportedYetException : EvalTimeException
  {
    public NotSupportedYetException() : base() { }
    public NotSupportedYetException(string msg) : base(msg) { }
  }

  class TypeNotMatchException : EvalTimeException
  {
    public TypeNotMatchException() : base() { }
    public TypeNotMatchException(string msg) : base(msg) { }
    public TypeNotMatchException(TypeNotMatchException e) : base(e.Message) { }
  }

  class IdentifierNotDefinedException : EvalTimeException
  {
    public IdentifierNotDefinedException() : base() { }
    public IdentifierNotDefinedException(string msg) : base(msg) { }
  }

  // S式オブジェクトを作成できなかった場合
  class CannotCreateSExpException : EvalTimeException
  {
    public CannotCreateSExpException() : base() { }
    public CannotCreateSExpException(string msg) : base(msg) { }
  }

  class CannotCreateClosureException : CannotCreateSExpException
  {
    public CannotCreateClosureException() : base() { }
    public CannotCreateClosureException(string msg) : base(msg) { }
  }

  class ArityNotMatchException : CannotCreateClosureException
  {
    public ArityNotMatchException() : base() { }
    public ArityNotMatchException(string msg) : base(msg) { }
  }

  class ApplicationTerminationException : EvalTimeException
  {
    public ApplicationTerminationException() : base() { }
    public ApplicationTerminationException(string msg) : base(msg) { }
  }
}
