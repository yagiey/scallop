all:
	gmcs -out:scallop.exe Main.cs Processor.cs Lexer.cs Parser.cs SExpression.cs Environment.cs Exception.cs Procedures.cs Syntaxes.cs Utilities.cs
