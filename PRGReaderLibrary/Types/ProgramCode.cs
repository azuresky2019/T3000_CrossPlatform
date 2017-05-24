﻿namespace PRGReaderLibrary
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public enum ExpressionType
    {
        DELIMITER,
        NUMBER,
        IDENTIFIER,
        KEYWORD,
        TEMP,
        BLOCKX,
        TIMES,
        COLON,
        SEMICOLON,
        GE = 96,
        LE,
        NL,
        POW = 101,
        MOD,
        [Name("*")]
        MUL,
        [Name("/")]
        DIV,
        [Name("-")]
        MINUSUNAR,
        [Name("+")]
        PLUS = 107,
        [Name("-")]
        MINUS,
        [Name("<")]
        LT,
        [Name(">")]
        GT,
        EQ = 113,
        NE,
        XOR,
        AND,
        OR,
        NOT,
        EOI,
        TTIME,
        BIT_AND,
        BIT_OR
    }

    public enum LineToken
    {
        BEEP = 7,
        ASSIGNAR,
        ASSIGN,
        CLEARX,
        FOR,
        NEXT = 13,
        IF,
        ELSE = 16,
        [Name("IF+")]
        IFP,
        [Name("IF-")]
        IFM,
        GOTO,
        GOSUB,
        RETURN,
        ENDPRG = 23,
        PRINT,
        REM = 26,
        PRINT_AT,
        STARTPRG,
        STOP,
        WAIT,
        HANGUP,
        PHONE,
        ALARM_AT,
        REMOTE_SET,
        RUN_MACRO,
        REMOTE_GET,
        ENABLEX,
        DISABLEX,
        ON_ERROR,
        SET_PRINTER,
        ASSIGNARRAY_1,
        GOTOIF,
        ON_ALARM,
        ASSIGNARRAY_2,
        OPEN,
        CLOSE,
        CALLB,
        DECLARE,
        ASSIGNARRAY,
        ON = 64,
        Alarm,
        DALARM = 67,
        USER_A = 69,
        USER_B,
        DIM = 138,
        _DATE = 160,
        PTIME,
        SENSOR_ON = 164,
        SENSOR_OFF,
        TO,
        STEP,
        THEN,
        LET,
        ALARM_AT_ALL,
        FINISHED,
        PRINT_ALARM_AT,
        PRINT_ALARM_AT_ALL,
        ARG,
        DO,
        WHILE,
        SWITCH,
        EOL,
        STRING
    }

    public enum FunctionToken
    {
        TUE = 2,
        WED,
        THU,
        FRI,
        SAT,
        SUN,
        MON,
        COM1 = 16,
        ABS = 50,
        AVG,
        DOY,
        DOW,
        _INT,
        MAX,
        MIN,
        SQR,
        Tbl,
        TIME,
        [Name("TIME-ON")]
        TIME_ON,
        TIME_OFF,
        INTERVAL,
        TIME_FORMAT,
        WR_ON,
        WR_OFF,
        UNACK,
        _Status = 71,
        RUNTIME,
        SCANS = 75,
        POWER_LOSS,
        LN,
        LN_1,
        OUTPUTD = 80,
        INKEYD,
        DOM,
        MOY,
        CONPROP = 86,
        CONRATE,
        CONRESET,
        CLEARPORT,
        JAN = 193,
        FEB,
        MAR,
        APR,
        MAY,
        JUN,
        JUL,
        AUG,
        SEP,
        OCT,
        NOV,
        DEC,
        LOCAL_POINT_PRG = 156,
        CONST_VALUE_PRG,
        REMOTE_POINT_PRG
    }

    public static class Enum<T>
    {
        public static bool IsDefined(T value)
        {
            return Enum.IsDefined(typeof(T), value);
        }
    }

    public static class ProgramCodeUtilities
    {
        public static bool IsLine(byte value) =>
            value == (byte)LineToken.ASSIGN;

        public static bool IsVariable(byte value) =>
            value == (byte)FunctionToken.CONST_VALUE_PRG ||
            value == (byte)FunctionToken.REMOTE_POINT_PRG ||
            value == (byte)FunctionToken.LOCAL_POINT_PRG ||
            value == (byte)FunctionToken.TIME_ON;

        public static bool IsFunction(byte value)
        {
            if (IsVariable(value))
            {
                return false;
            }

            if (value == (byte)LineToken.STOP)
            {
                return true;
            }

            return Enum<FunctionToken>.IsDefined((FunctionToken)value);
        }

        public static bool IsExpression(byte value)
        {
            if (IsVariable(value) || IsFunction(value) || value == 0)
            {
                return false;
            }

            return Enum<ExpressionType>.IsDefined((ExpressionType)value);
        }

        public static bool IsBinaryExpression(ExpressionType value)
        {
            return value != ExpressionType.NOT &&
                   value != ExpressionType.MINUSUNAR;
        }

        public enum ByteType
        {
            None,
            Variable,
            Line,
            Function,
            Expression
        }

        public static ByteType GetByteType(byte[] bytes, int offset)
        {
            var value = bytes[offset];

            if (IsVariable(value))
                return ByteType.Variable;
            else if (IsLine(value))
                return ByteType.Line;
            else if (IsFunction(value))
                return ByteType.Function;
            else if (IsExpression(value))
                return ByteType.Expression;

            return ByteType.None;
        }
    }

    public class Expression : Version
    {
        public ExpressionType Type { get; set; }

        public Expression(FileVersion version = FileVersion.Current)
            : base(version)
        { }

        public static Expression operator +(Expression expression1, Expression expression2) =>
            new BinaryExpression(expression1, expression2,
                ExpressionType.PLUS, expression1.FileVersion);

        public static Expression operator -(Expression expression1, Expression expression2) =>
            new BinaryExpression(expression1, expression2,
                ExpressionType.MINUS, expression1.FileVersion);
    }

    public class VariableExpression : Expression
    {
        public Prg Prg { get; set; }

        public FunctionToken Token { get; set; }
        public int Number { get; set; }

        public int Number2 { get; set; }
        public int Number3 { get; set; }
        public int Number4 { get; set; }
        public int Number5 { get; set; }

        public VariableValue Value { get; set; }

        public override string ToString()
        {
            switch (Token)
            {
                case FunctionToken.TIME_ON:
                    return $"{Token.GetName()} ( INIT )";

                case FunctionToken.CONST_VALUE_PRG:
                    var text = Value.Unit == Unit.Time
                        ? $"{Value}"
                        : $"{((double)Value.ToObject()).ToString("####")}";
                    return string.IsNullOrWhiteSpace(text)
                        ? "0"
                        : text;

                case FunctionToken.LOCAL_POINT_PRG:
                    return Prg.Variables[Number].Label ?? $"VAR{Number}";

                case FunctionToken.REMOTE_POINT_PRG:
                    return $"{Number3}.{Number4}.REG{Number + 1}";

                default:
                    return $"{Token.GetName()}";
            }
        }


        #region Binary data

        public VariableExpression(byte[] bytes, Prg prg, ref int offset,
            FileVersion version = FileVersion.Current)
            : base(version)
        {
            Prg = prg;
            Token = (FunctionToken)bytes.ToByte(ref offset);

            switch (Token)
            {
                case FunctionToken.TIME_ON:
                    Number = bytes.ToByte(ref offset);
                    Type = (ExpressionType)bytes.ToByte(ref offset);
                    break;

                case FunctionToken.LOCAL_POINT_PRG:
                    Number = bytes.ToByte(ref offset);
                    Type = (ExpressionType)bytes.ToByte(ref offset);
                    break;

                case FunctionToken.REMOTE_POINT_PRG:
                    Number = bytes.ToByte(ref offset);
                    Number2 = bytes.ToByte(ref offset);
                    Number3 = bytes.ToByte(ref offset);
                    Number4 = bytes.ToByte(ref offset);
                    Number5 = bytes.ToByte(ref offset);
                    break;

                case FunctionToken.CONST_VALUE_PRG:
                    var value = bytes.ToInt32(ref offset);
                    var unit = Unit.Unused;
                    if (offset < bytes.Length)
                    {
                        var valueType = (FunctionToken)bytes.ToByte(ref offset);
                        if (valueType == FunctionToken.TIME_FORMAT)
                        {
                            unit = Unit.Time;
                        }
                        else
                        {
                            offset -= 1;
                        }
                    }
                    Value = new VariableValue(value, unit);
                    break;

                default:
                    //Number = bytes.ToByte(ref offset);
                    //Type = (ExpressionType)bytes.ToByte(ref offset);
                    break;
                    //throw new NotImplementedException(
                    //    $"Function token: {Token} not implemented.");
            }
        }

        public byte[] ToBytes()
        {
            var bytes = new List<byte>();

            return bytes.ToArray();
        }

        #endregion

    }

    public class FunctionExpression : Expression
    {
        public FunctionToken Token { get; set; }
        public List<Expression> Arguments { get; set; } = new List<Expression>();

        public FunctionExpression(
            FunctionToken token,
            FileVersion version = FileVersion.Current, params Expression[] arguments)
            : base(version)
        {
            Token = token;

            Arguments.AddRange(arguments);
        }

        public override string ToString()
        {
            switch (Type)
            {
                case (ExpressionType)29:
                    return $"STOP {string.Join(" , ", Arguments.Select(i => i.ToString()))}";

                default:
                    return $"{Token.GetName()} ( {string.Join(" , ", Arguments.Select(i => i.ToString()))} )";
            }
        }

        #region Binary data


        public FunctionExpression(byte[] bytes, Prg prg, ref int offset,
            FileVersion version = FileVersion.Current)
            : base(version)
        {
            Arguments.Add(new VariableExpression(bytes, prg, ref offset, FileVersion));
            var token = bytes.ToByte(ref offset);
            while (!ProgramCodeUtilities.IsFunction(token))
            {
                --offset;
                Arguments.Add(new VariableExpression(bytes, prg, ref offset, FileVersion));
                token = bytes.ToByte(ref offset);
            }
            Token = (FunctionToken)token;
        }

        public byte[] ToBytes()
        {
            var bytes = new List<byte>();

            return bytes.ToArray();
        }

        #endregion

    }

    public class UnaryExpression : Expression
    {
        public Expression Expression { get; set; }

        public UnaryExpression(Expression expression,
            ExpressionType type,
            FileVersion version = FileVersion.Current)
            : base(version)
        {
            Expression = expression;
            Type = type;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case ExpressionType.MINUSUNAR:
                    return $"{Type.GetName()}{Expression.ToString()}";

                case ExpressionType.NOT:
                    return $"{Type.GetName()} {Expression.ToString()}";

                case (ExpressionType)LineToken.STOP:
                    return $"STOP {Expression.ToString()}";

                default:
                    return $"{Type.GetName()} ( {Expression.ToString()} )";
            }
        }

        #region Binary data

        public UnaryExpression(byte[] bytes, Prg prg, ref int offset,
            FileVersion version = FileVersion.Current)
            : base(version)
        {
            Expression = new VariableExpression(bytes, prg, ref offset, FileVersion);
            Type = (ExpressionType)bytes.ToByte(ref offset);
        }

        public byte[] ToBytes()
        {
            var bytes = new List<byte>();

            return bytes.ToArray();
        }

        #endregion

    }

    public class BinaryExpression : Expression
    {
        public Expression First { get; set; }
        public Expression Second { get; set; }

        public BinaryExpression(Expression first, Expression second,
            ExpressionType type,
            FileVersion version = FileVersion.Current)
            : base(version)
        {
            First = first;
            Second = second;
            Type = type;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case (ExpressionType)9:
                    return $"{First.ToString()} = {Second.ToString()}";

                case ExpressionType.DELIMITER:
                    return $"{First.ToString()} {Second.ToString()}";

                default:
                    return $"{First.ToString()} {Type.GetName()} {Second.ToString()}";
            }
        }

        #region Binary data

        public BinaryExpression(byte[] bytes, Prg prg, ref int offset,
            FileVersion version = FileVersion.Current)
            : base(version)
        {
            First = new VariableExpression(bytes, prg, ref offset, FileVersion);
            Second = new VariableExpression(bytes, prg, ref offset, FileVersion);
            Type = (ExpressionType)bytes.ToByte(ref offset);
        }

        public byte[] ToBytes()
        {
            var bytes = new List<byte>();

            return bytes.ToArray();
        }

        #endregion

    }

    public class Line : Version
    {
        public ExpressionType Type { get; set; }
        public int Number { get; set; }
        public LineToken Token { get; set; }
        public byte[] Data { get; set; }
        public List<Expression> Expressions { get; set; } = new List<Expression>();

        public override string ToString()
        {
            switch (Token)
            {
                case LineToken.REM:
                    return $"{Number} {Token} {Data.GetString()}";

                case LineToken.IF:
                case LineToken.IFM:
                case LineToken.IFP:
                    var text = $"{Number} {Token.GetName()} {Expressions[0]?.ToString()}";

                    if (Expressions.Count >= 2)
                    {
                        text += $" THEN {Expressions[1]?.ToString()}";
                    }

                    if (Expressions.Count >= 3)
                    {
                        text += $" ELSE {Expressions[2]?.ToString()}";
                    }

                    return text;


                case LineToken.ASSIGN:
                    return $"{Number} {Expressions[0].ToString()} = {Expressions[1].ToString()}";

                default:
                    Console.WriteLine(Token);
                    return string.Empty;
            }
        }


        #region Binary data

        public static byte[] GetLinePart(byte[] bytes, ref int offset,
            FileVersion version = FileVersion.Current,
            int nextPart = 0)
        {
            var i = offset;
            for (; bytes[i] != 255; ++i) ;
            if (nextPart != 0)
            {
                i = bytes[nextPart - 1] != 255 ? nextPart : nextPart - 1;
            }
            else
            {
                //--i;
            }

            var part = bytes.ToBytes(offset, i - offset);
            offset = i;

            return part;
        }

        private List<Expression> GetExpressions(byte[] bytes, Prg prg, ref int offset,
            FileVersion version = FileVersion.Current, LineToken lineToken = 0,
            bool isTrace = false)
        {
            if (isTrace)
            {
                Console.WriteLine("----------------------------");
                Console.WriteLine(DebugUtilities.CompareBytes(bytes, bytes,
                    onlyDif: false, toText: false, offset: offset));
            }

            var expressions = new List<Expression>();

            while (offset < bytes.Length)
            {
                var byteType = ProgramCodeUtilities.GetByteType(bytes, offset);

                if (byteType == ProgramCodeUtilities.ByteType.Variable)
                {
                    if (isTrace)
                    {
                        Console.WriteLine($"IsVariable {offset}");
                    }
                    var expression = new VariableExpression(bytes, prg, ref offset, FileVersion);
                    expressions.Add(expression);
                }
                else if (byteType == ProgramCodeUtilities.ByteType.Line)
                {
                    if (isTrace)
                    {
                        Console.WriteLine($"IsLine {offset}");
                    }
                    var token = (LineToken)bytes.ToByte(ref offset);
                    var lineExpressions = GetExpressions(bytes, prg, ref offset, version, token, isTrace);
                    var expression = lineExpressions.Count > 1 ? new BinaryExpression(
                        lineExpressions[0],
                        lineExpressions[1],
                        (ExpressionType)token, FileVersion) :
                        lineExpressions[0];
                    expressions.Add(expression);
                }
                else if (byteType == ProgramCodeUtilities.ByteType.Function)
                {
                    if (isTrace)
                    {
                        Console.WriteLine($"IsFunction {offset}");
                    }
                    var token = (FunctionToken)bytes.ToByte(ref offset);
                    if ((byte)token == (byte)LineToken.STOP)
                    {
                        var variable = new VariableExpression(bytes, prg, ref offset, FileVersion);
                        var expression = new UnaryExpression(variable, (ExpressionType)LineToken.STOP, version);

                        expressions.Add(expression);
                    }
                    else if (expressions.Count == 0)
                    {
                        Console.WriteLine("expressions.Count = 0");
                        var temp = 0;
                        offset -= 1;
                        expressions.Add(new VariableExpression(bytes, prg, ref offset, FileVersion));
                        expressions.AddRange(
                            GetExpressions(bytes.ToBytes(ref offset, 3), prg, ref temp, FileVersion, lineToken));

                        var prevLastExpression = expressions[expressions.Count - 2];
                        var lastExpression = expressions[expressions.Count - 1];
                        expressions.RemoveAt(expressions.Count - 1);
                        expressions.RemoveAt(expressions.Count - 1);
                        var expression = new BinaryExpression(prevLastExpression, lastExpression, ExpressionType.DELIMITER, FileVersion);
                        expressions.Add(expression);
                    }
                    else
                    {
                        var expression = new FunctionExpression(token, FileVersion, expressions.ToArray());
                        expressions.Clear();
                        expressions.Add(expression);
                    }
                }
                else if (byteType == ProgramCodeUtilities.ByteType.Expression)
                {
                    if (isTrace)
                    {
                        Console.WriteLine($"IsExpression {offset}");
                    }
                    var type = (ExpressionType)bytes.ToByte(ref offset);
                    if (ProgramCodeUtilities.IsBinaryExpression(type))
                    {
                        var prevLastExpression = expressions[expressions.Count - 2];
                        var lastExpression = expressions[expressions.Count - 1];
                        expressions.RemoveAt(expressions.Count - 1);
                        expressions.RemoveAt(expressions.Count - 1);
                        var expression = new BinaryExpression(prevLastExpression, lastExpression, type, FileVersion);
                        expressions.Add(expression);
                    }
                    else
                    {
                        var lastExpression = expressions[expressions.Count - 1];
                        expressions.RemoveAt(expressions.Count - 1);
                        var expression = new UnaryExpression(lastExpression, type, FileVersion);
                        expressions.Add(expression);
                    }
                }
                else
                {
                    break;
                }
            }

            if (isTrace)
            {
                Console.WriteLine($"Result: {expressions.Count} expressions. Line token: {lineToken}");
                for (var i = 0; i < expressions.Count; ++i)
                {
                    Console.WriteLine($"Expression {i + 1}: {expressions[i].ToString()}");
                }
                Console.WriteLine("----------------------------");
                Console.WriteLine("");
            }

            return expressions;
        }

        private Expression ToExpression(byte[] bytes, Prg prg, int offset = 0,
            FileVersion version = FileVersion.Current, bool isTrace = false)
        {
            var expressions = GetExpressions(bytes, prg, ref offset, FileVersion, isTrace: isTrace);

            if (offset != bytes.Length || expressions.Count != 1)
            {
                Console.WriteLine($@"Bad part:
bytes: {DebugUtilities.CompareBytes(bytes, bytes, onlyDif: false, toText: false)}
offset: {offset}
bytes.Length: {bytes.Length}
expressions: {expressions.Count}
");
            }

            //One expression
            if (expressions.Count == 1)
            {
                return expressions[0];
            }

            return null;
        }

        private void FromASSIGN(byte[] bytes, Prg prg, ref int offset,
            FileVersion version = FileVersion.Current)
        {
            Expressions.Add(new VariableExpression(bytes, prg, ref offset, FileVersion));

            var first = new VariableExpression(bytes, prg, ref offset, FileVersion);
            var second = new VariableExpression(bytes, prg, ref offset, FileVersion);
            var type = (ExpressionType)bytes.ToByte(ref offset);
            Expression expression = new BinaryExpression(first, second, type, FileVersion);

            var temp = offset;
            var check = bytes.ToByte(ref offset);
            --offset;
            //Console.WriteLine($"{Number} {check} before");
            if (check != (byte)ExpressionType.NUMBER)
            {
                if (ProgramCodeUtilities.IsVariable(check))
                {
                    var third = new VariableExpression(bytes, prg, ref offset, FileVersion);
                    var type2 = (ExpressionType)bytes.ToByte(ref offset);
                    expression = new BinaryExpression(expression, third, type2, FileVersion);
                }
                else if (ProgramCodeUtilities.IsFunction(check))
                {
                    var token = (FunctionToken)bytes.ToByte(ref offset);
                    expression = new FunctionExpression(token, FileVersion, expression);
                }

                //++offset;
                //check = (ExpressionType)bytes.ToByte(ref offset);
                //--offset;
                //Console.WriteLine($"{Number} {check} after");
            }

            Expressions.Add(expression);
        }

        private void FromIFM(byte[] bytes, Prg prg, ref int offset,
            FileVersion version = FileVersion.Current)
        {
            var ifPart = GetLinePart(bytes, ref offset, FileVersion);
            Expressions.Add(ToExpression(ifPart, prg, 0, FileVersion));
            offset += 4;
            var thenPart = GetLinePart(bytes, ref offset, FileVersion);
            Expressions.Add(ToExpression(thenPart, prg, 0, FileVersion));
            offset += 1;
        }

        private void FromIF(byte[] bytes, Prg prg, ref int offset,
            FileVersion version = FileVersion.Current)
        {
            var ifPart = GetLinePart(bytes, ref offset, FileVersion);
            Expressions.Add(ToExpression(ifPart, prg, 0, FileVersion));
            var check = bytes.ToByte(ref offset);
            if (check != 0xFF)
            {
                throw new OffsetException(offset, ifPart.Length);
            }
            var nextOffset = bytes.ToUInt16(ref offset);

            var thenPart = GetLinePart(bytes, ref offset, FileVersion, nextOffset);
            Expressions.Add(ToExpression(thenPart, prg, 0, FileVersion));
            if (bytes[offset] == 0xFF)
                offset += 1; //0xFF byte
            if (offset != nextOffset)
            {
                throw new OffsetException(offset, nextOffset);
            }

            return;
        }

        public Line(byte[] bytes, Prg prg, ref int offset,
            FileVersion version = FileVersion.Current)
            : base(version)
        {
            Type = (ExpressionType)bytes.ToByte(ref offset);
            Number = bytes.ToInt16(ref offset);
            Token = (LineToken)bytes.ToByte(ref offset);

            try
            {
                if (Number == 140)
                {

                    var ifPart = GetLinePart(bytes, ref offset, FileVersion);
                    Expressions.Add(ToExpression(ifPart, prg, 0, FileVersion));
                    var check = bytes.ToByte(ref offset);
                    if (check != 0xFF)
                    {
                        throw new OffsetException(offset, ifPart.Length);
                    }
                    var nextOffset = bytes.ToUInt16(ref offset);

                    //Expressions.Add(new FunctionExpression(bytes, prg, ref offset, FileVersion));
                    //offset += 4;
                    //offset += 3;
                    //var type = (ExpressionType)bytes.ToByte(ref offset);
                    //var first = new VariableExpression(bytes, prg, ref offset, FileVersion);
                    //var second = new FunctionExpression(bytes, prg, ref offset, FileVersion);
                    //Expressions.Add(new BinaryExpression(first, second, type, FileVersion));
                    //offset += 2;

                    var thenPart = GetLinePart(bytes, ref offset, FileVersion, nextOffset);
                    Expressions.Add(ToExpression(thenPart, prg, 0, FileVersion, isTrace: true));
                    if (bytes[offset] == 0xFF)
                        offset += 1; //0xFF byte
                    if (offset != nextOffset)
                    {
                        throw new OffsetException(offset, nextOffset);
                    }

                    return;
                }

                if (Number == 300)
                {
                    Expressions.Add(new UnaryExpression(bytes, prg, ref offset, FileVersion));
                    offset += 3;
                    var type = (ExpressionType)bytes.ToByte(ref offset);
                    var first = new VariableExpression(bytes, prg, ref offset, FileVersion);
                    var expression = new VariableExpression(bytes, prg, ref offset, FileVersion) +
                         new VariableExpression(bytes, prg, ref offset, FileVersion);
                    expression.Type = (ExpressionType)bytes.ToByte(ref offset);
                    expression = new FunctionExpression(
                        (FunctionToken)bytes.ToByte(ref offset), FileVersion,
                        expression);
                    expression = first + expression;
                    expression.Type = type;
                    Expressions.Add(expression);
                    offset += 1;
                    return;
                }

                if (Number == 350)
                {
                    var ifPart = GetLinePart(bytes, ref offset, FileVersion);
                    Expressions.Add(ToExpression(ifPart, prg, 0, FileVersion));
                    offset += 3;
                    var type = (ExpressionType)bytes.ToByte(ref offset);
                    var first = new VariableExpression(bytes, prg, ref offset, FileVersion);
                    var second = new UnaryExpression(bytes, prg, ref offset, FileVersion);
                    var third = new VariableExpression(bytes, prg, ref offset, FileVersion);
                    var four = new VariableExpression(bytes, prg, ref offset, FileVersion);
                    var type2 = (FunctionToken)bytes.ToByte(ref offset);
                    offset += 1;
                    var type3 = (FunctionToken)bytes.ToByte(ref offset);
                    offset += 1;
                    Expression expression = new FunctionExpression(type2, FileVersion, third, four);
                    expression = new FunctionExpression(type3, FileVersion, second, expression);
                    expression = new BinaryExpression(first, expression, type, FileVersion);
                    Expressions.Add(expression);
                    offset += 1;
                    return;
                }

                if (Number == 360)
                {
                    var ifPart = GetLinePart(bytes, ref offset, FileVersion);
                    Expressions.Add(ToExpression(ifPart, prg, 0, FileVersion));
                    offset += 3;
                    var type = (ExpressionType)bytes.ToByte(ref offset);
                    var first = new VariableExpression(bytes, prg, ref offset, FileVersion);
                    var second = new VariableExpression(bytes, prg, ref offset, FileVersion);
                    var third = new VariableExpression(bytes, prg, ref offset, FileVersion);
                    var type2 = (ExpressionType)bytes.ToByte(ref offset);
                    var four = new VariableExpression(bytes, prg, ref offset, FileVersion);
                    var type3 = (ExpressionType)bytes.ToByte(ref offset);
                    Expression expression = new BinaryExpression(second, third, type2, FileVersion);
                    expression = new BinaryExpression(expression, four, type3, FileVersion);
                    expression = new BinaryExpression(first, expression, type, FileVersion);
                    Expressions.Add(expression);
                    offset += 5;
                    type = (ExpressionType)bytes.ToByte(ref offset);
                    first = new VariableExpression(bytes, prg, ref offset, FileVersion);
                    second = new VariableExpression(bytes, prg, ref offset, FileVersion);
                    expression = new BinaryExpression(first, second, type, FileVersion);
                    Expressions.Add(expression);
                    offset += 1;
                    return;
                }

                switch (Token)
                {
                    case LineToken.IF:
                        FromIF(bytes, prg, ref offset, FileVersion);
                        break;

                    case LineToken.IFM:
                        FromIFM(bytes, prg, ref offset, FileVersion);
                        break;

                    case LineToken.ASSIGN:
                        FromASSIGN(bytes, prg, ref offset, FileVersion);
                        break;

                    case LineToken.REM:
                    default:
                        var size = bytes.ToByte(ref offset);
                        Data = bytes.ToBytes(ref offset, size);
                        break;
                }

                if (Number == 380)
                {
                    offset += 115;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($@"Exception on line: 
Type: {Type}
Number: {Number}
Token: {Token}
{exception.Message}");
                throw;
            }
        }

        public byte[] ToBytes()
        {
            var bytes = new List<byte>();

            return bytes.ToArray();
        }

        #endregion

    }

    public class ProgramCode : BaseCode, IBinaryObject
    {
        public int Version { get; set; }
        public List<Line> Lines { get; set; } = new List<Line>();

        public override string ToString() => string.Join(Environment.NewLine,
            Lines.Select(line => line.ToString()));

        public ProgramCode(byte[] code = null, FileVersion version = FileVersion.Current)
            : base(2000, version)
        {
            Code = code;
        }

        #region Binary data

        public static int GetCount(FileVersion version = FileVersion.Current)
        {
            switch (version)
            {
                case FileVersion.Current:
                    return 16;

                default:
                    throw new FileVersionNotImplementedException(version);
            }
        }

        public static int GetSize(FileVersion version = FileVersion.Current)
        {
            switch (version)
            {
                case FileVersion.Current:
                    return 2000;

                default:
                    throw new FileVersionNotImplementedException(version);
            }
        }

        /// <summary>
        /// FileVersion.Current - need 2000 bytes
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <param name="version"></param>
        public ProgramCode(byte[] bytes, Prg prg, int offset = 0,
            FileVersion version = FileVersion.Current)
            : base(bytes, 2000, offset, version)
        {
            var maxSize = GetSize(FileVersion);

            Version = bytes.ToInt16(ref offset);
            while (offset < maxSize)
            {
                //Console.WriteLine($"{offset}: Line {Lines.Count}");
                var line = new Line(bytes, prg, ref offset, FileVersion);

                Lines.Add(line);
            }

            offset = 2000;
            CheckOffset(offset, GetSize(FileVersion));
        }

        /// <summary>
        /// FileVersion.Current - 2000 bytes
        /// </summary>
        /// <returns></returns>
        public new byte[] ToBytes()
        {
            var bytes = base.ToBytes();

            CheckSize(bytes.Length, GetSize(FileVersion));

            return bytes;
        }

        #endregion

    }
}
