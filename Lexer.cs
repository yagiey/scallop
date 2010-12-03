using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Scallop
{
  class Lexer
  {
    bool _inStr;
    bool _inInt;
    bool _inId;
    Predicate<char> _isWhiteSpace;

    public Lexer(Predicate<char> isWhiteSpace)
    {
      _inStr = false;
      _inInt = false;
      _inId  = false;
      _isWhiteSpace = isWhiteSpace;
    }

    static public bool IsAtomToken(LexerToken token)
    {
      return
        token is TokenString ||
        token is TokenInteger ||
        token is TokenIdentifier;
    }

    public IEnumerable<LexerToken> EnumerateTokens(string source)
    {
      StringReader reader = new StringReader(source);
      StringBuilder sb = new StringBuilder();
      int tmp = -1;

      while (-1 != (tmp = reader.Read()))
      {
        char peek = (char)tmp;
        if (_isWhiteSpace(peek))
        {
          if (!_inStr && !_inInt && !_inId) continue;
          else if (_inStr) sb.Append(peek.ToString());
          else if (_inInt)
          {
            _inInt = false;
            yield return new TokenInteger(sb.ToString());
            sb.Length = 0;
          }
          else
          {
            _inId = false;
            yield return new TokenIdentifier(sb.ToString());
            sb.Length = 0;
          }
        }
        else if (peek == '(')
        {
          if (_inStr) sb.Append(peek.ToString());
          if (_inInt)
          {
            _inInt = false;
            yield return new TokenInteger(sb.ToString());
            sb.Length = 0;
            yield return new TokenLeftParen();
          }
          else if (_inId)
          {
            _inId = false;
            yield return new TokenIdentifier(sb.ToString());
            sb.Length = 0;
            yield return new TokenLeftParen();
          }
          else { yield return new TokenLeftParen(); }
        }
        else if (peek == ')')
        {
          if (_inStr) sb.Append(peek.ToString());
          if (_inInt)
          {
            _inInt = false;
            yield return new TokenInteger(sb.ToString());
            sb.Length = 0;
            yield return new TokenRightParen();
          }
          else if (_inId)
          {
            _inId = false;
            yield return new TokenIdentifier(sb.ToString());
            sb.Length = 0;
            yield return new TokenRightParen();
          }
          else { yield return new TokenRightParen(); }
        }
        else if (peek == '"')
        {
          if (!_inStr && !_inInt && !_inId)
          {
            _inStr = true;
          }
          else if (_inStr)
          {
            _inStr = false;
            yield return new TokenString(sb.ToString());
            sb.Length = 0;
          }
          else if (_inInt)
          {
            _inInt = false;
            yield return new TokenInteger(sb.ToString());
            sb.Length = 0;
          }
          else
          {
            _inId = false;
            yield return new TokenIdentifier(sb.ToString());
            sb.Length = 0;
          }
        }
        else if (char.IsDigit(peek))
        {
          if (!_inStr && !_inInt && !_inId)
          {
            _inInt = true;
          }
          sb.Append(peek.ToString());
        }
        else
        {
          if (!_inStr && !_inInt && !_inId)
          {
            _inId = true;
          }
          else if (_inInt)
          {
            const string MESSAGE =
              "Non-Integer numeric value is not supported currently";
            throw new LexerException(MESSAGE);
          }
          else { }
          sb.Append(peek.ToString());
        }
      }
      reader.Close();

      if (0 < sb.Length)
      {
        if (_inInt) yield return new TokenInteger(sb.ToString());
        else if (_inId) yield return new TokenIdentifier(sb.ToString());
        else
        {
          const string MESSAGE = "Unexpected string termination.";
          throw new LexerException(MESSAGE);
        }
      }
    }
  }

  #region LexerToken

  abstract class LexerToken
  {
    string _val;

    public LexerToken(string str)
    {
      _val = str;
    }

    public string Value
    {
      get { return _val; }
    }

    public override string ToString()
    {
      return _val;
    }
  }

  class TokenLeftParen : LexerToken
  {
    public TokenLeftParen() : base("(") { }
  }

  class TokenRightParen : LexerToken
  {
    public TokenRightParen() : base(")") { }
  }

  class TokenInteger : LexerToken
  {
    public TokenInteger(string str) : base(str) { }
  }

  class TokenString : LexerToken
  {
    public TokenString(string str) : base(str) { }

    public override string ToString()
    {
      return string.Format("\"{0}\"", base.ToString());
    }
  }

  class TokenIdentifier : LexerToken
  {
    public TokenIdentifier(string str) : base(str) { }
  }

  #endregion
}
